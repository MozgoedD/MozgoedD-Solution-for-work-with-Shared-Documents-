using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BLL.Abstract
{
    public interface IEmailService
    {
        void SendEmail(string subject, string messageText, string recipientAddress);
    }
}