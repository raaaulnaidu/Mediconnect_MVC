using Microsoft.AspNetCore.Mvc;
using MediConnect.Web.Models;
using MediConnect.Web.Services;

namespace MediConnect.Web.Controllers
{
    public class ProvidersController : Controller
    {
        private readonly InMemoryRepository<Provider> _providers;
        private readonly JsonBackedStore _store;

        public ProvidersController(InMemoryRepository<Provider> providers, JsonBackedStore store)
        {
            _providers = providers;
            _store = store;
        }

        // List
        public IActionResult Index()
        {
            var list = _providers.GetAll()
                .OrderBy(p => p.FullName)
                .ToList();
            return View(list);
        }

        // Details
        public IActionResult Details(int id)
        {
            var provider = _providers.Find(id);
            if (provider == null) return NotFound();
            return View(provider);
        }

        // CREATE (GET) — accepts route values from NPI “Add” button
        [HttpGet]
        public IActionResult Create(string? name, string? specialty, string? phone, string? city, string? state)
        {
            var m = new Provider
            {
                FullName = name,
                Specialty = specialty,
                Phone = phone,
                City = city,
                State = state
            };
            return View(m);
        }

        // CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Provider model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _providers.Add(model);
            _store.Save();

            TempData["Ok"] = "Provider added successfully!";
            return RedirectToAction(nameof(Index));
        }

        // EDIT (GET)
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var provider = _providers.Find(id);
            if (provider == null) return NotFound();
            return View(provider);
        }

        // EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Provider model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existing = _providers.Find(id);
            if (existing == null) return NotFound();

            existing.FullName  = model.FullName;
            existing.Specialty = model.Specialty;
            existing.Phone     = model.Phone;
            existing.Email     = model.Email;
            existing.City      = model.City;
            existing.State     = model.State;
            existing.NpiNumber = model.NpiNumber;

            _providers.Update(id, existing);
            _store.Save();

            TempData["Ok"] = "Provider updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // DELETE (GET)
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var provider = _providers.Find(id);
            if (provider == null) return NotFound();
            return View(provider);
        }

        // DELETE (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _providers.Delete(id);
            _store.Save();
            TempData["Ok"] = "Provider deleted!";
            return RedirectToAction(nameof(Index));
        }
    }
}
