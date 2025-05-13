// Services/AppointmentService.cs
using AngularAuthApi.Context;
using AngularAuthApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularAuthApi.Services
{
    public class AppointmentService
    {
        private readonly AppDbContext _context;

        public AppointmentService(AppDbContext context)
        {
            _context = context;
        }

        // Fetch appointments for a specific doctor
        public async Task<List<Appointment>> GetAppointmentsByDoctorAsync(string doctorPhone)
        {
            return await _context.Appointments
                .Where(a => a.DoctorPhone == doctorPhone)
                .OrderBy(a => a.AppointmentTime)
                .ToListAsync();
        }

        // Add a new appointment
        public async Task AddAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
        }

        // Update appointment status (for example, when completed)
        public async Task UpdateAppointmentStatusAsync(int appointmentId, string status)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment != null)
            {
                appointment.Status = status;
                await _context.SaveChangesAsync();
            }
        }

        // You can add more methods like deleting or fetching by patient name
    }
}
