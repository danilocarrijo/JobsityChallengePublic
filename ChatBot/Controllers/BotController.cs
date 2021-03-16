using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServicesInterface;

namespace ChatBot.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BotController : ControllerBase
    {

        private readonly ILogger<BotController> _logger;
        public readonly IStockService _stockService;

        public BotController(
            ILogger<BotController> logger
            , IStockService stockService)
        {
            _logger = logger;
            _stockService = stockService;
        }

        [HttpPost("{user}/{command}")]
        public IActionResult ExecCommand(string user, string command)
        {
            try
            {
                var mes = command.Split("=");

                _ = (mes[0]) switch
                {
                    "stock" => Task.Run(() => _stockService.GetStockValue(user, mes[1])),
                    _ => throw new Exception("Command not found"),
                };

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(601, ex.Message);
            }
        }
    }
}
