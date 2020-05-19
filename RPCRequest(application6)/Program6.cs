using Grpc.Net.Client;
using LoadToDatabase_application5._5_;
using System;
using System.Threading.Tasks;
using System.Xml;
using System.Net.Mail;
using System.Net;
using System.IO;

namespace RPCRequest_application6_
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // создаем канал для обмена сообщениями с сервером
            // параметр - адрес сервера gRPC
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            // создаем клиента
            var client = new Greeter.GreeterClient(channel);
            Console.Write("Ввидите почту для отправки:");
            string name = Console.ReadLine();
            // обмениваемся сообщениями с сервером
            var reply = await client.SayHelloAsync(new HelloRequest { Name = name });
            //Console.WriteLine("Ответ сервера: " + reply.Message);
            XmlDocument result = new XmlDocument();
            result.InnerXml = reply.Message;
            // вывод полученного xml документа
            PrintItem(result.DocumentElement);
            // Отправка сообщения на почту по протоколу SMTP
            SendEmail(name, result.InnerXml);

            Console.WriteLine("Что то куда то отправленно!");
            Console.ReadKey();
        }

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

        // Отправляет эл. письмо на адрес по протоколу SMTP например(tim.urfu@mail.ru).
        public static void SendEmail(string email, string text)
        {
            // отправитель - устанавливаем адрес и отображаемое в письме имя
            MailAddress from = new MailAddress("chaylord@bk.ru", "Vasiliy");
            // кому отправляем
            MailAddress to = new MailAddress(email);
            // создаем объект сообщения
            MailMessage m = new MailMessage(from, to);
            // тема письма
            m.Subject = "Курс валют";
            // текст письма
            m.Body = $"<h2>{text}</h2>";
            // письмо представляет код html
            m.IsBodyHtml = true;
            // адрес smtp-сервера и порт, с которого будем отправлять письмо
            SmtpClient smtp = new SmtpClient("smtp.bk.ru", 2525);
            // логин и пароль от вашей почты, а выше домееная хрень, можно посмотреть в настрйоках
            smtp.Credentials = new NetworkCredential("chaylord@bk.ru", "****************");
            smtp.EnableSsl = true;
            smtp.Send(m);
        }
    }
}
