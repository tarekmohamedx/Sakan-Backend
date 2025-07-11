using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Sakan.Application.Interfaces;
using SendGrid.Helpers.Mail;
using SendGrid;

namespace Sakan.Application.Services
{
    public class SendGridEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public SendGridEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            var apiKey = _configuration["EmailSetting:SendGridApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                // لا تقم بإيقاف التطبيق، فقط سجل الخطأ
                Console.WriteLine("SendGrid API Key is not configured.");
                return;
            }

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(
                _configuration["EmailSetting:FromEmail"],
                _configuration["EmailSetting:FromName"]
            );
            var to = new EmailAddress(toEmail);

            // يمكنك إنشاء محتوى نصي بسيط كبديل للمتصفحات التي لا تدعم HTML
            var plainTextContent = "Please view this email in an HTML-compatible email client.";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                // سجل الخطأ لمتابعته لاحقاً
                Console.WriteLine($"Failed to send email. Status Code: {response.StatusCode}");
                // يمكنك قراءة تفاصيل الخطأ من response.Body
            }
            else
            {
                Console.WriteLine($"Email sent successfully to {toEmail}");
            }
        }
    }
}
