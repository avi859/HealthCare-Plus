using System;
using AngularAuthApi.Context;
using AngularAuthApi.Helpers;
using AngularAuthApi.Models;
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

    public UserController(AppDbContext appDbContext)
    {
      _authContext = appDbContext;
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

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] User userObj)
    {
      if(userObj == null)return BadRequest();
      //check username
      if(await CheckUserNameExistAsync(userObj.Username))
      {
        return BadRequest(new {Message = "Username Already Exist!"});
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
