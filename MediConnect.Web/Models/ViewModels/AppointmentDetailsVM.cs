using MediConnect.Web.Models;

namespace MediConnect.Web.ViewModels
{
    public class AppointmentDetailsVM
    {
        public Appointment Appointment { get; set; } = default!;
        public Patient? Patient { get; set; }
        public Provider? Provider { get; set; }
    }
}
