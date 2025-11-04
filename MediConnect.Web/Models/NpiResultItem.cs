namespace MediConnect.Web.Models
{
    /// <summary>
    /// Represents a simplified provider record fetched from the NPPES NPI Registry API.
    /// Used in both NpiResults view and NpiVisualize charts.
    /// </summary>
    public class NpiResultItem
    {
        // Unique NPI number assigned to the provider
        public string? Npi { get; set; }

        // Display name of the provider (person or organization)
        public string? DisplayName { get; set; }

        // Primary taxonomy / specialty of the provider
        public string? PrimaryTaxonomy { get; set; }

        // City where the provider is registered
        public string? City { get; set; }

        // State where the provider practices
        public string? State { get; set; }

        // Contact phone number
        public string? Phone { get; set; }

        // Indicates whether the provider is an Individual or Organization
        public string? EntityType { get; set; }
    }
}
