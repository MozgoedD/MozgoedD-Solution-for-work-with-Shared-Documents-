using ConsoleSyncApp.Services.Abstract;
using ConsoleSyncApp.Services.Concrete;
using Ninject.Modules;
using DAL.Concrete;
using DAL.Abstract;
using BLL.Abstract;
using BLL.Concrete;

namespace ConsoleSyncApp
{
    public class NinjectBindings : NinjectModule
    {
        public override void Load()
        {
            Bind<IUnitOfWork>().To<UnitOfWork>();
            Bind<ISyncSharedDocsWithDbRepo>().To<SyncSharedDocsWithDbRepo>();
            Bind<IUserSettignsBuilder>().To<UserSettingsBuilderFromJson>();
            Bind<ISharedPointDocumentService>().To<SharedPointDocumentService>();
            Bind<ISpContextCredentialsService>().To<SpContextCredentialsService>();
        }
    }
}
