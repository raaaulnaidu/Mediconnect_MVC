using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using MediConnect.Web.Models;

namespace MediConnect.Web.Services
{
    /// <summary>
    /// Minimal client for NPPES NPI Registry v2.1 returning typed models.
    /// </summary>
    public class NpiClient
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNameCaseInsensitive = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public NpiClient(HttpClient http)
        {
            _http = http;
            _http.BaseAddress ??= new Uri("https://npiregistry.cms.hhs.gov/");
            _http.Timeout = TimeSpan.FromSeconds(20);
        }

        /// <summary>
        /// Typed search. Builds query string from NpiSearchQuery and parses into NpiApiResponse.
        /// </summary>
        public async Task<NpiApiResponse> SearchAsync(NpiSearchQuery query, int limit = 100)
        {
            var qs = new Dictionary<string, string?>
            {
                ["version"] = "2.1",
                ["first_name"]          = Maybe(query.FirstName),
                ["last_name"]           = Maybe(query.LastName),
                ["organization_name"]   = Maybe(query.Organization),
                ["state"]               = Maybe(query.State),
                ["city"]                = Maybe(query.City),
                ["taxonomy_description"]= Maybe(query.Taxonomy),
                ["limit"]               = Math.Clamp(limit, 1, 200).ToString()
            };

            var url = new StringBuilder("api/?");
            bool first = true;
            foreach (var kv in qs)
            {
                if (string.IsNullOrWhiteSpace(kv.Value)) continue;
                if (!first) url.Append('&'); first = false;
                url.Append(Uri.EscapeDataString(kv.Key))
                   .Append('=')
                   .Append(Uri.EscapeDataString(kv.Value!));
            }
            if (first) url.Append("version=2.1&limit=50");

            using var resp = await _http.GetAsync(url.ToString(), HttpCompletionOption.ResponseHeadersRead);
            resp.EnsureSuccessStatusCode();

            await using var stream = await resp.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);

            var outResp = new NpiApiResponse();

            if (!doc.RootElement.TryGetProperty("results", out var results) ||
                results.ValueKind != JsonValueKind.Array)
                return outResp;

            foreach (var item in results.EnumerateArray())
            {
                var rec = new NpiRecord
                {
                    Number = GetString(item, "number"),
                    EnumerationType = GetString(item, "enumeration_type")
                };

                // basic
                if (item.TryGetProperty("basic", out var basic) && basic.ValueKind == JsonValueKind.Object)
                {
                    rec.Basic = new NpiBasic
                    {
                        FirstName        = GetString(basic, "first_name"),
                        LastName         = GetString(basic, "last_name"),
                        OrganizationName = GetString(basic, "organization_name"),
                        Gender           = GetString(basic, "gender")
                    };
                }

                // addresses
                if (item.TryGetProperty("addresses", out var addrArr) && addrArr.ValueKind == JsonValueKind.Array)
                {
                    rec.Addresses = new List<NpiAddress>();
                    foreach (var a in addrArr.EnumerateArray())
                    {
                        rec.Addresses.Add(new NpiAddress
                        {
                            AddressPurpose  = GetString(a, "address_purpose"),
                            City            = GetString(a, "city"),
                            State           = GetString(a, "state"),
                            TelephoneNumber = GetString(a, "telephone_number"),
                            Phone           = GetString(a, "phone")
                        });
                    }
                }

                // taxonomies
                if (item.TryGetProperty("taxonomies", out var taxArr) && taxArr.ValueKind == JsonValueKind.Array)
                {
                    rec.Taxonomies = new List<NpiTaxonomy>();
                    foreach (var t in taxArr.EnumerateArray())
                    {
                        rec.Taxonomies.Add(new NpiTaxonomy
                        {
                            Primary = GetBool(t, "primary"),
                            Desc    = GetString(t, "desc") ?? GetString(t, "taxonomy_group")
                        });
                    }
                }

                outResp.Results.Add(rec);
            }

            return outResp;
        }

        // ---- helpers
        private static string? Maybe(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

        private static string? GetString(JsonElement obj, string prop)
            => obj.TryGetProperty(prop, out var v)
               ? v.ValueKind switch
               {
                   JsonValueKind.String => v.GetString(),
                   JsonValueKind.Number => v.TryGetInt64(out var n) ? n.ToString() : v.ToString(),
                   JsonValueKind.True   => "true",
                   JsonValueKind.False  => "false",
                   _ => null
               }
               : null;

        private static bool? GetBool(JsonElement obj, string prop)
        {
            if (!obj.TryGetProperty(prop, out var v)) return null;
            return v.ValueKind switch
            {
                JsonValueKind.True  => true,
                JsonValueKind.False => false,
                JsonValueKind.String => bool.TryParse(v.GetString(), out var b) ? b : null,
                _ => null
            };
        }
    }
}
