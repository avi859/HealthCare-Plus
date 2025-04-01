using AngularAuthApi.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

[ApiController]
[Route("api/appointment")]
public class AppointmentController : ControllerBase
{
  private readonly IWebHostEnvironment _webHostEnvironment;

  public AppointmentController(IWebHostEnvironment webHostEnvironment)
  {
    _webHostEnvironment = webHostEnvironment;
  }

  [HttpPost("generate-pdf")]
  public IActionResult GeneratePDF([FromBody] AppointmentModel appointment)
  {
    if (appointment == null)
    {
      return BadRequest("Invalid data");
    }

    using (MemoryStream memoryStream = new MemoryStream())
    {
      Document document = new Document(PageSize.A4, 40, 40, 50, 50);
      PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
      document.Open();

      // ✅ Get Absolute Path for Image
      string logoPath = Path.Combine(_webHostEnvironment.WebRootPath, "temp", "logo.png");

      if (System.IO.File.Exists(logoPath))
      {
        Image logo = Image.GetInstance(logoPath);
        logo.ScaleToFit(100, 100);
        logo.Alignment = Element.ALIGN_CENTER;
        document.Add(logo);
      }

      // ✅ Title
      Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, new BaseColor(0, 0, 255));
      Paragraph title = new Paragraph("Healthcare Plus - Appointment Confirmation\n\n", titleFont);
      title.Alignment = Element.ALIGN_CENTER;
      document.Add(title);

      // ✅ Subtitle
      Font subtitleFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, new BaseColor(105, 105, 105));
      Paragraph subtitle = new Paragraph("Your appointment has been successfully booked. Below are the details:\n\n", subtitleFont);
      subtitle.Alignment = Element.ALIGN_CENTER;
      document.Add(subtitle);

      // ✅ Create Table
      PdfPTable table = new PdfPTable(2);
      table.WidthPercentage = 100;
      table.SetWidths(new float[] { 2, 3 });

      Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, new BaseColor(255, 255, 255));
      BaseColor headerBgColor = new BaseColor(0, 121, 193);
      Font dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, new BaseColor(0, 0, 0));

      PdfPCell AddCell(string text, bool isHeader = false)
      {
        PdfPCell cell = new PdfPCell(new Phrase(text, isHeader ? headerFont : dataFont));
        cell.Padding = 8;
        if (isHeader)
        {
          cell.BackgroundColor = headerBgColor;
          cell.HorizontalAlignment = Element.ALIGN_CENTER;
        }
        return cell;
      }

      // ✅ Table Headers
      table.AddCell(AddCell("Field", true));
      table.AddCell(AddCell("Details", true));

      // ✅ Table Data
      table.AddCell("Full Name:");
      table.AddCell(appointment.FullName);
      table.AddCell("Email:");
      table.AddCell(appointment.Email);
      table.AddCell("Phone:");
      table.AddCell(appointment.Phone);
      table.AddCell("Department:");
      table.AddCell(appointment.Department);
      table.AddCell("Doctor:");
      table.AddCell(appointment.Doctor);
      table.AddCell("Date:");
      table.AddCell(appointment.Date);
      table.AddCell("Time:");
      table.AddCell(appointment.Time);
      table.AddCell("Symptoms:");
      table.AddCell(appointment.Symptoms);
      table.AddCell("Medical History:");
      table.AddCell(appointment.PreviousHistory);

      document.Add(table);

      // ✅ Footer
      Paragraph footer = new Paragraph("\nThank you for choosing Healthcare Plus. We look forward to serving you!", subtitleFont);
      footer.Alignment = Element.ALIGN_CENTER;
      document.Add(footer);

      document.Close();

      byte[] pdfBytes = memoryStream.ToArray();
      return File(pdfBytes, "application/pdf", "AppointmentConfirmation.pdf");
    }
  }
}
