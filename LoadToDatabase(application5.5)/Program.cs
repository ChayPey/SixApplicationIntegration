using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;
using System.Xml;

namespace LoadToDatabase_application5._5_
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }

    public class LoadDatabase
    {
        // ����� ����� XML ���������
        public void PrintItem(XmlElement item, int indent = 0)
        {
            // ������� ��� ������ ��������.
            // new string('\t', indent) - ������� ������ ��������� �� indent �����.
            // ��� ����� ��� �������� ������.
            // ������ ������ ����� ����� �������� �� ��������� � �����.
            Console.Write($"{new string('\t', indent)}{item.LocalName} ");

            // ���� � �������� ���� ��������, 
            // �� ������� �� ����������, ������ � ���������� �������.
            foreach (XmlAttribute attr in item.Attributes)
            {
                Console.Write($"[{attr.InnerText}]");
            }

            // ���� � �������� ���� ��������� ��������, �� �������.
            foreach (var child in item.ChildNodes)
            {
                if (child is XmlElement node)
                {
                    // ���� ��������� ������� ���� �������,
                    // �� ��������� �� ����� ������ 
                    // � ���������� �������� �����.
                    // ��������� ������� ����� ������ �� ���� ������ ������.
                    Console.WriteLine();
                    PrintItem(node, indent + 1);
                }

                if (child is XmlText text)
                {
                    // ���� ��������� ������� �����,
                    // �� ������� ��� ����� ����.
                    Console.Write($"- {text.InnerText}");
                }
            }
        }

        // �������� �� ���� ������ XML �������
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

        // �������� ������� ������ ��� ���������� � ����� ������
        public string CreateConnection(string server, string user, string database, string password)
        {
            string lineConnectionDB = "server = " + server + "; user = " + user + "; database = " + database + "; password = " + password + ";";
            return lineConnectionDB;
        }
    }
}
