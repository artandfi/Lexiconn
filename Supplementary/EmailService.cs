using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexiconn.Supplementary
{
    public class EmailService
    {
        private const string TITLE_ADMIN = "Адміністрація Lexiconn";
        private const string MAIL_ADMIN = "misdispel@gmail.com";
        private const string PWD_ADMIN = "dummy";
        private const string SMTP_SERVER = "smtp.gmail.com";
        private const int SMTP_PORT = 465;

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(TITLE_ADMIN, MAIL_ADMIN));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message };
            
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(SMTP_SERVER, SMTP_PORT, true);
                await client.AuthenticateAsync(MAIL_ADMIN, PWD_ADMIN);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}