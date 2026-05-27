using System.Data;
using System.Data.SqlClient;

namespace DataLogger.DAO
{
    public class ApiDAO
    {
        public DataTable ListaUsuariosDispositivos(string nomeUsuario = null)
        {
            string sql = @"SELECT u.nomeUsuario, d.id AS idDispositivo, d.descricao
                           FROM tbUsuarios u
                           INNER JOIN tbDispositivos d ON d.idUsuario = u.id";

            if (!string.IsNullOrEmpty(nomeUsuario))
            {
                sql += " WHERE u.nomeUsuario = @nomeUsuario";
                var parametros = new SqlParameter[] { new SqlParameter("nomeUsuario", nomeUsuario) };
                return HelperDAO.ExecutaSqlSelect(sql, parametros);
            }

            return HelperDAO.ExecutaSqlSelect(sql, null);
        }
    }
}
