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
        public async Task<IActionResult> CreateBooking([FromBody] AppointmentBooking booking)
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

        [HttpGet("user/{username}")]
        public async Task<IActionResult> GetUserBookings(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest(new { Message = "Username is required." });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            var bookings = await _context.Bookings
                .Where(b => b.Username == username)
                .OrderByDescending(b => b.Date)
                .ToListAsync();

            return Ok(bookings);
        }
        [HttpGet("upcoming/{username}")]
        public async Task<IActionResult> GetUpcomingBookings(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest(new { Message = "Username is required." });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            var today = DateTime.Now.Date;

            var upcomingBookings = await _context.Bookings
                .Where(b => b.Username == username && b.Date >= today)
                .OrderBy(b => b.Date)
                .ToListAsync();

            return Ok(upcomingBookings);
        }
        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound(new { Message = "Booking not found." });
            }

            booking.Status = "Cancelled";
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Booking cancelled successfully." });
        }
        [HttpGet("doctor/{doctorName}")]
        public async Task<IActionResult> GetAppointmentsForDoctor(string doctorName)
        {
            var bookings = await _context.Bookings
                .Where(b => b.Doctor == doctorName) // Match based on Doctor field
                .ToListAsync();

            if (bookings == null || bookings.Count == 0)
            {
                return NotFound("No appointments found for this doctor.");
            }

            return Ok(bookings);
        }



    }

}
