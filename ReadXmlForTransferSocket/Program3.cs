using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;
using System.Xml;
using System.Net.Sockets;

namespace ReadXmlForTransferSocket
{
    class Program
    {
        static void Main(string[] args)
        {
            // Ввод имени и расположения XMl документа - C:\Users\chaylord\source\repos\SixApplicationIntegration\SixApplicationIntegration\bin\Debug\netcoreapp3.1\курс_валют.xml
            Console.WriteLine("Введите имя XMl документа и его расположение.");
            string filename = Console.ReadLine();
            // Чтение и вывод в консоль XML документа
            XmlDocument xDoc = TransferSocket.LoadXML(filename);
            //XmlDocument xDoc = TransferSocket.LoadXML();
            // Вывод на экран для визуализации
            TransferSocket.PrintItem(xDoc.DocumentElement);
            Console.WriteLine();
            //// Тест приобразования в строку
            //Console.WriteLine(xDoc.InnerXml);
            // Создание соединения с приложением 4 и отправка документа
            if (TransferSocket.DispatchXML(xDoc))
            {
                Console.WriteLine("Документ XML был отправлен успешно!");
            }
            else
            {
                Console.WriteLine("Документ XML не был отправлен!");
            }
        }
    }

    public static class TransferSocket
    {
        // Имя и рассположение файла по умолчанию
        private static string DefauFileName = "C:/Users/chaylord/source/repos/SixApplicationIntegration/SixApplicationIntegration/bin/Debug/netcoreapp3.1/курс_валют.xml";
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

        public static XmlDocument LoadXML(string filename)
        {
            // Установка поставщика кодирования для работы опредленных классов...
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            // Банк России почему то закодировал текст в windows-1254, почему, зачем?
            Encoding.GetEncoding("windows-1254");
            // Создаем xml документ
            XmlDocument xDoc = new XmlDocument();
            // Загружаем xml документ со по URL
            xDoc.Load(filename);
            // Возвращаем XML документ
            return xDoc;
        }

        public static XmlDocument LoadXML()
        {
            // Установка поставщика кодирования для работы опредленных классов...
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            // Банк России почему то закодировал текст в windows-1254, почему, зачем?
            Encoding.GetEncoding("windows-1254");
            // Создаем xml документ
            XmlDocument xDoc = new XmlDocument();
            // Загружаем xml документ со по URL
            xDoc.Load(DefauFileName);
            // Возвращаем XML документ
            return xDoc;
        }

        // Отправка XML Документа
        public static bool DispatchXML(XmlDocument xDoc)
        {
            try
            {
                // Буффер для данных
                byte[] data;
                // Создание сервера для прослушки соединений
                Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 19970));
                listener.Listen(40);
                // Установка соединения
                Socket client = listener.Accept();
                // Упаковка данных
                data = Encoding.UTF8.GetBytes(xDoc.InnerXml);
                // Отправка данных
                client.Send(data);
                // Закрываем соединение и прослушку
                listener.Close();
                client.Close();
                // Успешность отправки
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
