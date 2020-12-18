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
            var userSettingsBuilderManager = new UserSettingsBuilderFromJson(configPath);
            var settings = userSettingsBuilderManager.GetUserSettings();

            var spContextCredentialsServiceManager = new SpContextCredentialsService(settings.SpAccountLogin, settings.SpAccountPassword);
            var syncWithDbRepoServiceManager = new SyncWithDbRepo();
            var sharedDocsFilesGetterManager = new SharedDocsFilesGetter(spContextCredentialsServiceManager, settings.SpSiteUrl,
                settings.SpSiteSharedDocsName);

            var spSyncProcedureManager = new SyncProcedure(spContextCredentialsServiceManager, syncWithDbRepoServiceManager,
                sharedDocsFilesGetterManager, settings.SpSiteUrl);

            Console.WriteLine("Program starting...\nPress any button to end");
            StartSync(spSyncProcedureManager);
            Console.ReadKey();
            return 1;
        }

        static async void StartSync(SyncProcedure syncProcedureManager)
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    Console.WriteLine($"New next synchronization procedure according to shedule {DateTime.Now}");
                    syncProcedureManager.StartSyncProcedure();

                    // Waiting in minutes before next synchronization procedure
                    Thread.Sleep(1 * 60000);
                }
            });
        }
    }
}
