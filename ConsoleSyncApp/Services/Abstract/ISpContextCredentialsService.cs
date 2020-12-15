using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSyncApp.Services.Abstract
{
    public interface ISpContextCredentialsService
    {
        NetworkCredential SpCredentials {get; set;}
    }
}
