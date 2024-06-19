namespace IIS.Dashboard.Models
{
    public class AppPool
    {
        public string Name { get; set; }
        public string State { get; set; }
    }
    public class AppPoolEdit
    {
        public string Name { get; set; }
        public bool Enable32BitAppOnWin64 { get; set; }
        public string Mode { get; set; }
        public string RuntimeVersion { get; set; }
        public bool IsEdit { get; set; }
    }
}
