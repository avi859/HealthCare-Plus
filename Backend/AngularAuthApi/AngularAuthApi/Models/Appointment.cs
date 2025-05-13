namespace AngularAuthApi.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public string? PatientName { get; set; }
        public string? DoctorPhone { get; set; }
        public DateTime AppointmentTime { get; set; }
        public string? Status { get; set; } 
    }

}
