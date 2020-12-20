using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.SharePoint.Client;
using System.Linq;
using System.Security;
using System.IO;
using ConsoleSyncApp.Models;
using System.Collections.Generic;
using ConsoleSyncApp.Services.Concrete;
using Ninject;
using System.Reflection;
using ConsoleSyncApp.Services.Abstract;
using Ninject.Parameters;
using SharePointDAL.Abstract;
using SharePointDAL.Concrete;

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
                new ConstructorArgument("SpAccountLogin", settings.SpAccountLogin),
                new ConstructorArgument("SpAccountPassword", settings.SpAccountPassword));

            var syncWithDbRepoServiceManager = kernel.Get<ISyncWithDbRepo>();

            var sharedDocsFilesGetterManager = kernel.Get<SharedDocsGetter>(
                new ConstructorArgument("spContextCredentialsServiceManager", spContextCredentialsServiceManager),
                new ConstructorArgument("SpSiteUrl", settings.SpSiteUrl),
                new ConstructorArgument("SpSiteSharedDocsName", settings.SpSiteSharedDocsName));

            var spSyncProcedureManager = new SyncProcedure(spContextCredentialsServiceManager, syncWithDbRepoServiceManager,
                sharedDocsFilesGetterManager, settings.SpSiteUrl);

            Console.WriteLine("Program starting...\nPress any button to end");
            spSyncProcedureManager.StartSync();
            Console.ReadKey();
            return 1;
        }
    }
}
