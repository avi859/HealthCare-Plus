namespace AngularAuthApi.Models
{
   public class SymptomRequest
    {
        public string? Symptoms { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string? PatientName { get; set; }
    }
}
