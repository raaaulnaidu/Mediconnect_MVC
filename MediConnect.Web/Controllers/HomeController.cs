using MediConnect.Web.Models;
using MediConnect.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace MediConnect.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly InMemoryRepository<Patient> _patients;
        private readonly InMemoryRepository<Appointment> _appts;

        public HomeController(InMemoryRepository<Patient> patients,
                              InMemoryRepository<Appointment> appts)
        {
            _patients = patients;
            _appts = appts;
        }

        public IActionResult Index()
        {
            var pCount = _patients.GetAll().Count();
            var vCount = _appts.GetAll().Count();

            var model = new HomeDashboardVM
            {
                PatientCount = pCount,
                VisitCount = vCount,
                OccupancyPct = Math.Min(100, vCount * 10),
                Recent = _appts.GetAll()
                    .OrderByDescending(a => a.StartsAt)
                    .Take(7)
                    .Select(a => new HomeRecentVM
                    {
                        When = a.StartsAt,
                        Type = "Visit",
                        Summary = $"#{a.Id} — Patient {a.PatientId} • {(string.IsNullOrWhiteSpace(a.Reason) ? "General" : a.Reason)}"
                    })
                    .ToList()
            };

            return View(model);
        }

        // Quick Search JSON endpoint the dashboard calls
        [HttpGet]
        public IActionResult SearchPatients(string q)
        {
            q ??= "";
            var list = _patients.GetAll()
                .Where(p =>
                    string.IsNullOrWhiteSpace(q) ||
                    (p.FullName ?? "").Contains(q, StringComparison.OrdinalIgnoreCase) ||
                    p.Id.ToString().Equals(q, StringComparison.OrdinalIgnoreCase))
                .OrderBy(p => p.FullName)
                .Take(10)
                .Select(p => new
                {
                    id = p.Id,
                    name = p.FullName ?? "—",
                    dob = p.DateOfBirth.HasValue ? p.DateOfBirth.Value.ToString("yyyy-MM-dd") : "—",
                    gender = string.IsNullOrWhiteSpace(p.Gender) ? "—" : p.Gender
                })
                .ToList();

            return Json(list);
        }
    }
}
