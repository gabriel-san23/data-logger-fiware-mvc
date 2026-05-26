using System.Data;
using System;
using System.Data.SqlClient;
using DataLogger.Models;
using System.Collections.Generic;

namespace DataLogger.DAO
{
    public class RegistroDAO : PadraoDAO<RegistroViewModel>
    {
        protected override void SetTabela()
        {
            Tabela = "tbRegistros";
            ChaveIdentity = true;
        }

        protected override SqlParameter[] CriaParametros(RegistroViewModel model)
        {
            return new SqlParameter[]
            {
                new SqlParameter("idDispositivo", model.IdDispositivo),
                new SqlParameter("valorUmidade", model.ValorUmidade),
                new SqlParameter("valorLuminosidade", model.ValorLuminosidade),
                new SqlParameter("valorTemperatura", model.ValorTemperatura)
            };
        }

        protected override RegistroViewModel MontaModel(DataRow registro)
        {
            var r = new RegistroViewModel()
            {
                Id = Convert.ToInt32(registro["id"]),
                IdDispositivo = Convert.ToInt32(registro["idDispositivo"]),
                ValorUmidade = Convert.ToInt32(registro["valorUmidade"]),
                ValorLuminosidade = Convert.ToInt32(registro["valorLuminosidade"]),
                ValorTemperatura = Convert.ToDecimal(registro["valorTemperatura"]),
                DataHora = Convert.ToDateTime(registro["dataHora"])
            };

            if (registro.Table.Columns.Contains("descricao"))
                r.DescricaoDispositivo = registro["descricao"].ToString();

            return r;
        }

        public void SalvarRegistro(int idDispositivo, int umidade, int luminosidade, decimal temperatura)
        {
            var model = new RegistroViewModel()
            {
                IdDispositivo = idDispositivo,
                ValorUmidade = umidade,
                ValorLuminosidade = luminosidade,
                ValorTemperatura = temperatura
            };
            Insert(model);
        }

        public List<RegistroViewModel> ListagemPorDispositivo(int idDispositivo, int lastN = 30)
        {
            string sql = @"SELECT TOP (@lastN) r.id, r.idDispositivo, r.valorUmidade,
                                  r.valorLuminosidade, r.valorTemperatura, r.dataHora,
                                  d.descricao
                           FROM tbRegistros r
                           INNER JOIN tbDispositivos d ON r.idDispositivo = d.id
                           WHERE r.idDispositivo = @idDispositivo
                           ORDER BY r.dataHora DESC";

            var parametros = new SqlParameter[]
            {
                new SqlParameter("lastN", lastN),
                new SqlParameter("idDispositivo", idDispositivo)
            };

            var tabela = HelperDAO.ExecutaSqlSelect(sql, parametros);
            var lista = new List<RegistroViewModel>();
            foreach (DataRow registro in tabela.Rows)
                lista.Add(MontaModel(registro));

            return lista;
        }
    }
}
