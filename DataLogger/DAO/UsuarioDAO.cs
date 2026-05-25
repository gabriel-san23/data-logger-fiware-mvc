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

        public UsuarioViewModel ConsultaPorNome(string nomeUsuario)
        {
            var p = new SqlParameter[]
            {
                new SqlParameter("NOME_USUARIO", nomeUsuario)
            };
            var tabela = HelperDAO.ExecutaProcSelect("spConsultaPorNome", p);
            if (tabela.Rows.Count == 0)
                return null;
            else
                return MontaModel(tabela.Rows[0]);
        }

        // recebe uma Model (C#)
        // retorna os parâmetros usados na query (SQL)
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

        // recebe registro do banco de dados (SQL)
        // retorna uma Model (C#)
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
    }
}
