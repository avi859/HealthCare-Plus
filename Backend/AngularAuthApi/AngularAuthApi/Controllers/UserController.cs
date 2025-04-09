using System;
using System.Security.Cryptography;
using AngularAuthApi.Context;
using AngularAuthApi.Helpers;
using AngularAuthApi.Models;
using AngularAuthApi.utilityService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AngularAuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _authContext;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public UserController(AppDbContext appDbContext, IConfiguration configuration, IEmailService emailService)
        {
            _authContext = appDbContext;
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User userObj)
        {
            if (userObj == null)
            {
                Console.WriteLine("Received null userObj");
                return BadRequest(new { Message = "Invalid request" });
            }

            Console.WriteLine($"Received Username: {userObj.Username}");

            var user = await _authContext.Users.FirstOrDefaultAsync(x => x.Username == userObj.Username);

            if (user == null)
            {
                Console.WriteLine($"User with username '{userObj.Username}' not found in database.");
                return NotFound(new { Message = "Username not found!" });
            }

            if (!PasswordHasher.VerifyPassword(userObj.Password, user.Password))
            {
                Console.WriteLine("Password does not match");
                return BadRequest(new { Message = "Invalid password!" });
            }

            return Ok(new { Message = "Login Success!" });
        }

        [HttpPost("send-reset-email/{email}")]
        public async Task<IActionResult> SendEmail(string email)
        {
            // Look for the user in the database by email
            var user = await _authContext.Users.FirstOrDefaultAsync(a => a.Email == email);

            if (user == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Email doesn't exist"
                });
            }

            // Generate a secure token (you might want to store it in the DB or a cache with an expiration)
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var emailToken = Convert.ToBase64String(tokenBytes);
            user.ResetPasswordToken = emailToken;
            user.ResetPasswordExpiry = DateTime.Now.AddMinutes(15).ToString("yyyy-MM-dd HH:mm:ss");
            string from = _configuration["EmailSettings:From"];
            var emailModel = new EmailModel(email, "Reset Password!", EmailBody.EmailStringBody(email, emailToken));
            _emailService.SendEmail(emailModel);
            _authContext.Entry(user).State = EntityState.Modified;
            await _authContext.SaveChangesAsync();
            return Ok(new
            {
                StatusCode = 200,
                Message = "Email sent!"
            });
        }

        [HttpPost("reset -Password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            //var newToken = resetPasswordDto.EmailToken!.Replace(" ", "+");
            var newToken = Uri.UnescapeDataString(resetPasswordDto.EmailToken!);
            var user = await _authContext.Users.FirstOrDefaultAsync(a => a.Email == resetPasswordDto.Email);
            if (user == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Email doesn't exist"
                });
            }
            var tokenCode = user.ResetPasswordToken;
            //DateTime emailTokenExpiry = user.ResetPasswordExpiry;
            DateTime emailTokenExpiry;
            if (!DateTime.TryParse(user.ResetPasswordExpiry, out emailTokenExpiry))
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Invalid token expiry format"
                });
            }
            if (tokenCode != resetPasswordDto.EmailToken || emailTokenExpiry < DateTime.Now)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Invalid reset Link"
                });
            }
            user.Password = PasswordHasher.HashPassword(resetPasswordDto.NewPassword);
            _authContext.Entry(user).State = EntityState.Modified;
            await _authContext.SaveChangesAsync();
            return Ok(new
            {
                StatusCode = 200,
                Message = "Password reset successfully"
            });
        }




        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User userObj)
        {
            if (userObj == null) return BadRequest();
            //check username
            if (await CheckUserNameExistAsync(userObj.Username))
            {
                return BadRequest(new { Message = "Username Already Exist!" });
            }


            //check Email
            if (await CheckEmailExistAsync(userObj.Email))
            {
                return BadRequest(new { Message = "Email Already Exist!" });
            }


            //check password Strength


            userObj.Password = PasswordHasher.HashPassword(userObj.Password);
            userObj.Role = "User";
            userObj.Token = "";
            await _authContext.Users.AddAsync(userObj);
            await _authContext.SaveChangesAsync();
            return Ok(new { Message = "User Registered!" });
        }
        //need to check is username is unique or not
        private async Task<bool> CheckUserNameExistAsync(string username)
        {
            return await _authContext.Users.AnyAsync(x => x.Username == username);
        }
        private async Task<bool> CheckEmailExistAsync(string email)
        {
            return await _authContext.Users.AnyAsync(x => x.Email == email);
        }

    }
}
