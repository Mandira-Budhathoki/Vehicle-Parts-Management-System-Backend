using backend.Data;
using backend.Model;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace backend.Services
{
    public class AutomatedTaskService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<AutomatedTaskService> _logger;

        public AutomatedTaskService(IServiceScopeFactory scopeFactory, IConfiguration config, ILogger<AutomatedTaskService> logger)
        {
            _scopeFactory = scopeFactory;
            _config = config;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AutomatedTaskService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessTasksAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing automated tasks.");
                }

                // Wait for 10 seconds for demo/testing purposes
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        private async Task ProcessTasksAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await CheckLowStockAsync(dbContext);
            await CheckUnpaidCreditsAsync(dbContext);
        }

        private async Task CheckLowStockAsync(AppDbContext dbContext)
        {
            var lowStockParts = await dbContext.Parts
                .Where(p => p.StockQuantity < 10)
                .ToListAsync();

            if (!lowStockParts.Any()) return;

            var admins = await dbContext.Users
                .Where(u => u.Role == "ADMIN")
                .ToListAsync();

            if (!admins.Any()) return;

            var thresholdTime = DateTime.UtcNow.AddDays(-1);

            foreach (var part in lowStockParts)
            {
                var message = $"Low stock alert: {part.PartName} (Stock: {part.StockQuantity})";

                foreach (var admin in admins)
                {
                    var recentNotif = await dbContext.Notifications
                        .AnyAsync(n => n.UserId == admin.UserId 
                                    && n.Type == "ALERT" 
                                    && n.Message == message 
                                    && n.CreatedAt > thresholdTime);

                    if (!recentNotif)
                    {
                        var notification = new Notification
                        {
                            UserId = admin.UserId,
                            Message = message,
                            Type = "ALERT",
                            IsRead = false,
                            CreatedAt = DateTime.UtcNow
                        };
                        dbContext.Notifications.Add(notification);
                    }
                }
            }

            await dbContext.SaveChangesAsync();
        }

        private async Task CheckUnpaidCreditsAsync(AppDbContext dbContext)
        {
            var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);

            var unpaidSales = await dbContext.Sales
                .Include(s => s.User)
                .Where(s => s.PaymentStatus == "PENDING" && s.Date < oneMonthAgo)
                .ToListAsync();

            foreach (var sale in unpaidSales)
            {
                if (sale.User == null || string.IsNullOrWhiteSpace(sale.User.Email))
                    continue;

                var reminderMessage = $"REMINDER: Unpaid Invoice INV-{sale.SalesId:D6}";

                var thresholdTime = DateTime.UtcNow.AddDays(-7);
                var recentReminder = await dbContext.Notifications
                    .AnyAsync(n => n.UserId == sale.User.UserId 
                                && n.Type == "CREDIT_REMINDER" 
                                && n.Message == reminderMessage 
                                && n.CreatedAt > thresholdTime);

                if (!recentReminder)
                {
                    bool emailSent = await SendReminderEmailAsync(sale.User, sale);

                    if (emailSent)
                    {
                        var notification = new Notification
                        {
                            UserId = sale.User.UserId,
                            Message = reminderMessage,
                            Type = "CREDIT_REMINDER",
                            IsRead = false,
                            CreatedAt = DateTime.UtcNow
                        };
                        dbContext.Notifications.Add(notification);
                    }
                }
            }

            await dbContext.SaveChangesAsync();
        }

        private async Task<bool> SendReminderEmailAsync(User user, Sales sale)
        {
            try
            {
                var smtpSection = _config.GetSection("Smtp");
                var host = smtpSection["Host"] ?? "smtp.gmail.com";
                if (string.IsNullOrEmpty(smtpSection["Host"]))
                {
                    _logger.LogWarning("SMTP not fully configured. Skipping email.");
                    return false; // Return false so we can test the fallback or avoid inserting a notification if it failed
                }
                
                var portStr = smtpSection["Port"];
                var port = string.IsNullOrEmpty(portStr) ? 587 : int.Parse(portStr);
                
                var fromEmail = smtpSection["From"] ?? "";
                var password = smtpSection["Password"] ?? "";
                var displayName = smtpSection["DisplayName"] ?? "VP Vehicle Parts";

                if (string.IsNullOrEmpty(fromEmail) || string.IsNullOrEmpty(password))
                {
                    _logger.LogWarning("SMTP credentials missing. Skipping email.");
                    return false;
                }

                using var client = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(fromEmail, password),
                    EnableSsl = true
                };

                var sb = new StringBuilder();
                sb.AppendLine("<html><body style='font-family:Arial,sans-serif;color:#333;'>");
                sb.AppendLine($"<h2 style='color:#ef4444;'>VP Vehicle Parts — Payment Reminder</h2>");
                sb.AppendLine($"<p>Dear {user.Name},</p>");
                sb.AppendLine($"<p>This is a friendly reminder that your invoice <strong>INV-{sale.SalesId:D6}</strong> from {sale.Date:dd MMM yyyy} is currently unpaid and is overdue by more than a month.</p>");
                sb.AppendLine($"<p><strong>Total Due:</strong> Rs. {sale.FinalAmount:F2}</p>");
                sb.AppendLine("<p>Please settle this amount as soon as possible to continue enjoying our services.</p>");
                sb.AppendLine("<hr/><p style='color:#888;font-size:0.85em;'>Thank you for your business! — VP Vehicle Parts Center</p>");
                sb.AppendLine("</body></html>");

                var mail = new MailMessage
                {
                    From = new MailAddress(fromEmail, displayName),
                    Subject = $"Payment Reminder: Invoice INV-{sale.SalesId:D6}",
                    Body = sb.ToString(),
                    IsBodyHtml = true
                };

                mail.To.Add(user.Email);
                await client.SendMailAsync(mail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send reminder email to {Email}", user.Email);
                return false;
            }
        }
    }
}
