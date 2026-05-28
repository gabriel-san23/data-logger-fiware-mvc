using DataLogger.DAO;
using DataLogger.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DataLogger.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult FazLogin(string usuario, string senha)
        {
            //Este é apenas um exemplo, aqui você deve consultar na sua tabela de usuários
            //se existe esse usuário e senha
            try
            {
                var dao = new UsuarioDAO();
                UsuarioViewModel model = dao.ConsultaPorNome(usuario);

                // caso o usuário não exista
                // a comparação não funciona
                if (model == null)
                {
                    ViewBag.Erro = "Erro: Usuário não cadastrado!";
                    return View("Index");

                }
                else if (senha != model.SenhaUsuario)
                {
                    ViewBag.Erro = "Senha incorreta!";
                    return View("Index");
                }
                else
                {
                    HttpContext.Session.SetString("Logado", "true");
                    HttpContext.Session.SetString("NomeUsuario", model.NomeUsuario);
                    HttpContext.Session.SetInt32("IdUsuario", model.Id);
                    HttpContext.Session.SetString("TipoUsuario", model.TipoUsuario);

                    return RedirectToAction("index", "home");
                }
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }            
        }
        public IActionResult LogOff()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
