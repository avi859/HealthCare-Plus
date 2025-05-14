using AngularAuthApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AngularAuthApi.Context
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<User> Users { get; set; } = default!;
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Appointment> Appointments { get; set; }

    public DbSet<AppointmentBooking> Bookings { get; set; }
    public DbSet<DoctorAccount> DoctorAccounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Doctor>()
              .Property(d => d.ConsultationFee)
              .HasPrecision(10, 2);
    }

  }
}
