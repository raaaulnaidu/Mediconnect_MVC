// Models/Patient.cs
namespace MediConnect.Web.Models
{
    public class Patient : BaseEntity
    {
        public string? FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }     // optional, used by UI/seed
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
}
