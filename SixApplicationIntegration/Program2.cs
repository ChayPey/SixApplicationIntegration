using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;
using System.Xml;

namespace SixApplicationIntegration
{
    class Program
    {
        static void Main(string[] args)
        {
            // Запрашиваем URL
            Console.WriteLine("Введите URL, где находиться требуемы XML документ");
            string url = Console.ReadLine();
            // Запрашиваем Имя и рассположение файла
            Console.WriteLine("Введите имя XMl документа и куда его требуется сохранить.");
            string filename = Console.ReadLine();
            // Вызов запроса и сохранение
            RequestXML requestXML = new RequestXML();
            XmlDocument xDoc = requestXML.Request(url, filename);
            // Вывод для визуализацииXML Документа
            Console.WriteLine();
            requestXML.PrintItem(xDoc.DocumentElement);
        }
    }

    class RequestXML
    {
        // URL для запроса XML по умолчанию или для получения от 1 программы https://localhost:44312/weatherforecast
        private string DefaultURL = "http://www.cbr.ru/scripts/XML_daily.asp?date_req=";
        // Имя и рассположение файла по умолчанию
        private string DefauFileName = "курс_валют.xml";

        // Запрашивает  URL для запроса XML и сохраняет документ в указаное место
        public XmlDocument Request(string url, string filename)
        {
            // Установка поставщика кодирования для работы опредленных классов...
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            // Банк России почему то закодировал текст в windows-1254, почему, зачем?
            Encoding.GetEncoding("windows-1254");
            // Создаем xml документ
            XmlDocument xDoc = new XmlDocument();
            // Загружаем xml документ со по URL
            xDoc.Load(url);
            // Сохраняем документ в указанное место
            XmlTextWriter tw = new XmlTextWriter(filename, null);
            xDoc.Save(tw);
            // Возвращаем XML документ
            return xDoc;
        }

        // Используем дефолтное URL и имя файла
        public XmlDocument Request()
        {
            // Установка поставщика кодирования для работы опредленных классов...
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            // Банк России почему то закодировал текст в windows-1254, почему, зачем?
            Encoding.GetEncoding("windows-1254");
            // Создаем xml документ
            XmlDocument xDoc = new XmlDocument();
            // Загружаем xml документ со по URL
            xDoc.Load(DefaultURL);
            // Сохраняем документ в указанное место
            XmlTextWriter tw = new XmlTextWriter(DefauFileName, null);
            xDoc.Save(tw);
            // Возвращаем XML документ
            return xDoc;
        }

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
    }
}
