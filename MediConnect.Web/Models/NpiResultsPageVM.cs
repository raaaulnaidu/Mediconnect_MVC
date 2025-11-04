using System.Collections.Generic;

namespace MediConnect.Web.Models
{
    public class NpiResultsPageVM
    {
        // Echoed query (for links/navigation)
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Organization { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? Taxonomy { get; set; }

        // Table rows
        public List<NpiTableRow> Rows { get; set; } = new();

        // Charts – by State
        public List<string> StateLabels { get; set; } = new();
        public List<int>    StateValues { get; set; } = new();

        // Charts – by Specialty
        public List<string> SpecLabels  { get; set; } = new();
        public List<int>    SpecValues  { get; set; } = new();

        // Charts – by City
        public List<string> CityLabels  { get; set; } = new();
        public List<int>    CityValues  { get; set; } = new();
    }

  
}
