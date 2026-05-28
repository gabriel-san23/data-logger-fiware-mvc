using System.Collections.Generic;
using System.Data;
using DataLogger.DAO;
using Microsoft.AspNetCore.Mvc;

namespace DataLogger.Controllers
{
    public class ApiController : Controller
    {
        public IActionResult usuariosDispositivos(string nomeUsuario = null)
        {
            var dao = new ApiDAO();
            DataTable tabela = dao.ListaUsuariosDispositivos(nomeUsuario);

            var lista = new List<object>();
            foreach (DataRow row in tabela.Rows)
            {
                lista.Add(new
                {
                    nomeUsuario = row["nomeUsuario"].ToString(),
                    idDispositivo = row["idDispositivo"].ToString(),
                    descricao = row["descricao"].ToString()
                });
            }

            return Json(lista);
        }
    }
}
