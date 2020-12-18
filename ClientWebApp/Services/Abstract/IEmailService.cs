using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientWebApp.Services.Abstract
{
    interface IEmailService
    {
        void SendEmail(string subject, string messageText, string recipientAddress);
    }
}
