namespace AngularAuthApi.Models
{
  public class Doctor
  {
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Specialty { get; set; }
    public string? Education { get; set; }
    public int Experience { get; set; }
    public string? ImageUrl { get; set; }
    public string? PhoneNumber { get; set; }

    public decimal ConsultationFee { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    }
}
