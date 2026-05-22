using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text.Json;
using System.Threading.Tasks;
using DataLogger.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Xml.Linq;
using DataLogger.DAO;

namespace DataLogger.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly FiwareServices _fiwareServices;

        private static bool jaExecutado = false;

        public HomeController(ILogger<HomeController> logger, FiwareServices fiwareServices)
        {
            _logger = logger;
            _fiwareServices = fiwareServices;
        }
     
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> historicoLuminosidade(string serverIp, int lastN)
        {
            
            if (jaExecutado == false)
            {
                await _fiwareServices.SubscribeParameters(serverIp);
                jaExecutado = true;
            }

            var historicoLuminosidade = await _fiwareServices.RequestLuminosity(serverIp, lastN);

            return Content(historicoLuminosidade, "application/json");
        }

        public async Task<IActionResult> historicoTemperatura(string serverIp, int lastN)
        {

            if (jaExecutado == false)
            {
                await _fiwareServices.SubscribeParameters(serverIp);
                jaExecutado = true;
            }

            var historicoTemperatura = await _fiwareServices.RequestTemperature(serverIp, lastN);

            return Content(historicoTemperatura, "application/json");
        }

        public async Task<IActionResult> historicoHumidade(string serverIp, int lastN)
        {

            if (jaExecutado == false)
            {
                await _fiwareServices.SubscribeParameters(serverIp);
                jaExecutado = true;
            }

            var historicoHumidade = await _fiwareServices.RequestHumidity(serverIp, lastN);

            return Content(historicoHumidade, "application/json");
        }
        
    }
}
