using AngularAuthApi.Context;
using AngularAuthApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace AngularAuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBooking([FromBody] Booking booking)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == booking.Username);
            if (user == null)
            {
                return BadRequest(new { Message = "Invalid User. Cannot create booking." });
            }

            booking.Username = user.Username; // Optional: Auto-fill username if needed

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Booking created successfully!" });
        }
        


    }

}
