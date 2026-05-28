using Microsoft.AspNetCore.Http;

namespace DataLogger.Controllers
{
    public class HelperControllers
    {
        public static bool VerificaUserLogado(ISession session)
        {
            string logado = session.GetString("Logado");
            if (logado == null)
                return false;
            else
                return true;
        }
    } // Classe auxiliar para verificar se o usuário está logado.
}
