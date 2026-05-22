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
using System.Text.Json;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Xml.Linq;

namespace DataLogger.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        //TESTE
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

        private HttpClient CriaClienteFiware()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("fiware-service", "smart");
            client.DefaultRequestHeaders.Add("fiware-servicepath", "/");
            client.Timeout = TimeSpan.FromSeconds(10);
            return client;

        }

        //IOT AGENT - HEALTH CHECK
        public async Task<IActionResult> HealthCheckIOTAgent(string serverIp)
        {
            try
            {
                using (var client = CriaClienteFiware())
                {
                    string url = $"http://{serverIp}:4041/iot/about";
                    var response = await client.GetAsync(url);
                    string corpo = await response.Content.ReadAsStringAsync();

                    return Json(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, dados = ex.Message });
            }
        }

        //IOT AGENT - PROVENDO UM GRUPO DE SERVIÇOS PARA O MQTT
        public async Task<IActionResult> ProvendoGrupoMQTT(string serverIp)
        {
            try
            {
                using (var client = CriaClienteFiware())
                {
                    string url = $"http://{serverIp}:4041/iot/services";

                    var body = new
                    {   
                        services = new[]
                        {
                            new
                            {
                                apikey = "TEF",
                                cbroker = $"http://{serverIp}:1026",
                                entity_type = "Thing",
                                resource = ""
                            }                            
                        }
                    };

                    string json = JsonSerializer.Serialize(body);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, content);
                    string corpo = await response.Content.ReadAsStringAsync();

                    return Json(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
            }
            catch (Exception ex)
            {                
                return Json(new { sucesso = false, dados = ex.Message });
            }
        }

        //IOT AGENT - HEALTH CHECK SERVICES
        public async Task<IActionResult> HealthCheckServices(string serverIp)
        {
            try
            {
                using (var client = CriaClienteFiware())
                {
                    string url = $"http://{serverIp}:4041/iot/services";
                    var response = await client.GetAsync(url);
                    string corpo = await response.Content.ReadAsStringAsync();

                    return Json(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, dados = ex.Message });
            }
        }

        //IOT AGENT - DELETAR UM GRUPO DE SERVIÇOS
        public async Task<IActionResult> DeletarGrupoSevicos(string serverIp)
        {
            try
            {
                using (var client = CriaClienteFiware())
                {
                    string url = $"http://{serverIp}:4041/iot/services/?resource=&apikey=TEF";
                    var response = await client.DeleteAsync(url);
                    string corpo = await response.Content.ReadAsStringAsync();

                    return Json(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, dados = ex.Message });
            }
        }

        //IOT AGENT - PROVENDO UM DATA LOGGER
        public async Task<IActionResult> ProverDataLogger(string serverIp)
        {
            try
            {
                using (var client = CriaClienteFiware())
                {
                    string url = $"http://{serverIp}:4041/iot/devices";

                    var body = new
                    {
                        devices = new[]
                        {
                            new
                            {
                                device_id = "datalogger001", //id variável
                                entity_name = "urn:ngsi-ld:DataLogger:001", //id variável
                                entity_type = "DataLogger",
                                protocol = "PDI-IoTA-UltraLight",
                                transport = "MQTT",
                                attributes = new[]
                                {
                                    new { object_id = "l", name = "luminosity", type = "Integer" },
                                    new { object_id = "t", name = "temperature", type = "Float" },
                                    new { object_id = "h", name = "humidity", type = "Integer" }
                                }
                            }
                        }
                    };

                    string json = JsonSerializer.Serialize(body);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, content);
                    string corpo = await response.Content.ReadAsStringAsync();

                    return Json(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, dados = ex.Message });
            }
        }

        //IOT AGENT - LISTAR DISPOSITIVOS
        public async Task<IActionResult> ListarDispositivos(string serverIp)
        {
            try
            {
                using (var client = CriaClienteFiware())
                {
                    string url = $"http://{serverIp}:4041/iot/devices";
                    var response = await client.GetAsync(url);
                    string corpo = await response.Content.ReadAsStringAsync();

                    return Json(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, dados = ex.Message });
            }
        }

        //IOT AGENT - RESULT OF DATA LOGGER LUMINOSITY
        public async Task<IActionResult> ResultadoLuminosidadeDataLogger(string serverIp)
        {
            try
            {
                using (var client = CriaClienteFiware())
                {
                    string url = $"http://{serverIp}:1026/v2/entities/urn:ngsi-ld:DataLogger:001/attrs/luminosity";
                    var response = await client.GetAsync(url);
                    string corpo = await response.Content.ReadAsStringAsync();

                    return Json(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, dados = ex.Message });
            }
        }

        //IOT AGENT - RESULT OF DATA LOGGER TEMPERATURE
        public async Task<IActionResult> ResultadoTemperaturaDataLogger(string serverIp)
        {
            try
            {
                using (var client = CriaClienteFiware())
                {
                    string url = $"http://{serverIp}:1026/v2/entities/urn:ngsi-ld:DataLogger:001/attrs/temperature";
                    var response = await client.GetAsync(url);
                    string corpo = await response.Content.ReadAsStringAsync();

                    return Json(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, dados = ex.Message });
            }
        }

        //IOT AGENT - RESULT OF DATA LOGGER HUMIDITY
        public async Task<IActionResult> ResultadoHumidadeDataLogger(string serverIp)
        {
            try
            {
                using (var client = CriaClienteFiware())
                {
                    string url = $"http://{serverIp}:1026/v2/entities/urn:ngsi-ld:DataLogger:001/attrs/humidity";
                    var response = await client.GetAsync(url);
                    string corpo = await response.Content.ReadAsStringAsync();

                    return Json(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, dados = ex.Message });
            }
        }

        //IOT AGENT - DELETE DATA LOGGER IN IOT AGENT
        public async Task<IActionResult> DeletarDispositivoIOT(string serverIp)
        {
            try
            {
                using (var client = CriaClienteFiware())
                {
                    string url = $"http://{serverIp}:4041/iot/devices/datalogger001";
                    var response = await client.DeleteAsync(url);
                    string corpo = await response.Content.ReadAsStringAsync();

                    return Json(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, dados = ex.Message });
            }
        }

        //STH COMET - HEALTH CHECK
        public async Task<IActionResult> HealthCheckSTHComet(string serverIp)
        {
            try
            {
                using (var client = CriaClienteFiware())
                {
                    string url = $"http://{serverIp}:8666/version";
                    var response = await client.GetAsync(url);
                    string corpo = await response.Content.ReadAsStringAsync();

                    return Json(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, dados = ex.Message });
            }
        }

        // STH COMET - RECEBE DADOS
        private async Task<IActionResult> SubscribeParameters(string serverIp)
        {
            try
            {
                using (var client = CriaClienteFiware())
                {
                    string url = $"http://{serverIp}:1026/v2/subscriptions";

                    var body = new
                    {
                        description = "Notifica o STH Comet de todas as mudanças de parâmetros",
                        subject = new
                        {
                            entities = new[]
                            {
                                new { id = "urn:ngsi-ld:DataLogger:001", type = "DataLogger" }
                            },
                            condition = new { attrs = new[] { "luminosity", "temperature", "humidity" } }
                        },
                        notification = new
                        {
                            http = new { url = $"http://{serverIp}:8666/notify" },
                            attrs = new[] { "luminosity", "temperature", "humidity" },
                            attrFormat = "legacy"
                        }
                    };

                    string json = JsonSerializer.Serialize(body);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, content);
                    string corpo = await response.Content.ReadAsStringAsync();

                    return Json(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, dados = ex.Message });
            }
        }

        private async Task<IActionResult> RequestLuminosity(string serverIp, int lastN = 30)
        {
            try
            {
                using (var client = CriaClienteFiware())
                {
                    string url = $"http://{serverIp}:8666/STH/v1/contextEntities/type/DataLogger/id/urn:ngsi-ld:DataLogger:001/attributes/luminosity?lastN={lastN}";

                    var response = await client.GetAsync(url);
                    string corpo = await response.Content.ReadAsStringAsync();

                    return Json(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, dados = ex.Message });
            }
        }

        private async Task<IActionResult> RequestTemperature(string serverIp, int lastN = 30)
        {
            try
            {
                using (var client = CriaClienteFiware())
                {
                    string url = $"http://{serverIp}:8666/STH/v1/contextEntities/type/DataLogger/id/urn:ngsi-ld:DataLogger:001/attributes/temperature?lastN={lastN}";

                    var response = await client.GetAsync(url);
                    string corpo = await response.Content.ReadAsStringAsync();

                    return Json(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, dados = ex.Message });
            }
        }

        private async Task<IActionResult> RequestHumidity(string serverIp, int lastN = 30)
        {
            try
            {
                using (var client = CriaClienteFiware())
                {
                    string url = $"http://{serverIp}:8666/STH/v1/contextEntities/type/DataLogger/id/urn:ngsi-ld:DataLogger:001/attributes/humidity?lastN={lastN}";

                    var response = await client.GetAsync(url);
                    string corpo = await response.Content.ReadAsStringAsync();

                    return Json(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, dados = ex.Message });
            }
        }

        //ORION - PEGAR VERSÃO
        public async Task<IActionResult> HealthCheckOrion(string serverIp)
        {
            try
            {
                using (var client = CriaClienteFiware())
                {
                    string url = $"http://{serverIp}:1026/version";
                    var response = await client.GetAsync(url);
                    string corpo = await response.Content.ReadAsStringAsync();

                    return Json(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
            }
            catch (Exception erro)
            {
                return Json(new { sucesso = false, dados = erro.Message });
            }
        }
    }
}
