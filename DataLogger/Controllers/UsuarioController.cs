using DataLogger.DAO;
using DataLogger.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace DataLogger.Controllers
{
    public class UsuarioController : PadraoController<UsuarioViewModel>
    {
        public UsuarioController()
        {
            DAO = new UsuarioDAO();
            GeraProximoId = true;
        }

        protected override void PreencheDadosParaView(string Operacao, UsuarioViewModel model)
        {
            if (GeraProximoId && Operacao == "I")
            {
                model.Id = DAO.ProximoId();
                model.TipoUsuario = "padrao";
            }

        }

        protected override void ValidaDados(UsuarioViewModel model, string operacao)
        {
            base.ValidaDados(model, operacao);
            if (string.IsNullOrEmpty(model.NomeUsuario))
                ModelState.AddModelError("NomeUsuario", "Preencha o nome de usuário.");

            if (string.IsNullOrEmpty(model.SenhaUsuario))
                ModelState.AddModelError("SenhaUsuario", "Preencha a senha.");

            //Imagem será obrigatória apenas na inclusão.
            //Na alteração iremos considerar a que já estava salva.
            if (model.FotoPerfil == null && operacao == "I")
                ModelState.AddModelError("FotoPerfil", "Escolha uma foto de perfil.");
            if (model.FotoPerfil != null && model.FotoPerfil.Length / 1024 / 1024 >= 2)
                ModelState.AddModelError("FotoPerfil", "Imagem limitada a 2 mb.");
            if (ModelState.IsValid)
            {
                //na alteração, se não foi informada a imagem, iremos manter a que já estava salva.
                if (operacao == "A" && model.FotoPerfil == null)
                {
                    UsuarioViewModel u = DAO.Consulta(model.Id);
                    model.FotoPerfilEmByte = u.FotoPerfilEmByte;
                }
                else
                {
                    model.FotoPerfilEmByte = ConvertImageToByte(model.FotoPerfil);
                }
            }
        }

        public byte[] ConvertImageToByte(IFormFile file)
        {
            if (file != null)
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    return ms.ToArray();
                }
            else
                return null;
        }

        // Precisamos fazer diferentes updates dependendo do que o usuario alterar

        /*
        public override IActionResult Save(UsuarioViewModel model, string Operacao)
        {
            try
            {
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

        public override IActionResult Edit(int id)
        {
            try
            {
                ViewBag.Operacao = "A";
                var model = DAO.Consulta(id);
                if (model == null)
                    return RedirectToAction(NomeViewIndex);
                else
                {
                    PreencheDadosParaView("A", model);
                    return View(NomeViewForm, model);
                }
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }
        */
    }
}
