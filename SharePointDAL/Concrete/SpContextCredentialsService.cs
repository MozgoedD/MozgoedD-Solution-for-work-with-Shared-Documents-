using DAL.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Concrete
{
    public class SpContextCredentialsService : ISpContextCredentialsService
    {
        public NetworkCredential SpCredentials { get; set; }

        public SpContextCredentialsService(string spAccountLogin, string spAccountPassword)
        {
            var secPass = new SecureString();
            foreach (char c in spAccountPassword.ToCharArray())
            {
                secPass.AppendChar(c);
            }
            SpCredentials = new NetworkCredential(spAccountLogin, secPass);
        }
    }
}
