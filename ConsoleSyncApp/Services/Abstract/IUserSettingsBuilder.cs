using ConsoleSyncApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSyncApp.Services.Abstract
{
    public interface IUserSettignsBuilder
    {
        Settings GetUserSettings();
    }
}
