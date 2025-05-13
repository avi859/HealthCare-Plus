using System.ComponentModel.DataAnnotations;

namespace AngularAuthApi.DTOs
{
    public class DoctorLoginDto
    {
        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
