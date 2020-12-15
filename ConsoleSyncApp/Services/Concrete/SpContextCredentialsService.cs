using ConsoleSyncApp.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSyncApp.Services.Concrete
{
    class SpContextCredentialsService : ISpContextCredentialsService
    {
        public NetworkCredential SpCredentials { get; set; }

        public SpContextCredentialsService(string SpAccountLogin, string SpAccountPassword)
        {
            var secPass = new SecureString();
            foreach (char c in SpAccountPassword.ToCharArray())
            {
                secPass.AppendChar(c);
            }
            SpCredentials = new NetworkCredential(SpAccountLogin, secPass);
        }
    }
}
