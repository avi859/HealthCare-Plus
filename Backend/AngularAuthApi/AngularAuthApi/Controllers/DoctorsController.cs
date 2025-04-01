using AngularAuthApi.Context;
using AngularAuthApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AngularAuthApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DoctorsController : ControllerBase
  {
    private readonly AppDbContext _context;

    public DoctorsController(AppDbContext context)
    {
      _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetDoctors()
    {
      var doctors = await _context.Doctors.ToListAsync();
      if (doctors == null || doctors.Count == 0)
      {
        SeedDummyData();
        doctors = await _context.Doctors.ToListAsync();
      }
      return Ok(doctors);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDoctor(int id)
    {
      var doctor = await _context.Doctors.FindAsync(id);

      if (doctor == null)
      {
        return NotFound();
      }

      return Ok(doctor);
    }

    [HttpPost]
    public async Task<IActionResult> CreateDoctor([FromBody] Doctor doctor)
    {
      if (doctor == null)
      {
        return BadRequest();
      }

      _context.Doctors.Add(doctor);
      await _context.SaveChangesAsync();

      return Ok(new { Message = "Doctor added successfully!", Doctor = doctor });
    }





    private void SeedDummyData()
    {
      var dummyDoctors = new List<Doctor>
            {
                new Doctor
                {
                    Name = "Dr. Emma Johnson",
                    Specialty = "Cardiology",
                    Education = "MD, Harvard Medical School",
                    Experience = 12,
                    ImageUrl = "https://randomuser.me/api/portraits/men/1.jpg",
                    PhoneNumber = "+15005550006"
                },
                new Doctor
                {
                    Name = "Dr. Michael Brown",
                    Specialty = "Neurology",
                    Education = "MD, Johns Hopkins University",
                    Experience = 15,
                    ImageUrl = "https://via.placeholder.com/150?text=Dr.+Michael+Brown",
                    PhoneNumber = "+15005550006"
                },
                new Doctor
                {
                    Name = "Dr. Sarah Wilson",
                    Specialty = "Pediatrics",
                    Education = "MD, Stanford University",
                    Experience = 10,
                    ImageUrl = "https://via.placeholder.com/150?text=Dr.+Sarah+Wilson",
                    PhoneNumber = "+15005550006"
                },
                new Doctor
                {
                    Name = "Dr. Robert Thompson",
                    Specialty = "Orthopedics",
                    Education = "MD, Yale University",
                    Experience = 18,
                    ImageUrl = "https://via.placeholder.com/150?text=Dr.+Robert+Thompson",
                    PhoneNumber = "+15005550006"
                },
                new Doctor
                {
                    Name = "Dr. Olivia Davis",
                    Specialty = "Dermatology",
                    Education = "MD, Columbia University",
                    Experience = 8,
                    ImageUrl = "https://via.placeholder.com/150?text=Dr.+Olivia+Davis",
                    PhoneNumber = "+15005550006"
                },
                new Doctor
                {
                    Name = "Dr. James Anderson",
                    Specialty = "Cardiology",
                    Education = "MD, Duke University",
                    Experience = 14,
                    ImageUrl = "https://via.placeholder.com/150?text=Dr.+James+Anderson",
                    PhoneNumber = "+15005550006"
                },
                new Doctor
                {
                    Name = "Dr. Laura Martinez",
                    Specialty = "Neurology",
                    Education = "MD, University of California, San Francisco",
                    Experience = 11,
                    ImageUrl = "https://via.placeholder.com/150?text=Dr.+Laura+Martinez",
                    PhoneNumber = "+15005550006"
                },
                new Doctor
                {
                    Name = "Dr. David Lee",
                    Specialty = "Orthopedics",
                    Education = "MD, Cornell University",
                    Experience = 16,
                    ImageUrl = "https://via.placeholder.com/150?text=Dr.+David+Lee",
                    PhoneNumber = "+15005550006"
                },
                new Doctor
                {
                    Name = "Dr. Emily Carter",
                    Specialty = "Pediatrics",
                    Education = "MD, University of Pennsylvania",
                    Experience = 9,
                    ImageUrl = "https://via.placeholder.com/150?text=Dr.+Emily+Carter",
                    PhoneNumber = "+15005550006"
                },
                new Doctor
                {
                    Name = "Dr. Ryan Patel",
                    Specialty = "Dermatology",
                    Education = "MD, Northwestern University",
                    Experience = 7,
                    ImageUrl = "https://via.placeholder.com/150?text=Dr.+Ryan+Patel",
                    PhoneNumber = "+15005550006"
                },
                new Doctor
                {
                    Name = "Dr. Sophia Kim",
                    Specialty = "General Medicine",
                    Education = "MD, University of Chicago",
                    Experience = 13,
                    ImageUrl = "https://via.placeholder.com/150?text=Dr.+Sophia+Kim",
                    PhoneNumber = "+15005550006"
                },
                new Doctor
                {
                    Name = "Dr. Thomas Garcia",
                    Specialty = "Cardiology",
                    Education = "MD, Mayo Clinic",
                    Experience = 15,
                    ImageUrl = "https://via.placeholder.com/150?text=Dr.+Thomas+Garcia",
                    PhoneNumber = "+15005550006"
                },
                new Doctor
                {
                    Name = "Dr. Anna White",
                    Specialty = "Neurology",
                    Education = "MD, Massachusetts Institute of Technology",
                    Experience = 10,
                    ImageUrl = "https://via.placeholder.com/150?text=Dr.+Anna+White",
                    PhoneNumber = "+15005550006"
                },
                new Doctor
                {
                    Name = "Dr. Henry Turner",
                    Specialty = "Orthopedics",
                    Education = "MD, University of Michigan",
                    Experience = 17,
                    ImageUrl = "https://via.placeholder.com/150?text=Dr.+Henry+Turner",
                    PhoneNumber = "+15005550006"
                },
                new Doctor
                {
                    Name = "Dr. Rachel Adams",
                    Specialty = "Pediatrics",
                    Education = "MD, University of Washington",
                    Experience = 8,
                    ImageUrl = "https://via.placeholder.com/150?text=Dr.+Rachel+Adams",
                    PhoneNumber = "+15005550006"
                },
                new Doctor
                {
                    Name = "Dr. Ethan Moore",
                    Specialty = "Dermatology",
                    Education = "MD, University of Texas",
                    Experience = 6,
                    ImageUrl = "https://via.placeholder.com/150?text=Dr.+Ethan+Moore",
                    PhoneNumber = "+15005550006"
                }
            };

      // Check if there are any doctors in the database
      if (!_context.Doctors.Any())
      {
        _context.Doctors.AddRange(dummyDoctors);
      }
      else
      {
        // Update existing doctors with missing ImageUrl and PhoneNumber
        var existingDoctors = _context.Doctors.ToList();
        for (int i = 0; i < dummyDoctors.Count && i < existingDoctors.Count; i++)
        {
          var existingDoctor = existingDoctors[i];
          var dummyDoctor = dummyDoctors[i];

          existingDoctor.ImageUrl = dummyDoctor.ImageUrl;
          existingDoctor.PhoneNumber = dummyDoctor.PhoneNumber;
        }
      }
      _context.SaveChanges();
    }
  }
}
