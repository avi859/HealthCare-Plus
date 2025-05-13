using System.Security.Cryptography;
using AngularAuthApi.Context;
using AngularAuthApi.DTOs;
using AngularAuthApi.Helpers;
using AngularAuthApi.Models;
using AngularAuthApi.utilityService;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AngularAuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorAccountController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public DoctorAccountController(AppDbContext context, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] DoctorLoginDto doctor)
        {
            if (doctor == null) return BadRequest(new { Message = "Invalid request" });

            var dbDoctor = await _context.DoctorAccounts.FirstOrDefaultAsync(x => x.Username == doctor.Username);
            if (dbDoctor == null) return NotFound(new { Message = "Username not found!" });

            if (!PasswordHasher.VerifyPassword(doctor.Password, dbDoctor.Password))
                return BadRequest(new { Message = "Invalid password!" });

            var token = JwtHelper.GenerateJwtToken(new User
            {
                Id = dbDoctor.Id,
                Username = dbDoctor.Username,
                Email = dbDoctor.Email,
                Role = "Doctor"
            }, _configuration);

            return Ok(new { Token = token, Message = "Login Success!" });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] DoctorAccount doctor)
        {
            if (doctor == null) return BadRequest();

            if (await _context.DoctorAccounts.AnyAsync(x => x.Username == doctor.Username))
                return BadRequest(new { Message = "Username Already Exist!" });

            if (await _context.DoctorAccounts.AnyAsync(x => x.Email == doctor.Email))
                return BadRequest(new { Message = "Email Already Exist!" });

            doctor.Password = PasswordHasher.HashPassword(doctor.Password);
            await _context.DoctorAccounts.AddAsync(doctor);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Doctor Registered!" });
        }

        [HttpPost("send-reset-email/{email}")]
        public async Task<IActionResult> SendEmail(string email)
        {
            var doctor = await _context.DoctorAccounts.FirstOrDefaultAsync(a => a.Email == email);
            if (doctor == null) return NotFound(new { Message = "Email doesn't exist" });

            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var emailToken = Convert.ToBase64String(tokenBytes);

            doctor.ResetPasswordToken = emailToken;
            doctor.ResetPasswordExpiry = DateTime.Now.AddMinutes(15).ToString("yyyy-MM-dd HH:mm:ss");

            var emailModel = new EmailModel(email, "Reset Password!", EmailBody.EmailStringBody(email, emailToken));
            _emailService.SendEmail(emailModel);

            _context.Entry(doctor).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Email sent!" });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            var newToken = Uri.UnescapeDataString(dto.EmailToken!);
            var doctor = await _context.DoctorAccounts.FirstOrDefaultAsync(a => a.Email == dto.Email);
            if (doctor == null) return NotFound(new { Message = "Email doesn't exist" });

            if (!DateTime.TryParse(doctor.ResetPasswordExpiry, out var expiry) ||
                doctor.ResetPasswordToken != dto.EmailToken ||
                expiry < DateTime.Now)
            {
                return BadRequest(new { Message = "Invalid reset link" });
            }

            doctor.Password = PasswordHasher.HashPassword(dto.NewPassword);
            _context.Entry(doctor).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Password reset successfully" });
        }

        //[HttpPost("google-login")]
        //public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto loginDto)
        //{
        //    try
        //    {
        //        var payload = await GoogleJsonWebSignature.ValidateAsync(loginDto.IdToken);
        //        var doctor = await _context.DoctorAccounts.FirstOrDefaultAsync(u => u.Email == payload.Email);

        //        if (doctor == null)
        //        {
        //            doctor = new DoctorAccount
        //            {
        //                Email = payload.Email,
        //                Username = payload.GivenName,
        //                Password = "",
        //            };
        //            await _context.DoctorAccounts.AddAsync(doctor);
        //            await _context.SaveChangesAsync();
        //        }

        //        var token = JwtHelper.GenerateJwtToken(new User
        //        {
        //            Id = doctor.Id,
        //            Username = doctor.Username,
        //            Email = doctor.Email,
        //            Role = "Doctor"
        //        }, _configuration);

        //        return Ok(new { Token = token });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { Message = $"Google login failed: {ex.Message}" });
        //    }
        //}

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto loginDto)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(loginDto.IdToken);

                var doctor = await _context.DoctorAccounts.FirstOrDefaultAsync(d => d.Email == payload.Email);

                if (doctor == null)
                {
                    // Auto-register new doctor
                    doctor = new DoctorAccount
                    {
                        Email = payload.Email,
                        Username = payload.GivenName,
                        Password = "", // No password needed
                        Role = "Doctor"
                    };
                    await _context.DoctorAccounts.AddAsync(doctor);
                    await _context.SaveChangesAsync();
                }

                var token = JwtHelper.GenerateJwtToken(doctor, _configuration);
                return Ok(new { token });
            }
            catch (InvalidJwtException ex)
            {
                return BadRequest(new { message = $"Invalid Google ID token: {ex.Message}" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Doctor Google Login Error: {ex}");
                return StatusCode(500, new { message = $"Doctor Google login failed: {ex.Message}" });
            }
        }

    }
}
