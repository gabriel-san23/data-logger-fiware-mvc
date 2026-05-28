using System.Data;
using System;
using System.Data.SqlClient;
using DataLogger.Models;
using System.Collections.Generic;

namespace DataLogger.DAO
{
    public class DispositivoDAO : PadraoDAO<DispositivoViewModel>
    {
        protected override void SetTabela()
        {
            Tabela = "tbDispositivos";
            ChaveIdentity = true;
        }

        protected override SqlParameter[] CriaParametros(DispositivoViewModel model)
        {
            return new SqlParameter[]
            {
                new SqlParameter("ID_DISPOSITIVO", model.Id),
                new SqlParameter("DESCRICAO", model.Descricao),
                new SqlParameter("ID_USUARIO", model.IdUsuario)
            };
        }

        protected override DispositivoViewModel MontaModel(DataRow registro)
        {
            var d = new DispositivoViewModel()
            {
                Id = Convert.ToInt32(registro["id"]),
                Descricao = registro["descricao"].ToString(),
                IdUsuario = Convert.ToInt32(registro["idUsuario"])
            };

            if (registro.Table.Columns.Contains("nomeUsuario"))
                d.NomeUsuario = registro["nomeUsuario"].ToString();

            return d;
        }

        public List<DispositivoViewModel> ListagemPorUsuario(int idUsuario)
        {
            string sql = @"SELECT d.id, d.descricao, d.idUsuario, u.nomeUsuario
                           FROM tbDispositivos d
                           INNER JOIN tbUsuarios u ON d.idUsuario = u.id
                           WHERE d.idUsuario = @idUsuario
                           ORDER BY d.id";

            var parametros = new SqlParameter[]
            {
                new SqlParameter("idUsuario", idUsuario)
            };

            var tabela = HelperDAO.ExecutaSqlSelect(sql, parametros);

            var lista = new List<DispositivoViewModel>();
            foreach (DataRow registro in tabela.Rows)
                lista.Add(MontaModel(registro));

            return lista;
        }
    }
}
