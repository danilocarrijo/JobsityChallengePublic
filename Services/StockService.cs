using Entities;
using Microsoft.Extensions.Options;
using RabbitMQConsumer;
using ServicesInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Services
{
    public class StockService : IStockService
    {
        private readonly StockSettings _options;
        public readonly PostRabbitMQHostedService _postRabbitMQHostedService;

        public StockService(IOptions<StockSettings> options,
            PostRabbitMQHostedService postRabbitMQHostedService)
        {
            _options = options.Value;
            _postRabbitMQHostedService = postRabbitMQHostedService;
        }

        public void GetStockValue(string user,string staockName)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_options.url.Replace("[staockname]", staockName));
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                var resp = reader.ReadToEnd();
                var respLines = resp.Split(
                    new[] { Environment.NewLine },
                    StringSplitOptions.None
                );
                respLines = respLines[1].Split(',');
                _postRabbitMQHostedService.Post($"{user};{respLines[0]} quote is ${respLines[6]} per share");
            }
        }
    }
}
