using Microsoft.SharePoint.Client;
using DAL.SharePoint.Abstract;
using System;
using System.Threading;
using System.Threading.Tasks;
using SharePointDAL.DataBase.Abstract;

namespace ConsoleSyncApp
{
    public class SyncProcedure
    {
        ISpContextCredentialsService _spContextCredentialsServiceManager;
        ISyncSharedDocsWithDbRepo _syncWithDbRepoManager;
        ISharedDocsGetter _sharedDocsGetter;

        string SpSiteUrl;
        public SyncProcedure(ISpContextCredentialsService spContextCredentialsServiceManager, ISyncSharedDocsWithDbRepo syncWithDbRepoManager,
            ISharedDocsGetter sharedDocsFilesGetterManager, string SpSiteUrl)
        {
            _spContextCredentialsServiceManager = spContextCredentialsServiceManager;
            _syncWithDbRepoManager = syncWithDbRepoManager;
            _sharedDocsGetter = sharedDocsFilesGetterManager;
            this.SpSiteUrl = SpSiteUrl;
        }

        public async void StartSync()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    Console.WriteLine($"New next synchronization procedure according to shedule {DateTime.Now}");
                    StartSyncProcedure();

                    // Waiting in minutes before next synchronization procedure
                    Thread.Sleep(1 * 60000);
                }
            });
        }

        void StartSyncProcedure()
        {
            using (var clientContext = new ClientContext(SpSiteUrl))
            {
                clientContext.Credentials = _spContextCredentialsServiceManager.SpCredentials;
                var web = clientContext.Web;

                // Getting AppFileModel objects from the SharePoint's Shared Documents
                var filesInSP = _sharedDocsGetter.getAppFileModelObject();

                _syncWithDbRepoManager.UpdateFilesInDb(filesInSP);
                Console.WriteLine($"Sync process completed {DateTime.Now}");
            }
        }
    }
}
