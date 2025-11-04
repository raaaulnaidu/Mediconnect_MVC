namespace MediConnect.Web.Models
{
    /// <summary>
    /// Query parameters for NPI lookups, bound from the search form / query string.
    /// </summary>
    public class NpiSearchQuery
    {
        public string? FirstName { get; set; }
        public string? LastName  { get; set; }
        public string? Organization { get; set; }   // Organization/Facility name
        public string? State { get; set; }          // 2-letter
        public string? City  { get; set; }
        public string? Taxonomy { get; set; }       // taxonomy description text (e.g., "nurse", "cardio")
        public string? Npi { get; set; }            // optional direct NPI filter
    }
}
