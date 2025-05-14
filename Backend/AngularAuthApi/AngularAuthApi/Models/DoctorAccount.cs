using System.ComponentModel.DataAnnotations;


    namespace AngularAuthApi.Models
    {
        public class DoctorAccount :  IAccount
        {
            [Key]
            public int Id { get; set; }

            public string? FirstName { get; set; }

            public string? LastName { get; set; }

            [Required]
            public string? Username { get; set; }

            public string? Token { get; set; }

            public string? Role { get; set; } = "Doctor";

           
            public string? Email { get; set; }

            [Required]
            public string? Password { get; set; }

            public string? ResetPasswordToken { get; set; }

            public string? ResetPasswordExpiry { get; set; }

            public string? Phone { get; set; }
        }
    }

