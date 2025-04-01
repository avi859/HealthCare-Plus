namespace AngularAuthApi.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public string? PatientName { get; set; }
        public string? Doctor { get; set; }
        public DateTime Date { get; set; }
    }
}
