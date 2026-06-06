using DataLogger.DAO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using DataLogger.Models;

namespace DataLogger.Controllers
{
    public class DashboardController : Controller
    {
        private readonly FiwareServices _fiwareServices;

        public DashboardController(FiwareServices fiwareServices)
        {
            _fiwareServices = fiwareServices;
        }

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

                return Content(historico, "application/json");
            }
            catch (Exception ex)
            {
                return Content(JsonSerializer.Serialize(new { sucesso = false, dados = ex.Message }), "application/json");
            }
        }

        [HttpPost]
        public IActionResult salvarLote(int idDispositivo,
            [FromBody] LoteRegistrosViewModel lote)
        {
            try
            {
                var registroDAO = new RegistroDAO();
                registroDAO.SalvarLoteRegistros(
                    idDispositivo,
                    lote.Luminosidades,
                    lote.Temperaturas,
                    lote.Umidades
                );

                return Content(JsonSerializer.Serialize(new { sucesso = true }), "application/json");
            }
            catch (Exception ex)
            {
                return Content(JsonSerializer.Serialize(new { sucesso = false, mensagem = ex.Message }), "application/json");
            }
        }

        public IActionResult listaRegistros(int idDispositivo, int lastN = 30,
    string ordem = "desc", string filtroParametro = "todos")
        {
            try
            {
                var registroDAO = new RegistroDAO();
                var lista = registroDAO.ListagemComFiltro(idDispositivo, lastN, ordem, filtroParametro);

                var resultado = lista.Select(r => new
                {
                    id = r.Id,
                    dataHora = r.DataHora.ToString("dd/MM/yyyy HH:mm:ss"),
                    valorLuminosidade = r.ValorLuminosidade,
                    valorTemperatura = r.ValorTemperatura,
                    valorUmidade = r.ValorUmidade,
                    descricaoDispositivo = r.DescricaoDispositivo
                }).ToList();

                object informacoesAdicionais = null;
                if (lista.Count > 0)
                {
                    informacoesAdicionais = new
                    {
                        mediaLuminosidade = Math.Round(lista.Average(r => (double)r.ValorLuminosidade), 2),
                        maiorLuminosidade = lista.Max(r => r.ValorLuminosidade),
                        menorLuminosidade = lista.Min(r => r.ValorLuminosidade),

                        mediaTemperatura = Math.Round((double)lista.Average(r => r.ValorTemperatura), 2),
                        maiorTemperatura = lista.Max(r => r.ValorTemperatura),
                        menorTemperatura = lista.Min(r => r.ValorTemperatura),

                        mediaUmidade = Math.Round(lista.Average(r => (double)r.ValorUmidade), 2),
                        maiorUmidade = lista.Max(r => r.ValorUmidade),
                        menorUmidade = lista.Min(r => r.ValorUmidade)
                    };
                }

                return Content(JsonSerializer.Serialize(new
                {
                    sucesso = true,
                    dados = resultado,
                    informacoesAdicionais = informacoesAdicionais
                }), "application/json");
            }
            catch (Exception ex)
            {
                return Content(JsonSerializer.Serialize(new { sucesso = false, mensagem = ex.Message }), "application/json");
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
        /*
        private string SalvarDadosDoFiwareNoBanco(string jsonFiware, int idDispositivo, string tipoAtributo)
        {
            try
            {
                using var doc = JsonDocument.Parse(jsonFiware);
                var root = doc.RootElement;

                if (root.TryGetProperty("sucesso", out var sucesso) && !sucesso.GetBoolean())
                    return "Fiware retornou sucesso=false";

                if (!root.TryGetProperty("dados", out var dadosElement))
                    return "Propriedade 'dados' não encontrada no JSON";

                string dadosJson = dadosElement.GetString();
                if (string.IsNullOrEmpty(dadosJson))
                    return "dadosJson está vazio ou nulo";

                using var dadosDoc = JsonDocument.Parse(dadosJson);
                var dadosRoot = dadosDoc.RootElement;

                var valores = dadosRoot
                    .GetProperty("contextResponses")[0]
                    .GetProperty("contextElement")
                    .GetProperty("attributes")[0]
                    .GetProperty("values");

                if (valores.GetArrayLength() == 0)
                    return "Array de valores está vazio";

                var registroDAO = new RegistroDAO();
                int salvos = 0;

                foreach (var item in valores.EnumerateArray())
                {
                    var attrElement = item.GetProperty("attrValue");
                    string attrValue = attrElement.ValueKind == JsonValueKind.String
                        ? attrElement.GetString()
                        : attrElement.GetRawText();

                    int umidade = 0, luminosidade = 0;
                    decimal temperatura = 0;

                    if (tipoAtributo == "humidity")
                        int.TryParse(attrValue, out umidade);
                    else if (tipoAtributo == "luminosity")
                        int.TryParse(attrValue, out luminosidade);
                    else if (tipoAtributo == "temperature")
                        decimal.TryParse(attrValue,
                            System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture,
                            out temperatura);

                    registroDAO.SalvarRegistro(idDispositivo, umidade, luminosidade, temperatura);
                    salvos++;
                }

                return $"OK - {salvos} registros salvos";
            }
            catch (Exception ex)
            {
                return $"ERRO: {ex.Message}";
            }
        }
        */
    }
}
