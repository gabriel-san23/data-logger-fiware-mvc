using System.Data.SqlClient;

namespace DataLogger.DAO
{
    public class ConexaoBD
    {
        public static SqlConnection GetConexao()
        {
            //string strCon = "Data Source=LOCALHOST; Database=AULADB; user id=sa; password=123456";
            string strCon = "Data Source=DESKTOP-F8NSTBA\\SQLDEVELOPER; Database=AULADB; Integrated Security=True; TrustServerCertificate=True";
            SqlConnection conexao = new SqlConnection(strCon);
            conexao.Open();
            return conexao;
        }
    }
}