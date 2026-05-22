namespace DataLogger.Models
{
    public class FiwareViewModel
    {
        public string serverIp { get; set; }
        public int lastN { get; set; } = 30;
    }
}
