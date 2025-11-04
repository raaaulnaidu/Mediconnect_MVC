using Microsoft.AspNetCore.Mvc;
using MediConnect.Web.Models;
using MediConnect.Web.Services;
using System.Linq;

namespace MediConnect.Web.Controllers
{
    public class ExternalController : Controller
    {
        private readonly NpiClient _npi;

        public ExternalController(NpiClient npi)
        {
            _npi = npi;
        }

        // GET: /External/NpiSearch
        [HttpGet]
        public IActionResult NpiSearch() => View(new NpiSearchQuery());

        // GET: /External/NpiResults  — table + inline charts
        [HttpGet]
        public async Task<IActionResult> NpiResults([FromQuery] NpiSearchQuery query)
        {
            if (IsEmptyQuery(query))
            {
                ModelState.AddModelError("", "Enter at least one filter (name, organization, state, city, or taxonomy).");
                return View("NpiSearch", query);
            }

            var api = await _npi.SearchAsync(query, limit: 150);
            var vm = BuildVmFromApi(api, query);
            return View(vm);
        }

        // GET: /External/NpiVisualize — full-page charts
        [HttpGet]
        public async Task<IActionResult> NpiVisualize([FromQuery] NpiSearchQuery query)
        {
            if (IsEmptyQuery(query))
            {
                ModelState.AddModelError("", "Enter at least one filter (name, organization, state, city, or taxonomy).");
                return View("NpiSearch", query);
            }

            var api = await _npi.SearchAsync(query, limit: 200);
            var vm = BuildVmFromApi(api, query);
            return View(vm);
        }

        // ───────────────────────────────────────────────────────────
        // Helpers
        // ───────────────────────────────────────────────────────────
        private static bool IsEmptyQuery(NpiSearchQuery q)
        {
            return string.IsNullOrWhiteSpace(q.FirstName)
                && string.IsNullOrWhiteSpace(q.LastName)
                && string.IsNullOrWhiteSpace(q.Organization)
                && string.IsNullOrWhiteSpace(q.State)
                && string.IsNullOrWhiteSpace(q.City)
                && string.IsNullOrWhiteSpace(q.Taxonomy);
        }

        private static NpiResultsPageVM BuildVmFromApi(NpiApiResponse api, NpiSearchQuery query)
        {
            var results = api?.Results ?? new List<NpiRecord>();

            // Map rows for the table
            var rows = results.Select(r =>
            {
                var tax = r.Taxonomies?.FirstOrDefault(t => t.Primary == true)?.Desc
                          ?? r.Taxonomies?.FirstOrDefault()?.Desc
                          ?? "—";

                var addr = r.Addresses?.FirstOrDefault(a => a.AddressPurpose == "LOCATION")
                           ?? r.Addresses?.FirstOrDefault();

                var name = r.Basic?.OrganizationName;
                if (string.IsNullOrWhiteSpace(name))
                {
                    var first = r.Basic?.FirstName ?? "";
                    var last  = r.Basic?.LastName  ?? "";
                    name = string.Join(" ", new[] { first, last }.Where(s => !string.IsNullOrWhiteSpace(s)));
                }

                return new NpiTableRow
                {
                    Npi = r.Number,
                    Name = name,
                    PrimaryTaxonomy = tax,
                    City = addr?.City,
                    State = addr?.State,
                    Phone = addr?.TelephoneNumber
                };
            }).ToList();

            // Aggregations for charts
            var byState = results
                .Select(r => r.Addresses?.FirstOrDefault(a => a.AddressPurpose == "LOCATION") ?? r.Addresses?.FirstOrDefault())
                .Where(a => a != null && !string.IsNullOrWhiteSpace(a!.State))
                .GroupBy(a => a!.State!)
                .OrderByDescending(g => g.Count())
                .Take(12)
                .ToList();

            var bySpec = results
                .Select(r => r.Taxonomies?.FirstOrDefault(t => t.Primary == true)?.Desc
                             ?? r.Taxonomies?.FirstOrDefault()?.Desc
                             ?? "Unknown")
                .GroupBy(s => s)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .ToList();

            var byCity = results
                .Select(r => r.Addresses?.FirstOrDefault(a => a.AddressPurpose == "LOCATION") ?? r.Addresses?.FirstOrDefault())
                .Where(a => a != null && !string.IsNullOrWhiteSpace(a!.City))
                .GroupBy(a => a!.City!)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .ToList();

            return new NpiResultsPageVM
            {
                // Echo query
                FirstName    = query.FirstName,
                LastName     = query.LastName,
                Organization = query.Organization,
                State        = query.State,
                City         = query.City,
                Taxonomy     = query.Taxonomy,

                // Table
                Rows = rows,

                // Charts
                StateLabels = byState.Select(g => g.Key).ToList(),
                StateValues = byState.Select(g => g.Count()).ToList(),

                SpecLabels  = bySpec.Select(g => g.Key).ToList(),
                SpecValues  = bySpec.Select(g => g.Count()).ToList(),

                CityLabels  = byCity.Select(g => g.Key).ToList(),
                CityValues  = byCity.Select(g => g.Count()).ToList()
            };
        }
    }
}
