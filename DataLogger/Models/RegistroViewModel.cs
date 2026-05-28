using System;

namespace DataLogger.Models
{
    public class RegistroViewModel : PadraoViewModel
    {
        public int IdDispositivo { get; set; }

        public string DescricaoDispositivo { get; set; }

        public int ValorUmidade { get; set; }
        public int ValorLuminosidade { get; set; }
        public decimal ValorTemperatura { get; set; }

        public DateTime DataHora { get; set; }
    }
}
