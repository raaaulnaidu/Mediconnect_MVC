using System.Text.Json;

namespace MediConnect.Web.Models
{
    public class NpiVizVM
    {
        public List<string> StateLabels { get; set; } = new();
        public List<int> StateValues { get; set; } = new();
        public List<string> SpecLabels { get; set; } = new();
        public List<int> SpecValues { get; set; } = new();
        public List<string> GenderLabels { get; set; } = new();
        public List<int> GenderValues { get; set; } = new();
        public string ToJson(object o) => JsonSerializer.Serialize(o);
    }
}
