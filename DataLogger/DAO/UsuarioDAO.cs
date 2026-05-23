using DataLogger.Models;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DataLogger.DAO
{
    public class UsuarioDAO : PadraoDAO<UsuarioViewModel>
    {
        protected override SqlParameter[] CriaParametros(UsuarioViewModel model)
        {
            object imgByte = model.FotoPerfilEmByte;
            if (imgByte == null)
                imgByte = DBNull.Value;
            SqlParameter[] parametros =
            {
                 new SqlParameter("id", model.Id),
                 new SqlParameter("nomeUsuario", model.NomeUsuario),
                 new SqlParameter("senhaUsuario", model.SenhaUsuario),
                 new SqlParameter("tipoUsuario", model.TipoUsuario),
                 new SqlParameter("fotoPerfil", imgByte)
                 };
            return parametros;
        }

        protected override UsuarioViewModel MontaModel(DataRow registro)
        {
            var u = new UsuarioViewModel()
            {
                Id = Convert.ToInt32(registro["id"]),
                NomeUsuario = registro["nomeUsuario"].ToString(),
                SenhaUsuario = registro["senhaUsuario"].ToString(),
                TipoUsuario = registro["tipoUsuario"].ToString()
            };

            if (registro["fotoPerfil"] != DBNull.Value)
                u.FotoPerfilEmByte = registro["fotoPerfil"] as byte[];
            return u;
        }

        protected override void SetTabela()
        {
            Tabela = "tbUsuarios";
        }
    }
}
