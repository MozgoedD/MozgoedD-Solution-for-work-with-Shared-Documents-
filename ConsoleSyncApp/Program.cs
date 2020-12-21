﻿using Microsoft.Extensions.Configuration;
using System;
using Ninject;
using System.Reflection;
using ConsoleSyncApp.Services.Abstract;
using Ninject.Parameters;
using DAL.SharePoint.Abstract;
using DAL.SharePoint.Concrete;
using SharePointDAL.DataBase.Abstract;

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

            var syncWithDbRepoServiceManager = kernel.Get<ISyncSharedDocsWithDbRepo>();

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
