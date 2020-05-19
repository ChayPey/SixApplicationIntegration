using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApplicationREST.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        // URL для запроса XML по умолчанию
        private string DefaultURL = "http://www.cbr.ru/scripts/XML_daily.asp?date_req=";
        // Используем дефолтное URL и имя файла
        private XmlDocument RequestXmlDocument()
        {
            // Установка поставщика кодирования для работы опредленных классов...
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            // Банк России почему то закодировал текст в windows-1254, почему, зачем?
            Encoding.GetEncoding("windows-1254");
            // Создаем xml документ
            XmlDocument xDoc = new XmlDocument();
            // Загружаем xml документ со по URL
            xDoc.Load(DefaultURL);
            // Возвращаем XML документ
            return xDoc;
        }

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {        
            return RequestXmlDocument().InnerXml;
        }
    }
}
