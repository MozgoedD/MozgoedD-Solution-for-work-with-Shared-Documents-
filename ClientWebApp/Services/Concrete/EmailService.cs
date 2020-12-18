using ClientWebApp.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;
using System.Web;

namespace ClientWebApp.Services.Concrete
{
    public class EmailService : IEmailService
    {
        SmtpSection smtpSection;
        readonly string EmailFrom;

        public EmailService()
        {
            smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            EmailFrom = smtpSection.From;
        }
        public void SendEmail(string subject, string messageText, string recipientAddress)
        {
            using (var smpt = new SmtpClient())
            using (var message = new MailMessage())
            {
                message.From = new MailAddress(EmailFrom, "Shared Documents Web App");
                message.Bcc.Add(new MailAddress(recipientAddress));
                message.Subject = subject;
                message.Body = messageText;
                message.IsBodyHtml = false;

                smpt.EnableSsl = false;
                try
                {
                    smpt.Send(message);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Email message was not sent: {e}");
                }
            }
        }
    }
}