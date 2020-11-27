using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace ClientWebApp.Infrastructure
{
    public static class EmailManager
    {
        readonly static string host = "smtp.gmail.com";
        readonly static int port = 587;
        public static void Send(string subject, string mailText, string userEmail)
        {
            MailAddress emailFrom = new MailAddress("testmvcapptask@gmail.com", "Azat Islamov");
            MailAddress emailTo = new MailAddress(userEmail);

            MailMessage message = new MailMessage(emailFrom, emailTo);
            message.Subject = subject;
            message.Body = mailText;
            message.IsBodyHtml = false;

            SmtpClient smpt = new SmtpClient(host, port);
            smpt.Credentials = new NetworkCredential("testmvcapptask@gmail.com", "MJzT4hOU");

            smpt.EnableSsl = true;
            smpt.Send(message);
        }
    }
}