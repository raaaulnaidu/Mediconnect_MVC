namespace MediConnect.Web.ViewModels
{
    public class AppointmentListVM
    {
        public int Id { get; set; }
        public DateTime StartsAt { get; set; }
        public string? Reason { get; set; }
        public string Status { get; set; } = "Scheduled";
        public string PatientName { get; set; } = "";
        public string ProviderName { get; set; } = "";
    }
}
