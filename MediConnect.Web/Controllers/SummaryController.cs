using Microsoft.AspNetCore.Mvc;
using MediConnect.Web.Models;
using MediConnect.Web.Services;

namespace MediConnect.Web.Controllers
{
    public class SummaryController : Controller
    {
        private readonly InMemoryRepository<Patient> _patients;
        private readonly InMemoryRepository<Provider> _providers;
        private readonly InMemoryRepository<Appointment> _appointments;

        public SummaryController(
            InMemoryRepository<Patient> patients,
            InMemoryRepository<Provider> providers,
            InMemoryRepository<Appointment> appointments)
        {
            _patients = patients;
            _providers = providers;
            _appointments = appointments;
        }

        public IActionResult Index()
        {
            var totalPatients = _patients.GetAll().Count();
            var totalProviders = _providers.GetAll().Count();
            var totalAppointments = _appointments.GetAll().Count();

            var completedAppointments = _appointments.GetAll()
                .Count(a => a.Status?.Equals("Completed", StringComparison.OrdinalIgnoreCase) == true);
            var cancelledAppointments = _appointments.GetAll()
                .Count(a => a.Status?.Equals("Cancelled", StringComparison.OrdinalIgnoreCase) == true);
            var upcomingAppointments = _appointments.GetAll()
                .Count(a => a.StartsAt > DateTime.Now);

            var summary = new
            {
                TotalPatients = totalPatients,
                TotalProviders = totalProviders,
                TotalAppointments = totalAppointments,
                CompletedAppointments = completedAppointments,
                CancelledAppointments = cancelledAppointments,
                UpcomingAppointments = upcomingAppointments
            };

            return View(summary);
        }
    }
}
