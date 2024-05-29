namespace IIS.Dashboard.Models
{
    public class Website
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public int Port { get; set; }
    }
}
