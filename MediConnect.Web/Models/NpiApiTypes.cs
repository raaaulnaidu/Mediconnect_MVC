using System.Collections.Generic;

namespace MediConnect.Web.Models
{
    public class NpiApiResponse
    {
        public List<NpiRecord> Results { get; set; } = new();
    }

    public class NpiRecord
    {
        public string? Number { get; set; }

        public string? EnumerationType { get; set; }  
        public NpiBasic? Basic { get; set; }
        public List<NpiAddress>? Addresses { get; set; }
        public List<NpiTaxonomy>? Taxonomies { get; set; }
    }

    public class NpiBasic
    {
        public string? FirstName { get; set; }
        public string? LastName  { get; set; }
        public string? OrganizationName { get; set; }
        public string? Gender { get; set; }  // "M" / "F" (or null)
    }

    public class NpiAddress
    {
        public string? AddressPurpose { get; set; } // e.g., "LOCATION" or "MAILING"
        public string? City { get; set; }
        public string? State { get; set; }
        public string? TelephoneNumber { get; set; }
        public string? Phone { get; set; }          // some payloads use "phone"
    }

    public class NpiTaxonomy
    {
        public bool? Primary { get; set; }
        public string? Desc { get; set; }           // description (specialty)
    }
}
