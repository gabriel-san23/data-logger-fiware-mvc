using System.Net.Http;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

namespace DataLogger.DAO
{
    public class FiwareServices
    {
        private HttpClient CriaClienteFiware()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("fiware-service", "smart");
            client.DefaultRequestHeaders.Add("fiware-servicepath", "/");

            client.Timeout = TimeSpan.FromSeconds(10);
            return client;

        }

        //IOT AGENT - HEALTH CHECK
        public async Task<string> HealthCheckIOTAgent(string serverIp)
        {
            try
            {
                using (var client = CriaClienteFiware())
                {
                    string url = $"http://{serverIp}:4041/iot/about";
                    var response = await client.GetAsync(url);
                    string corpo = await response.Content.ReadAsStringAsync();

                    return JsonSerializer.Serialize(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { sucesso = false, dados = ex.Message });
            }
        }

        //IOT AGENT - PROVENDO UM GRUPO DE SERVIÇOS PARA O MQTT
        public async Task<string> ProvendoGrupoMQTT(string serverIp)
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

                    return JsonSerializer.Serialize(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { sucesso = false, dados = ex.Message });
            }
        }

        //IOT AGENT - HEALTH CHECK SERVICES
        public async Task<string> HealthCheckServices(string serverIp)
        {
            try
            {
                using (var client = CriaClienteFiware())
                {
                    string serverIp_correto = serverIp.Trim();
                    string url = $"http://{serverIp}:4041/iot/services";
                    var response = await client.GetAsync(url);
                    string corpo = await response.Content.ReadAsStringAsync();

                    return JsonSerializer.Serialize(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { sucesso = false, dados = ex.Message });
            }
        }

        //IOT AGENT - DELETAR UM GRUPO DE SERVIÇOS
        public async Task<string> DeletarGrupoSevicos(string serverIp)
        {
            try
            {
                using (var client = CriaClienteFiware())
                {
                    string serverIp_correto = serverIp.Trim();
                    string url = $"http://{serverIp}:4041/iot/services/?resource=&apikey=TEF";
                    var response = await client.DeleteAsync(url);
                    string corpo = await response.Content.ReadAsStringAsync();

                    return JsonSerializer.Serialize(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { sucesso = false, dados = ex.Message });
            }
        }


        //IOT AGENT - PROVENDO UM DATA LOGGER
        public async Task<string> ProverDataLogger(string serverIp, string deviceId, string entityName)
        {
            try
            {
                using (var client = CriaClienteFiware())
                {
                    string serverIp_correto = serverIp.Trim();
                    string url = $"http://{serverIp}:4041/iot/devices";

                    var body = new
                    {
                        devices = new[]
                        {
                            new
                            {
                                device_id = deviceId, 
                                entity_name = entityName,
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

                    return JsonSerializer.Serialize(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { sucesso = false, dados = ex.Message });
            }
        }

        //IOT AGENT - LISTAR DISPOSITIVOS
        public async Task<string> ListarDispositivos(string serverIp)
        {
            try
            {
                using (var client = CriaClienteFiware())
                {
                    string serverIp_correto = serverIp.Trim();
                    string url = $"http://{serverIp}:4041/iot/devices";
                    var response = await client.GetAsync(url);
                    string corpo = await response.Content.ReadAsStringAsync();

                    return JsonSerializer.Serialize(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { sucesso = false, dados = ex.Message });
            }
        }

        //IOT AGENT - RESULT OF DATA LOGGER LUMINOSITY
        public async Task<string> ResultadoLuminosidadeDataLogger(string serverIp, string entityName)
        {
            try
            {
                string serverIp_correto = serverIp.Trim();
                using (var client = CriaClienteFiware())
                {
                    string url = $"http://{serverIp}:1026/v2/entities/{entityName}/attrs/luminosity";
                    var response = await client.GetAsync(url);
                    string corpo = await response.Content.ReadAsStringAsync();

                    return JsonSerializer.Serialize(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { sucesso = false, dados = ex.Message });
            }
        }

        //IOT AGENT - RESULT OF DATA LOGGER TEMPERATURE
        public async Task<string> ResultadoTemperaturaDataLogger(string serverIp, string entityName)
        {
            try
            {
                using (var client = CriaClienteFiware())
                {
                    string serverIp_correto = serverIp.Trim();
                    string url = $"http://{serverIp}:1026/v2/entities/{entityName}/attrs/temperature";
                    var response = await client.GetAsync(url);
                    string corpo = await response.Content.ReadAsStringAsync();

                    return JsonSerializer.Serialize(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { sucesso = false, dados = ex.Message });
            }
        }

        //IOT AGENT - RESULT OF DATA LOGGER HUMIDITY
        public async Task<string> ResultadoUmidadeDataLogger(string serverIp, string entityName)
        {
            try
            {
                using (var client = CriaClienteFiware())
                {
                    string serverIp_correto = serverIp.Trim();
                    string url = $"http://{serverIp}:1026/v2/entities/{entityName}/attrs/humidity";
                    var response = await client.GetAsync(url);
                    string corpo = await response.Content.ReadAsStringAsync();

                    return JsonSerializer.Serialize(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { sucesso = false, dados = ex.Message });
            }
        }

        //IOT AGENT - DELETE DATA LOGGER IN IOT AGENT
        public async Task<string> DeletarDispositivoIOT(string serverIp, string deviceId)
        {
            try
            {
                using (var client = CriaClienteFiware())
                {
                    string serverIp_correto = serverIp.Trim();
                    string url = $"http://{serverIp}:4041/iot/devices/{deviceId}";
                    var response = await client.DeleteAsync(url);
                    string corpo = await response.Content.ReadAsStringAsync();

                    return JsonSerializer.Serialize(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { sucesso = false, dados = ex.Message });
            }
        }

        //STH COMET - HEALTH CHECK
        public async Task<string> HealthCheckSTHComet(string serverIp)
        {
            try
            {
                using (var client = CriaClienteFiware())
                {
                    string serverIp_correto = serverIp.Trim();
                    string url = $"http://{serverIp}:8666/version";
                    var response = await client.GetAsync(url);
                    string corpo = await response.Content.ReadAsStringAsync();

                    return JsonSerializer.Serialize(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { sucesso = false, dados = ex.Message });
            }
        }

        public async Task<string> SubscribeParameters(string serverIp, string entityName)
        {
            try
            {
                using (var client = CriaClienteFiware())
                {
                    string serverIp_correto = serverIp.Trim();
                    string url = $"http://{serverIp}:1026/v2/subscriptions";

                    var body = new
                    {
                        description = "Notifica o STH Comet de todas as mudanças de parâmetros",
                        subject = new
                        {
                            entities = new[]
                            {
                                new { id = entityName, type = "DataLogger" }
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

                    return JsonSerializer.Serialize(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { sucesso = false, dados = ex.Message });
            }
        }

        public async Task<string> RequestLuminosity(string serverIp, string entityName, int lastN = 30)
        {
            try
            {
                using (var client = CriaClienteFiware())
                {
                    string serverIp_correto = serverIp.Trim();
                    string url = $"http://{serverIp}:8666/STH/v1/contextEntities/type/DataLogger/id/{entityName}/attributes/luminosity?lastN={lastN}";

                    var response = await client.GetAsync(url);
                    string corpo = await response.Content.ReadAsStringAsync();

                    return JsonSerializer.Serialize(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { sucesso = false, dados = ex.Message });
            }
        }

        public async Task<string> RequestTemperature(string serverIp, string entityName, int lastN = 30)
        {
            try
            {
                using (var client = CriaClienteFiware())
                {
                    string serverIp_correto = serverIp.Trim();
                    string url = $"http://{serverIp}:8666/STH/v1/contextEntities/type/DataLogger/id/{entityName}/attributes/temperature?lastN={lastN}";

                    var response = await client.GetAsync(url);
                    string corpo = await response.Content.ReadAsStringAsync();

                    return JsonSerializer.Serialize(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { sucesso = false, dados = ex.Message });
            }
        }

        public async Task<string> RequestHumidity(string serverIp, string entityName, int lastN = 30)
        {
            try
            {
                string serverIp_correto = serverIp.Trim();
                using (var client = CriaClienteFiware())
                {
                    string url = $"http://{serverIp}:8666/STH/v1/contextEntities/type/DataLogger/id/{entityName}/attributes/humidity?lastN={lastN}";

                    var response = await client.GetAsync(url);
                    string corpo = await response.Content.ReadAsStringAsync();

                    return JsonSerializer.Serialize(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { sucesso = false, dados = ex.Message });
            }
        }

        //ORION - PEGAR VERSÃO
        public async Task<string> HealthCheckOrion(string serverIp)
        {
            try
            {
                using (var client = CriaClienteFiware())
                {
                    string serverIp_correto = serverIp.Trim();
                    string url = $"http://{serverIp}:1026/version";
                    var response = await client.GetAsync(url);
                    string corpo = await response.Content.ReadAsStringAsync();

                    return JsonSerializer.Serialize(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
            }
            catch (Exception erro)
            {
                return JsonSerializer.Serialize(new { sucesso = false, dados = erro.Message });
            }
        }
    }
}
