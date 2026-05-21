using DataLogger.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DataLogger.DAO
{
    public abstract class PadraoDAO<T> : PadraoViewModel
    {
        public PadraoDAO() 
        {
            SetTabela();
        }
        protected string Tabela { get; set; }
        protected string NomeSpListagem { get; set; } = "spListagem";
        protected abstract SqlParameter[] CriaParametros(T model);
        protected abstract T MontaModel(DataRow registro);
        protected abstract void SetTabela();

    }
}
