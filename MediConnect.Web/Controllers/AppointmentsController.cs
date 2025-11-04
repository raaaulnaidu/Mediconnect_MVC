using Microsoft.AspNetCore.Mvc;
using MediConnect.Web.Models;
using MediConnect.Web.Services;

namespace MediConnect.Web.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly InMemoryRepository<Appointment> _appts;
        private readonly InMemoryRepository<Patient> _patients;
        private readonly InMemoryRepository<Provider> _providers;
        private readonly JsonBackedStore _store;

        public AppointmentsController(
            InMemoryRepository<Appointment> appts,
            InMemoryRepository<Patient> patients,
            InMemoryRepository<Provider> providers,
            JsonBackedStore store)
        {
            _appts = appts;
            _patients = patients;
            _providers = providers;
            _store = store;
        }

        public IActionResult Index()
        {
            var rows = _appts.GetAll()
                .OrderByDescending(a => a.StartsAt)
                .ToList();
            return View(rows);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.PatientOptions = _patients.GetAll()
                .OrderBy(p => p.FullName)
                .Select(p => new { p.Id, Label = $"{p.Id} — {p.FullName}" })
                .ToList();

            ViewBag.ProviderOptions = _providers.GetAll()
                .OrderBy(p => p.FullName)
                .Select(p => new { p.Id, Label = $"{p.Id} — {p.FullName}" })
                .ToList();

            var model = new Appointment
            {
                StartsAt = DateTime.Now.AddMinutes(10),
                EndsAt = DateTime.Now.AddMinutes(40),
                Status = "Scheduled"
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Appointment model)
        {
            // rebind dropdowns in case of validation errors
            ViewBag.PatientOptions = _patients.GetAll()
                .OrderBy(p => p.FullName)
                .Select(p => new { p.Id, Label = $"{p.Id} — {p.FullName}" })
                .ToList();
            ViewBag.ProviderOptions = _providers.GetAll()
                .OrderBy(p => p.FullName)
                .Select(p => new { p.Id, Label = $"{p.Id} — {p.FullName}" })
                .ToList();

            if (model.PatientId == 0)
                ModelState.AddModelError(nameof(model.PatientId), "Please choose a patient.");
            if (model.ProviderId == 0)
                ModelState.AddModelError(nameof(model.ProviderId), "Please choose a provider.");
            if (model.EndsAt != default && model.EndsAt < model.StartsAt)
                ModelState.AddModelError(nameof(model.EndsAt), "End cannot be before start.");

            if (!ModelState.IsValid)
                return View(model);

            _appts.Add(model);
            _store.Save();
            TempData["Ok"] = "Visit saved.";
            return RedirectToAction("Index", "Home");
        }
    }
}
