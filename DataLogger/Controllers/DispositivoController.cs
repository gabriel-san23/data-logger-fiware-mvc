using System;
using DataLogger.DAO;
using DataLogger.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DataLogger.Controllers
{
    public class DispositivoController : PadraoController<DispositivoViewModel>
    {
        public DispositivoController()
        {
            DAO = new DispositivoDAO();
            GeraProximoId = false;
        }

        protected override void PreencheDadosParaView(string Operacao, DispositivoViewModel model)
        {
            if (Operacao == "I")
            {
                int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
                if (idUsuario.HasValue)
                    model.IdUsuario = idUsuario.Value;
            }
        }

        protected override void ValidaDados(DispositivoViewModel model, string operacao)
        {            
            ModelState.Clear();

            if (string.IsNullOrEmpty(model.Descricao))
                ModelState.AddModelError("Descricao", "Informe a descrição do dispositivo.");

            if (model.IdUsuario <= 0)
                ModelState.AddModelError("IdUsuario", "Usuário inválido.");
        }

        public override IActionResult Index()
        {
            try
            {
                int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
                if (!idUsuario.HasValue)
                    return RedirectToAction("Index", "Login");

                var dao = (DispositivoDAO)DAO;
                var lista = dao.ListagemPorUsuario(idUsuario.Value);
                return View(NomeViewIndex, lista);
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public override IActionResult Save(DispositivoViewModel model, string Operacao)
        {
            try
            {                
                if (Operacao == "I")
                {
                    int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
                    if (idUsuario.HasValue)
                        model.IdUsuario = idUsuario.Value;
                }

                ValidaDados(model, Operacao);

                if (ModelState.IsValid == false)
                {
                    ViewBag.Operacao = Operacao;
                    PreencheDadosParaView(Operacao, model);
                    return View(NomeViewForm, model);
                }
                else
                {
                    if (Operacao == "I")
                        DAO.Insert(model);
                    else
                        DAO.Update(model);
                    return RedirectToAction(NomeViewIndex);
                }
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }
    }
}
