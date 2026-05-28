using DataLogger.DAO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataLogger.Controllers
{
    public class DashboardController : Controller
    {
        private readonly FiwareServices _fiwareServices;

        private static System.Collections.Generic.Dictionary<string, bool> _subscricoesFeitas
            = new System.Collections.Generic.Dictionary<string, bool>();

        public IActionResult Index()
        {
            ViewBag.Logado = HelperControllers.VerificaUserLogado(HttpContext.Session);
            ViewBag.TipoUsuario = HttpContext.Session.GetString("TipoUsuario");
            return View();
        }

        //Controles Admin

        public IActionResult Admin()
        {
            if (HttpContext.Session.GetString("TipoUsuario") != "admin")
                return RedirectToAction("Index");
            ViewBag.Logado = true;
            ViewBag.TipoUsuario = "admin";
            return View();
        }

        public async Task<IActionResult> AdminAction(string serverIp, string acao)
        {
            if (HttpContext.Session.GetString("TipoUsuario") != "admin")
                return Content(JsonSerializer.Serialize(new { sucesso = false, dados = "Acesso negado" }), "application/json");

            string resultado = acao switch
            {
                "healthOrion" => await _fiwareServices.HealthCheckOrion(serverIp),
                "healthIOT" => await _fiwareServices.HealthCheckIOTAgent(serverIp),
                "healthServices" => await _fiwareServices.HealthCheckServices(serverIp),
                "healthSTH" => await _fiwareServices.HealthCheckSTHComet(serverIp),
                "listarDispositivos" => await _fiwareServices.ListarDispositivos(serverIp),
                "provendoGrupoMQTT" => await _fiwareServices.ProvendoGrupoMQTT(serverIp),
                "deletarGrupo" => await _fiwareServices.DeletarGrupoSevicos(serverIp),
                _ => JsonSerializer.Serialize(new { sucesso = false, dados = "Ação desconhecida" })
            };

            return Content(resultado, "application/json");
        }


        public async Task<IActionResult> historicoLuminosidade(string serverIp, int idDispositivo, int lastN = 30)
        {
            try
            {
                var dispositivoDAO = new DispositivoDAO();
                var dispositivo = dispositivoDAO.Consulta(idDispositivo);

                if (dispositivo == null)
                    return Content(JsonSerializer.Serialize(new { sucesso = false, dados = "Dispositivo não encontrado" }), "application/json");

                await GaranteSubscricao(serverIp, dispositivo.FiwareEntityName);

                var historico = await _fiwareServices.RequestLuminosity(serverIp, dispositivo.FiwareEntityName, lastN);

                SalvarDadosDoFiwareNoBanco(historico, idDispositivo, "luminosity");

                return Content(historico, "application/json");
            }
            catch (Exception ex)
            {
                return Content(JsonSerializer.Serialize(new { sucesso = false, dados = ex.Message }), "application/json");
            }

        }

        public async Task<IActionResult> historicoTemperatura(string serverIp, int idDispositivo, int lastN = 30)
        {
            try
            {
                var dispositivoDAO = new DispositivoDAO();
                var dispositivo = dispositivoDAO.Consulta(idDispositivo);

                if (dispositivo == null)
                    return Content(JsonSerializer.Serialize(new { sucesso = false, dados = "Dispositivo não encontrado" }), "application/json");

                await GaranteSubscricao(serverIp, dispositivo.FiwareEntityName);

                var historico = await _fiwareServices.RequestTemperature(serverIp, dispositivo.FiwareEntityName, lastN);

                SalvarDadosDoFiwareNoBanco(historico, idDispositivo, "temperature");

                return Content(historico, "application/json");
            }
            catch (Exception ex)
            {
                return Content(JsonSerializer.Serialize(new { sucesso = false, dados = ex.Message }), "application/json");
            }
        }

        public async Task<IActionResult> historicoHumidade(string serverIp, int idDispositivo, int lastN = 30)
        {

            try
            {
                var dispositivoDAO = new DispositivoDAO();
                var dispositivo = dispositivoDAO.Consulta(idDispositivo);

                if (dispositivo == null)
                    return Content(JsonSerializer.Serialize(new { sucesso = false, dados = "Dispositivo não encontrado" }), "application/json");

                await GaranteSubscricao(serverIp, dispositivo.FiwareEntityName);

                var historico = await _fiwareServices.RequestHumidity(serverIp, dispositivo.FiwareEntityName, lastN);

                SalvarDadosDoFiwareNoBanco(historico, idDispositivo, "humidity");

                return Content(historico, "application/json");
            }
            catch (Exception ex)
            {
                return Content(JsonSerializer.Serialize(new { sucesso = false, dados = ex.Message }), "application/json");
            }
        }

        private async Task GaranteSubscricao(string serverIp, string entityName)
        {
            //string chave = $"{serverIp}_{entityName}";
            //if (!_subscricoesFeitas.ContainsKey(chave) || !_subscricoesFeitas[chave])
            //{
            await _fiwareServices.SubscribeParameters(serverIp, entityName);
            //    _subscricoesFeitas[chave] = true;
            //}
        }

        private void SalvarDadosDoFiwareNoBanco(string jsonFiware, int idDispositivo, string tipoAtributo)
        {
            try
            {
                using var doc = JsonDocument.Parse(jsonFiware);

                var root = doc.RootElement;

                if (root.TryGetProperty("sucesso", out var sucesso) && !sucesso.GetBoolean())
                    return; // Fiware retornou erro, não salva nada

                if (!root.TryGetProperty("dados", out var dadosElement))
                    return;

                string dadosJson = dadosElement.GetString();
                if (string.IsNullOrEmpty(dadosJson))
                    return;

                using var dadosDoc = JsonDocument.Parse(dadosJson);
                var dadosRoot = dadosDoc.RootElement;

                var contextResponses = dadosRoot.GetProperty("contextResponses");
                var primeiroResponse = contextResponses[0];
                var contextElement = primeiroResponse.GetProperty("contextElement");
                var attributes = contextElement.GetProperty("attributes");
                var primeiroAttr = attributes[0];
                var valores = primeiroAttr.GetProperty("values");

                if (valores.GetArrayLength() == 0)
                    return;

                var ultimoValor = valores[valores.GetArrayLength() - 1];
                string attrValue = ultimoValor.GetProperty("attrValue").GetString();

                var registroDAO = new RegistroDAO();

                int umidade = 0, luminosidade = 0;
                decimal temperatura = 0;

                if (tipoAtributo == "humidity" && int.TryParse(attrValue, out int u))
                    umidade = u;
                else if (tipoAtributo == "luminosity" && int.TryParse(attrValue, out int l))
                    luminosidade = l;
                else if (tipoAtributo == "temperature" && decimal.TryParse(attrValue,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out decimal t))
                    temperatura = t;

                registroDAO.SalvarRegistro(idDispositivo, umidade, luminosidade, temperatura);
            }
            catch (Exception ex)
            {
                // Loga o erro mas não interrompe a resposta para o front-end
                //_logger.LogError($"Erro ao salvar dados no banco: {ex.Message}");
            }
        }

    }
}
