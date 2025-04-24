using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using AngularAuthApi.Models;

namespace AngularAuthApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SymptomMatchController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public SymptomMatchController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> MatchSymptoms([FromBody] SymptomRequest request)
        {
            // Step 1: Get specialist from Groq
            var groqResponse = await GetSpecialistFromGroq(request.Symptoms!);

            // Step 2: Get nearby doctors from Google Places
            var doctors = await GetNearbyDoctors(request.Latitude, request.Longitude, groqResponse.Specialist);

            return Ok(new
            {
                specialist = groqResponse.Specialist,
                reason = groqResponse.Reason,
                nearbyDoctors = doctors
            });
        }

        private async Task<(string Specialist, string Reason)> GetSpecialistFromGroq(string symptoms)
        {
            var prompt = $"Patient reports: {symptoms}. Based on clinical guidelines, which specialist should they see and why?";

            var body = new
            {
                messages = new[] {
            new { role = "user", content = prompt }
        },
                model = "llama-3.3-70b-versatile",
                temperature = 0.7
            };

            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _configuration["GroqApiKey"]);

            var response = await _httpClient.PostAsync("https://api.groq.com/openai/v1/chat/completions", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonDocument.Parse(json);

            var fullText = result.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
            Console.WriteLine("==== RAW AI RESPONSE ====");
            Console.WriteLine(fullText);


            string specialist = "General Physician";
            string reason = "Reasoning not found.";

            if (!string.IsNullOrWhiteSpace(fullText))
            {
                var lines = fullText.Split('\n');

                foreach (var line in lines)
                {
                    if (line.ToLower().Contains("specialist"))
                        specialist = line.Split(':').Length > 1 ? line.Split(':')[1].Trim() : specialist;

                    if (line.ToLower().Contains("reason"))
                        reason = line.Split(':').Length > 1 ? line.Split(':')[1].Trim() : reason;
                }
            }

            return (specialist, reason);
        }

        private async Task<object> GetNearbyDoctors(double lat, double lng, string keyword)
        {
            var apiKey = _configuration["GooglePlacesApiKey"];
            var url = $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={lat},{lng}&radius=8000&type=doctor&keyword={keyword}&key={apiKey}";

            // Log the request URL for debugging
            Console.WriteLine($"Google API Request: {url}");

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var parsed = JsonDocument.Parse(json);

            // Debug the response body to identify what’s returned
            Console.WriteLine($"Google API Response: {json}");

            var doctors = parsed.RootElement.GetProperty("results").EnumerateArray()
                .Select(d => new
                {
                    name = d.GetProperty("name").GetString(),
                    address = d.GetProperty("vicinity").GetString(),
                    rating = d.TryGetProperty("rating", out var ratingProp) ? ratingProp.GetDouble() : 0
                }).ToList();

            // Fallback if no doctors are found (search for "doctor" instead of the specialist)
            if (!doctors.Any())
            {
                url = $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={lat},{lng}&radius=8000&type=doctor&keyword=doctor&key={apiKey}";
                response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                json = await response.Content.ReadAsStringAsync();
                parsed = JsonDocument.Parse(json);

                doctors = parsed.RootElement.GetProperty("results").EnumerateArray()
                    .Select(d => new
                    {
                        name = d.GetProperty("name").GetString(),
                        address = d.GetProperty("vicinity").GetString(),
                        rating = d.TryGetProperty("rating", out var ratingProp) ? ratingProp.GetDouble() : 0
                    }).ToList();
            }

            return doctors;
        }

    }

}
