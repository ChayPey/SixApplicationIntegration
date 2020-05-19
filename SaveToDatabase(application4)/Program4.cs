using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;
using System.Xml;
using System.Net.Sockets;
using MySql.Data.MySqlClient;

namespace SaveToDatabase_application4_
{
    class Program
    {
        static void Main(string[] args)
        {
            // Получаем данные от приложения 3
            XmlDocument xDoc = SaveDatabase.ReceiveData();
            // Выводим полученные данные от приложения 3 для визуализации
            Console.WriteLine("Вывод полученных данных от приложения 3 в консоль!");
            Console.WriteLine();
            SaveDatabase.PrintItem(xDoc.DocumentElement);
            Console.WriteLine();
            // Создание подключения к базе данных
            string lineConnection = SaveDatabase.CreateConnection("localhost", "root", "xml_data", "1C2z3x4VFdsaAsdf");
            // Сохранение в базу данныъ
            bool result = SaveDatabase.SaveDB(xDoc, lineConnection);
            if (result)
            {
                Console.WriteLine("Данные сохранены успешно!");
            }
            else
            {
                Console.WriteLine("Данные не сохранены!");
            }
            // Ожидание завершения
            Console.Read();
        }
    }


    public static class SaveDatabase
    {

        // Вывод древа XML документа
        public static void PrintItem(XmlElement item, int indent = 0)
        {
            // Выводим имя самого элемента.
            // new string('\t', indent) - создает строку состоящую из indent табов.
            // Это нужно для смещения вправо.
            // Пробел справа нужен чтобы атрибуты не прилипали к имени.
            Console.Write($"{new string('\t', indent)}{item.LocalName} ");

            // Если у элемента есть атрибуты, 
            // то выводим их поочередно, каждый в квадратных скобках.
            foreach (XmlAttribute attr in item.Attributes)
            {
                Console.Write($"[{attr.InnerText}]");
            }

            // Если у элемента есть зависимые элементы, то выводим.
            foreach (var child in item.ChildNodes)
            {
                if (child is XmlElement node)
                {
                    // Если зависимый элемент тоже элемент,
                    // то переходим на новую строку 
                    // и рекурсивно вызываем метод.
                    // Следующий элемент будет смещен на один отступ вправо.
                    Console.WriteLine();
                    PrintItem(node, indent + 1);
                }

                if (child is XmlText text)
                {
                    // Если зависимый элемент текст,
                    // то выводим его через тире.
                    Console.Write($"- {text.InnerText}");
                }
            }
        }

        // Получение данных от приложения 3 через интерфейс сокета
        public static XmlDocument ReceiveData()
        {
            // Устанавливаем соединение с приложением 3
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 19970));
            // Буффер для данных 
            byte[] data = new byte[100000];
            string xmlText;
            // получаем данные
            int size = socket.Receive(data);
            xmlText = Encoding.UTF8.GetString(data);
            // Установка поставщика кодирования для работы опредленных классов...
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            // Банк России почему то закодировал текст в windows-1254, почему, зачем?
            Encoding.GetEncoding("windows-1254");
            // Создаем xml документ
            XmlDocument xDoc = new XmlDocument();
            // Загружаем xml документ со по URL
            xDoc.InnerXml = xmlText;
            return xDoc;
        }

        // создание соединения с базой данных
        public static string CreateConnection(string server, string user, string database, string password)
        {
            string lineConnectionDB = "server = " + server + "; user = " + user + "; database = " + database + "; password = " + password + ";";
            return lineConnectionDB;
        }

        // сохранение данных в db
        public static bool SaveDB(XmlDocument xDoc, string lineConnection)
        {
            try
            {
                using (MySqlConnection connectionDatabase = new MySqlConnection(lineConnection))
                {
                    MySqlCommand command = connectionDatabase.CreateCommand();
                    command.CommandText = "UPDATE xmls SET xml_text = @count WHERE Id = @id;";
                    command.Parameters.AddWithValue("@id", 1);
                    command.Parameters.AddWithValue("@count", xDoc.InnerXml);
                    connectionDatabase.Open();
                    command.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибки при выполнение:");
                Console.WriteLine($"Исключение: {ex.Message}");
                Console.WriteLine($"Метод: {ex.TargetSite}");
                Console.WriteLine($"Трассировка стека: {ex.StackTrace}");
                return false;
            }
        }
    }

}
