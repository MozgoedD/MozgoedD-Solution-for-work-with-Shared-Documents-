using ConsoleSyncApp.Services.Abstract;
using ConsoleSyncApp.Services.Concrete;
using Ninject.Modules;
using SharePointDAL.Abstract;
using SharePointDAL.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSyncApp
{
    public class NinjectBindings : NinjectModule
    {
        public override void Load()
        {
            Bind<ISyncWithDbRepo>().To<SyncWithDbRepo>();
            Bind<IUserSettignsBuilder>().To<UserSettingsBuilderFromJson>();
            Bind<ISharedDocsGetter>().To<SharedDocsGetter>();
            Bind<ISpContextCredentialsService>().To<SpContextCredentialsService>();
        }
    }
}
