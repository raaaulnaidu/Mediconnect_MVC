using System.Text.Json;
using MediConnect.Web.Models;

namespace MediConnect.Web.Services
{
    /// <summary>
    /// Persists in-memory repos to a single JSON file (singleton-style persistence).
    /// </summary>
    public class JsonBackedStore
    {
        private readonly string _path;
        private readonly InMemoryRepository<Patient> _patients;
        private readonly InMemoryRepository<Provider> _providers;
        private readonly InMemoryRepository<Appointment> _appts;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true
        };

        public JsonBackedStore(
            InMemoryRepository<Patient> patients,
            InMemoryRepository<Provider> providers,
            InMemoryRepository<Appointment> appts,
            IWebHostEnvironment env)
        {
            _patients = patients;
            _providers = providers;
            _appts = appts;

            var dataDir = Path.Combine(env.ContentRootPath, "App_Data");
            Directory.CreateDirectory(dataDir);
            _path = Path.Combine(dataDir, "AppData.json");

            // Try load on startup
            if (File.Exists(_path))
            {
                try { Load(); } catch { /* ignore corrupt file */ }
            }
        }

        public void Save()
        {
            var payload = new SnapshotContainer
            {
                Patients = _patients.Snapshot(),
                Providers = _providers.Snapshot(),
                Appointments = _appts.Snapshot()
            };

            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            File.WriteAllText(_path, json);
        }

        public void Load()
        {
            if (!File.Exists(_path)) return;

            var json = File.ReadAllText(_path);
            var payload = JsonSerializer.Deserialize<SnapshotContainer>(json, _jsonOptions);
            if (payload is null) return;

            _patients.Load(payload.Patients ?? new List<Patient>());
            _providers.Load(payload.Providers ?? new List<Provider>());
            _appts.Load(payload.Appointments ?? new List<Appointment>());
        }

        private sealed class SnapshotContainer
        {
            public List<Patient>? Patients { get; set; }
            public List<Provider>? Providers { get; set; }
            public List<Appointment>? Appointments { get; set; }
        }
    }
}
