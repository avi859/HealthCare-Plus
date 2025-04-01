using AngularAuthApi.Context;
using AngularAuthApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1.Crmf;

//[ApiController]
//[Route("api/chat")]
//public class AiChatController : ControllerBase
//{
//    private readonly HttpClient _httpClient;
//    private readonly string _groqApiKey = "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"; // 🔒 Keep this secret

//    public AiChatController()
//    {
//        _httpClient = new HttpClient();
//    }

//[HttpPost("ask")]
//public async Task<IActionResult> AskGroq([FromBody] ChatRequest request)
//{
//    if (string.IsNullOrWhiteSpace(request.Message))
//        return BadRequest("Message cannot be empty");

//    var groqRequest = new
//    {
//        model = "llama-3.3-70b-versatile",
//        messages = new[] { new { role = "user", content = request.Message } }
//    };

//    var requestContent = new StringContent(JsonSerializer.Serialize(groqRequest), Encoding.UTF8, "application/json");
//    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _groqApiKey);

//    var response = await _httpClient.PostAsync("https://api.groq.com/openai/v1/chat/completions", requestContent);

//    if (!response.IsSuccessStatusCode)
//        return StatusCode((int)response.StatusCode, "Error connecting to AI");

//    var responseContent = await response.Content.ReadAsStringAsync();
//    return Ok(responseContent);
//}

[Route("api/chat")]
[ApiController]
public class AiChatController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly string _groqApiKey = "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";

    public AiChatController(AppDbContext context, HttpClient httpClient)
    {
        _context = context;
        _httpClient = httpClient;
    }

    [HttpPost("ask")]
    public async Task<IActionResult> AskGroq([FromBody] ChatRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
            return BadRequest("Message cannot be empty");

        var groqRequest = new
        {
            model = "llama-3.3-70b-versatile",
            messages = new[] { new { role = "user", content = request.Message } }
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(groqRequest), Encoding.UTF8, "application/json");
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _groqApiKey);

        var response = await _httpClient.PostAsync("https://api.groq.com/openai/v1/chat/completions", requestContent);

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode, "Error connecting to AI");

        var responseContent = await response.Content.ReadAsStringAsync();
        return Ok(responseContent);
    }

    [HttpPost("analyze-symptoms")]
    public async Task<IActionResult> AnalyzeSymptoms([FromBody] SymptomRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Symptoms))
            return BadRequest("Symptoms cannot be empty");

        var groqRequest = new
        {
            model = "llama-3.3-70b-versatile",
            messages = new[]
            {
                new { role = "system", content = "Analyze symptoms and return severity (1-10) and specialty (e.g., Cardiology, Neurology)." },
                new { role = "user", content = request.Symptoms }
            }
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(groqRequest), Encoding.UTF8, "application/json");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _groqApiKey);

        var response = await _httpClient.PostAsync("https://api.groq.com/openai/v1/chat/completions", requestContent);
        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode, "Error processing symptoms");

        var responseContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine("Groq API Response: " + responseContent);
        var analysisResult = JsonSerializer.Deserialize<AnalysisResponse>(responseContent);
        if (analysisResult == null)
            return BadRequest("Invalid AI response");

        var doctor = await _context.Doctors
            .Where(d => d.Specialty == analysisResult.Specialty)
            .OrderBy(d => Guid.NewGuid()) // Random doctor selection
            .FirstOrDefaultAsync();

        if (doctor != null)
        {
            analysisResult.RecommendedDoctor = doctor.Name;
            analysisResult.DoctorDetails = new DoctorDto
            {
                Name = doctor.Name,
                Specialty = doctor.Specialty,
                Experience = doctor.Experience,
                ImageUrl = doctor.ImageUrl,
                PhoneNumber = doctor.PhoneNumber
            };
        }

        // Auto-book appointment if severity is high
        if (analysisResult.Severity >= 7 && doctor != null)
        {
            var appointment = new Appointment
            {
                PatientName = request.PatientName,
                Doctor = doctor.Name,
                Date = DateTime.UtcNow.AddHours(2) // Auto-booking in 2 hours
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // Send confirmation via WhatsApp (Temporary implementation)
            await SendWhatsAppMessage(appointment.PatientName, $"Your appointment with {doctor.Name} is confirmed for {appointment.Date}.");
        }

        return Ok(analysisResult);
    }

    private async Task SendWhatsAppMessage(string phoneNumber, string message)
    {
        // Placeholder function for WhatsApp integration
        Console.WriteLine($"WhatsApp Message to {phoneNumber}: {message}");
        await Task.CompletedTask;
    }
}

public class ChatRequest
{
    public string? Message { get; set; }
}