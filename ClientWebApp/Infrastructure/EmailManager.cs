using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace ClientWebApp.Infrastructure
{
    public static class EmailManager
    {
        static readonly string host = "smtp.gmail.com";
        static readonly int port = 587;

        static EmailManager()
        {
            host = ConfigurationManager.AppSettings["EmailHost"];
            port = Int16.Parse(ConfigurationManager.AppSettings["EmailPort"]);
        }

        public static void Send(string subject, string mailText, string userEmail)
        {
            MailAddress emailFrom = new MailAddress(ConfigurationManager.AppSettings["Email"],
                ConfigurationManager.AppSettings["EmailName"]);
            MailAddress emailTo = new MailAddress(userEmail);

            MailMessage message = new MailMessage(emailFrom, emailTo);
            message.Subject = subject;
            message.Body = mailText;
            message.IsBodyHtml = false;

            SmtpClient smpt = new SmtpClient(host, port);
            smpt.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["Email"],
                ConfigurationManager.AppSettings["EmailPassword"]);

            smpt.EnableSsl = true;
            try
            {
                smpt.Send(message);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"MY MESSAGE: Email was not sent {e.Message}");
            }
        }
    }
}