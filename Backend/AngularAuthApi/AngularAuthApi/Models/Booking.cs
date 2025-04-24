namespace AngularAuthApi.Models
{
    public class Booking
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Department { get; set; }
    public string? Doctor { get; set; }
    public DateTime? Date { get; set; }
    public string? Time { get; set; }
    public string? Symptoms { get; set; }
    public string? PreviousHistory { get; set; }
}


}
