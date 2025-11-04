// Models/Appointment.cs
namespace MediConnect.Web.Models
{
    public class Appointment : BaseEntity
    {
        public int PatientId { get; set; }
        public int ProviderId { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime? EndsAt { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; }   // e.g., Scheduled/Completed/Cancelled
    }
}
