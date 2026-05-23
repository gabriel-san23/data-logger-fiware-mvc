using DataLogger.Models;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DataLogger.DAO
{
    public class UsuarioDAO : PadraoDAO<UsuarioViewModel>
    {
        protected override void SetTabela()
        {
            Tabela = "tbUsuarios";
            ChaveIdentity = true;
        }
        protected override SqlParameter[] CriaParametros(UsuarioViewModel model)
        {
            object imgByte = model.FotoPerfilEmByte;
            if (imgByte == null)
                imgByte = DBNull.Value;
            SqlParameter[] parametros =
            {
                 new SqlParameter("id", model.Id),
                 new SqlParameter("nome_Usuario", model.NomeUsuario),
                 new SqlParameter("senha_Usuario", model.SenhaUsuario),
                 new SqlParameter("tipo_Usuario", model.TipoUsuario),
                 new SqlParameter("foto_Perfil", imgByte)
                 };
            return parametros;
        }

        protected override UsuarioViewModel MontaModel(DataRow registro)
        {
            var u = new UsuarioViewModel()
            {
                Id = Convert.ToInt32(registro["id"]),
                NomeUsuario = registro["nome_Usuario"].ToString(),
                SenhaUsuario = registro["senha_Usuario"].ToString(),
                TipoUsuario = registro["tipo_Usuario"].ToString()
            };

            if (registro["foto_Perfil"] != DBNull.Value)
                u.FotoPerfilEmByte = registro["foto_Perfil"] as byte[];
            return u;
        }
    }
}
