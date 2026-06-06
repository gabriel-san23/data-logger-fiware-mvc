using System.Collections.Generic;

namespace DataLogger.Models
{
    public class LoteRegistrosViewModel
    {
        public List<int> Luminosidades { get; set; }
        public List<decimal> Temperaturas { get; set; }
        public List<int> Umidades { get; set; }
    }
}
