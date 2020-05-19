using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;
using System.Xml;
using System.Net.Sockets;
using MySql.Data.MySqlClient;

namespace LoadToDatabase_application5_
{
    class Program
    {
        static void Main(string[] args)
        {
            // Загрузка данных из бд
            LoadDatabase loadDatabase = new LoadDatabase();
            string lineConnection = loadDatabase.CreateConnection("localhost", "root", "xml_data", "1C2z3x4VFdsaAsdf");
            XmlDocument xmlDocument = loadDatabase.Get(lineConnection, 1);
            // Вывод загруженных данных
            loadDatabase.PrintItem(xmlDocument.DocumentElement);
            // Ожидание завершения
            Console.Read();
        }
    }

    public class LoadDatabase
    {
        // Вывод древа XML документа
        public void PrintItem(XmlElement item, int indent = 0)
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

        // Загрузка из базы данных XML таблицы
        public XmlDocument Get(string lineConnection, int id)
        {
            XmlDocument result = new XmlDocument(); ;
            using (MySqlConnection connectionDatabase = new MySqlConnection(lineConnection))
            {
                MySqlCommand command = connectionDatabase.CreateCommand();
                command.CommandText = "SELECT xml_text FROM xmls WHERE Id = @id;";
                command.Parameters.AddWithValue("@id", id);
                connectionDatabase.Open();
                result.InnerXml = (string)command.ExecuteScalar();
            }
            return result;
        }

        // Создание входных данных для соединения с базой данных
        public string CreateConnection(string server, string user, string database, string password)
        {
            string lineConnectionDB = "server = " + server + "; user = " + user + "; database = " + database + "; password = " + password + ";";
            return lineConnectionDB;
        }
    }
}
