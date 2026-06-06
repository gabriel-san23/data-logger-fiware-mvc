using System.Collections.Generic;
using System.Data;
using DataLogger.DAO;
using Microsoft.AspNetCore.Mvc;

namespace DataLogger.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiController : Controller
    {
        /// <summary>
        /// Retorna a lista de usuários e seus dispositivos cadastrados.
        /// </summary>
        /// <param name="nomeUsuario">Nome do usuário para filtrar. Deixe vazio para retornar todos.</param>
        /// <returns>Lista de objetos com nomeUsuario, idDispositivo e descricao</returns>
        [HttpGet("usuariosDispositivos")]
        [ProducesResponseType(200)]
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

            return Ok(lista);
        }
    }
}
