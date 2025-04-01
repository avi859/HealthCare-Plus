namespace AngularAuthApi.Models
{
    public class AnalysisResponse
    {
        public int Severity { get; set; }
        public string? Specialty { get; set; }
        public string? RecommendedDoctor { get; set; }
        public DoctorDto? DoctorDetails { get; set; }
    }
}
