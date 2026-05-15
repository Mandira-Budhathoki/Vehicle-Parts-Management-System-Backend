using backend.Data;
using backend.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace backend.Controllers
{
    [Route("api/email")]
    [ApiController]
    [Authorize(Roles = "STAFF,ADMIN")]
    public class EmailController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public EmailController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // POST: api/email/send-invoice
        // Feature 11: Staff sends a sales invoice to a customer's email
        [HttpPost("send-invoice")]
        public async Task<IActionResult> SendInvoice([FromBody] SendInvoiceEmailDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.CustomerEmail))
                return BadRequest(new { message = "Customer email is required." });

            var sale = await _context.Sales
                .Include(s => s.SalesItems)
                    .ThenInclude(si => si.Part)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.SalesId == dto.SalesId);

            if (sale == null)
                return NotFound(new { message = "Sale record not found." });

            // Build HTML invoice body
            var sb = new StringBuilder();
            sb.AppendLine("<html><body style='font-family:Arial,sans-serif;color:#333;'>");
            sb.AppendLine("<h2 style='color:#6366f1;'>VP Vehicle Parts — Invoice</h2>");
            sb.AppendLine($"<p><strong>Invoice #:</strong> INV-{sale.SalesId:D6}</p>");
            sb.AppendLine($"<p><strong>Date:</strong> {sale.Date:dd MMM yyyy}</p>");
            sb.AppendLine($"<p><strong>Customer:</strong> {sale.User.Name}</p>");
            sb.AppendLine("<hr/>");
            sb.AppendLine("<table border='1' cellpadding='8' cellspacing='0' style='border-collapse:collapse;width:100%;'>");
            sb.AppendLine("<thead style='background:#6366f1;color:#fff;'><tr><th>Part</th><th>Qty</th><th>Unit Price</th><th>Subtotal</th></tr></thead>");
            sb.AppendLine("<tbody>");

            foreach (var item in sale.SalesItems)
            {
                sb.AppendLine($"<tr><td>{item.Part?.PartName ?? "N/A"}</td><td>{item.Quantity}</td><td>Rs. {item.Price:F2}</td><td>Rs. {item.Subtotal:F2}</td></tr>");
            }

            sb.AppendLine("</tbody></table><br/>");
            sb.AppendLine($"<p><strong>Subtotal:</strong> Rs. {sale.TotalAmount:F2}</p>");

            if (sale.Discount > 0)
                sb.AppendLine($"<p><strong>Discount:</strong> Rs. {sale.Discount:F2}</p>");

            sb.AppendLine($"<p style='font-size:1.1em;'><strong>Total Due:</strong> Rs. {sale.FinalAmount:F2}</p>");
            sb.AppendLine($"<p><strong>Payment Status:</strong> {sale.PaymentStatus}</p>");
            sb.AppendLine("<hr/><p style='color:#888;font-size:0.85em;'>Thank you for your business! — VP Vehicle Parts Center</p>");
            sb.AppendLine("</body></html>");

            try
            {
                await SendEmailAsync(dto.CustomerEmail, $"Invoice INV-{sale.SalesId:D6} from VP Vehicle Parts", sb.ToString());
                return Ok(new { message = $"Invoice sent successfully to {dto.CustomerEmail}." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to send email.", detail = ex.Message });
            }
        }

        private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var smtpSection = _config.GetSection("Smtp");
            var host = smtpSection["Host"] ?? "smtp.gmail.com";
            var port = int.Parse(smtpSection["Port"] ?? "587");
            var fromEmail = smtpSection["From"] ?? throw new InvalidOperationException("Smtp:From not configured.");
            var password = smtpSection["Password"] ?? throw new InvalidOperationException("Smtp:Password not configured.");
            var displayName = smtpSection["DisplayName"] ?? "VP Vehicle Parts";

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(fromEmail, displayName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            mail.To.Add(toEmail);
            await client.SendMailAsync(mail);
        }
    }
}