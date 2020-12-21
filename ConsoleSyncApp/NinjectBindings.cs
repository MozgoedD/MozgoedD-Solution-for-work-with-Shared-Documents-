using ConsoleSyncApp.Services.Abstract;
using ConsoleSyncApp.Services.Concrete;
using Ninject.Modules;
using DAL.SharePoint.Abstract;
using DAL.SharePoint.Concrete;
using SharePointDAL.DataBase.Abstract;
using SharePointDAL.DataBase.Concrete;

namespace ConsoleSyncApp
{
    public class NinjectBindings : NinjectModule
    {
        public override void Load()
        {
            Bind<ISyncSharedDocsWithDbRepo>().To<SyncSharedDocsWithDbRepo>();
            Bind<IUserSettignsBuilder>().To<UserSettingsBuilderFromJson>();
            Bind<ISharedDocsGetter>().To<SharedDocsGetter>();
            Bind<ISpContextCredentialsService>().To<SpContextCredentialsService>();
        }
    }
}
