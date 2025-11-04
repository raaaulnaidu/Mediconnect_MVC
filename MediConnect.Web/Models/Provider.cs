namespace MediConnect.Web.Models
{
    public class Provider : BaseEntity
    {
        public string? FullName { get; set; }
        public string? Specialty { get; set; }
        public long? NpiNumber { get; set; }

        // 🔧 Add these to satisfy ProvidersController & views
        public string? State { get; set; }
        public string? City  { get; set; }

        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
}
