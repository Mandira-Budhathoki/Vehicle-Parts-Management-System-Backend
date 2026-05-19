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

        // ─────────────────────────────────────────────────────────────────────
        // POST: api/email/send-invoice
        // Feature 11: Staff sends a completed sales invoice to a customer's email.
        // Requires a real SalesId (created by POST /api/sales in Feature 7).
        // ─────────────────────────────────────────────────────────────────────
        [HttpPost("send-invoice")]
        public async Task<IActionResult> SendInvoice([FromBody] SendInvoiceEmailDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.CustomerEmail))
                return BadRequest(new { message = "Customer email is required." });

            // Load the sale with all related data
            var sale = await _context.Sales
                .Include(s => s.SalesItems)
                    .ThenInclude(si => si.Part)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.SalesId == dto.SalesId);

            if (sale == null)
                return NotFound(new { message = $"Sale record #{dto.SalesId} not found." });

            // Build professional HTML invoice body
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html><head><meta charset='utf-8'/></head>");
            sb.AppendLine("<body style='font-family:Arial,sans-serif;color:#333;max-width:650px;margin:0 auto;padding:24px;'>");

            // Header
            sb.AppendLine("<div style='background:linear-gradient(135deg,#6366f1,#8b5cf6);padding:24px 28px;border-radius:10px 10px 0 0;'>");
            sb.AppendLine("<h1 style='color:#fff;margin:0;font-size:1.6em;'>VP Vehicle Parts</h1>");
            sb.AppendLine("<p style='color:rgba(255,255,255,0.85);margin:4px 0 0;font-size:0.9em;'>Vehicle Services & Parts Retail Center</p>");
            sb.AppendLine("</div>");

            // Invoice meta
            sb.AppendLine("<div style='border:1px solid #e0e0e0;border-top:none;padding:24px 28px;border-radius:0 0 10px 10px;'>");
            sb.AppendLine("<div style='display:flex;justify-content:space-between;margin-bottom:20px;'>");
            sb.AppendLine($"<div><strong style='font-size:1.1em;color:#6366f1;'>Invoice #{sale.SalesId:D6}</strong><br/><span style='color:#777;font-size:0.85em;'>{sale.Date:dd MMMM yyyy, hh:mm tt} UTC</span></div>");
            sb.AppendLine($"<div style='text-align:right;'><strong>Billed To:</strong><br/>{sale.User?.Name ?? "Customer"}<br/><span style='color:#777;font-size:0.85em;'>{dto.CustomerEmail}</span></div>");
            sb.AppendLine("</div>");
            sb.AppendLine("<hr style='border:none;border-top:1px solid #e0e0e0;margin-bottom:20px;'/>");

            // Items table
            sb.AppendLine("<table style='width:100%;border-collapse:collapse;font-size:0.95em;'>");
            sb.AppendLine("<thead>");
            sb.AppendLine("<tr style='background:#6366f1;color:#fff;'>");
            sb.AppendLine("<th style='padding:10px 12px;text-align:left;border-radius:4px 0 0 0;'>Part Name</th>");
            sb.AppendLine("<th style='padding:10px 12px;text-align:center;'>Qty</th>");
            sb.AppendLine("<th style='padding:10px 12px;text-align:right;'>Unit Price</th>");
            sb.AppendLine("<th style='padding:10px 12px;text-align:right;border-radius:0 4px 0 0;'>Subtotal</th>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</thead><tbody>");

            bool alt = false;
            foreach (var item in sale.SalesItems)
            {
                string rowBg = alt ? "#f9f9ff" : "#ffffff";
                alt = !alt;
                sb.AppendLine($"<tr style='background:{rowBg};'>");
                sb.AppendLine($"<td style='padding:9px 12px;border-bottom:1px solid #ebebeb;'>{item.Part?.PartName ?? "N/A"}</td>");
                sb.AppendLine($"<td style='padding:9px 12px;text-align:center;border-bottom:1px solid #ebebeb;'>{item.Quantity}</td>");
                sb.AppendLine($"<td style='padding:9px 12px;text-align:right;border-bottom:1px solid #ebebeb;'>Rs. {item.Price:N2}</td>");
                sb.AppendLine($"<td style='padding:9px 12px;text-align:right;border-bottom:1px solid #ebebeb;'>Rs. {item.Subtotal:N2}</td>");
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</tbody></table>");

            // Totals
            sb.AppendLine("<div style='margin-top:16px;text-align:right;'>");
            sb.AppendLine($"<p style='margin:4px 0;'>Subtotal: <strong>Rs. {sale.TotalAmount:N2}</strong></p>");
            if (sale.Discount > 0)
                sb.AppendLine($"<p style='margin:4px 0;color:#22c55e;'>Loyalty Discount: <strong>– Rs. {sale.Discount:N2}</strong></p>");
            sb.AppendLine($"<p style='font-size:1.2em;margin:8px 0 0;padding-top:8px;border-top:2px solid #6366f1;color:#6366f1;'>Total Due: <strong>Rs. {sale.FinalAmount:N2}</strong></p>");
            sb.AppendLine($"<p style='margin:8px 0 0;'><span style='background:{(sale.PaymentStatus == "Completed" ? "#dcfce7" : "#fef9c3")};color:{(sale.PaymentStatus == "Completed" ? "#15803d" : "#92400e")};padding:3px 10px;border-radius:20px;font-size:0.85em;font-weight:bold;'>{sale.PaymentStatus.ToUpper()}</span></p>");
            sb.AppendLine("</div>");

            sb.AppendLine("<hr style='border:none;border-top:1px solid #e0e0e0;margin-top:24px;'/>");
            sb.AppendLine("<p style='color:#9ca3af;font-size:0.8em;text-align:center;margin-top:12px;'>Thank you for choosing VP Vehicle Parts Center. For queries, reply to this email.</p>");
            sb.AppendLine("</div>");
            sb.AppendLine("</body></html>");

            try
            {
                await SendEmailAsync(
                    dto.CustomerEmail,
                    $"Your Invoice #{sale.SalesId:D6} from VP Vehicle Parts",
                    sb.ToString()
                );

                return Ok(new
                {
                    message = $"Invoice #{sale.SalesId:D6} sent successfully to {dto.CustomerEmail}.",
                    salesId = sale.SalesId,
                    sentTo = dto.CustomerEmail
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Email delivery failed. Check Smtp credentials in appsettings.json.",
                    detail = ex.Message
                });
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Private helper: sends via Gmail SMTP (TLS port 587)
        // Credentials come from appsettings.json  →  "Smtp" section
        // ─────────────────────────────────────────────────────────────────────
        private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var smtp = _config.GetSection("Smtp");
            var host = smtp["Host"] ?? "smtp.gmail.com";
            var port = int.Parse(smtp["Port"] ?? "587");
            var fromEmail = smtp["From"] ?? throw new InvalidOperationException("Smtp:From not configured in appsettings.json.");
            var password = smtp["Password"] ?? throw new InvalidOperationException("Smtp:Password not configured in appsettings.json.");
            var displayName = smtp["DisplayName"] ?? "VP Vehicle Parts";

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
