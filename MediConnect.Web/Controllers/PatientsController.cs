using Microsoft.AspNetCore.Mvc;
using MediConnect.Web.Models;
using MediConnect.Web.Services;

namespace MediConnect.Web.Controllers
{
    public class PatientsController : Controller
    {
        private readonly InMemoryRepository<Patient> _patients;
        private readonly JsonBackedStore _store;

        public PatientsController(InMemoryRepository<Patient> patients, JsonBackedStore store)
        {
            _patients = patients;
            _store = store;
        }

        public IActionResult Index()
        {
            var list = _patients.GetAll().OrderBy(p => p.FullName).ToList();
            return View(list);
        }

        public IActionResult Details(int id)
        {
            var patient = _patients.Find(id);
            if (patient == null) return NotFound();
            return View(patient);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Patient());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Patient model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _patients.Add(model);
            _store.Save();
            TempData["Ok"] = "Patient added successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var patient = _patients.Find(id);
            if (patient == null) return NotFound();
            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Patient model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existing = _patients.Find(id);
            if (existing == null) return NotFound();

            existing.FullName = model.FullName;
            existing.DateOfBirth = model.DateOfBirth;
            existing.Email = model.Email;
            existing.Phone = model.Phone;
            existing.Gender = model.Gender;

            _patients.Update(id, existing);
            _store.Save();
            TempData["Ok"] = "Patient updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var patient = _patients.Find(id);
            if (patient == null) return NotFound();
            return View(patient);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _patients.Delete(id);
            _store.Save();
            TempData["Ok"] = "Patient deleted!";
            return RedirectToAction(nameof(Index));
        }
    }
}
