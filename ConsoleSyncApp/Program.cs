using Microsoft.Extensions.Configuration;
using System;
using Ninject;
using System.Reflection;
using ConsoleSyncApp.Services.Abstract;
using Ninject.Parameters;
using DAL.Abstract;
using BLL.Abstract;

namespace ConsoleSyncApp
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Please enter a link to the config file");
                return 0;
            }
            var configPath = args[0];

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

            var spSyncProcedureManager = new SyncProcedure(spContextCredentialsServiceManager, syncWithDbRepoServiceManager,
                sharedDocsFilesGetterManager, settings.SpSiteUrl);

            Console.WriteLine("Program starting...\nPress any button to end");
            spSyncProcedureManager.StartSync();
            Console.ReadKey();
            return 1;
        }
    }
}
