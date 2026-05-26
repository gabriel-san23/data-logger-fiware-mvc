namespace DataLogger.Models
{
    public class DispositivoViewModel : PadraoViewModel
    {
        public string Descricao { get; set; }

        public int IdUsuario { get; set; } //FK - iD do usuario

        public string NomeUsuario { get; set; }

        public string FiwareDeviceId => $"datalogger{Id:D3}";

        public string FiwareEntityName => $"urn:ngsi-ld:DataLogger:{Id:D3}";
    }
}
