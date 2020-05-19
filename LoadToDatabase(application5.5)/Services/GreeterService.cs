using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace LoadToDatabase_application5._5_
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            // Загрузка данных из бд
            LoadDatabase loadDatabase = new LoadDatabase();
            string lineConnection = loadDatabase.CreateConnection("localhost", "root", "xml_data", "1C2z3x4VFdsaAsdf");
            XmlDocument xmlDocument = loadDatabase.Get(lineConnection, 1);
            return Task.FromResult(new HelloReply
            {
                Message = xmlDocument.InnerXml
            });
        }
    }
}
