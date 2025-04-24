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
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Booking created successfully!" });
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetBookingsByUsername(string username)
        {
            var bookings = await _context.Bookings
                .Where(b => b.Username == username)
                .ToListAsync();
            return Ok(bookings);
        }
    }

}
