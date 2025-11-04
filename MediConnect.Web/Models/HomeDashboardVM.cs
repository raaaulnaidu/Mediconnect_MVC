namespace MediConnect.Web.Models
{
    public class HomeRecentVM
    {
        public DateTime When { get; set; }
        public string Type { get; set; } = "";
        public string Summary { get; set; } = "";
    }

    public class HomeDashboardVM
    {
        public int PatientCount { get; set; }
        public int VisitCount { get; set; }
        public int OccupancyPct { get; set; }
        public List<HomeRecentVM> Recent { get; set; } = new();
    }
}
