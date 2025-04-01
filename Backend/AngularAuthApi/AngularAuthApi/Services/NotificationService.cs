using System.Threading.Tasks;
using AngularAuthApi.Models;

public class NotificationService
{
    public async Task SendAppointmentConfirmation(Appointment appointment)
    {
        string message = $"Your appointment with {appointment.Doctor} is confirmed on {appointment.Date}.";
        await SendWhatsAppMessage(appointment.PatientName, message);
    }

    private async Task SendWhatsAppMessage(string phoneNumber, string message)
    {
        // Twilio or Gupshup API logic here
    }
}
