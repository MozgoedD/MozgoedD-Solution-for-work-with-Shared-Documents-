using BLL.Abstract;
using ConsoleSyncApp.Services.Abstract;
using DAL.Abstract;
using Ninject;
using Ninject.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSyncApp
{
    public class NinjectInitializer
    {
        public SyncProcedure GetSyncProcedure(string configPath)
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());

            var userSettingsBuilderManager = kernel.Get<IUserSettignsBuilder>(
                new ConstructorArgument("configPath", configPath));
            var settings = userSettingsBuilderManager.GetUserSettings();

            var spContextCredentialsServiceManager = kernel.Get<ISpContextCredentialsService>(
                new ConstructorArgument("spAccountLogin", settings.SpAccountLogin),
                new ConstructorArgument("spAccountPassword", settings.SpAccountPassword));

            var syncWithDbRepoServiceManager = kernel.Get<ISyncSharedDocsWithDbRepo>(
                new ConstructorArgument("unitOfWork", kernel.Get<IUnitOfWork>()));

            var sharedDocsFilesGetterManager = kernel.Get<ISharedPointDocumentService>(
                new ConstructorArgument("spContextCredentialsServiceManager", spContextCredentialsServiceManager),
                new ConstructorArgument("spSiteUrl", settings.SpSiteUrl),
                new ConstructorArgument("spSiteSharedDocsName", settings.SpSiteSharedDocsName));

            return new SyncProcedure(spContextCredentialsServiceManager, syncWithDbRepoServiceManager,
                sharedDocsFilesGetterManager, settings.SpSiteUrl);
        }
    }
}
