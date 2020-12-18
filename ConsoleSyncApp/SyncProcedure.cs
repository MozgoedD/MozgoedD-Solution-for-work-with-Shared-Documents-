using ConsoleSyncApp.Models;
using ConsoleSyncApp.Services.Abstract;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSyncApp
{
    public class SyncProcedure
    {
        ISpContextCredentialsService spContextCredentialsServiceManager;
        ISyncWithDbRepo syncWithDbRepoManager;
        ISharedDocsFilesGetter sharedDocsFilesGetterManager;

        string SpSiteUrl;
        public SyncProcedure(ISpContextCredentialsService spContextCredentialsServiceManager, ISyncWithDbRepo syncWithDbRepoManager,
            ISharedDocsFilesGetter sharedDocsFilesGetterManager, string SpSiteUrl)
        {
            this.spContextCredentialsServiceManager = spContextCredentialsServiceManager;
            this.syncWithDbRepoManager = syncWithDbRepoManager;
            this.sharedDocsFilesGetterManager = sharedDocsFilesGetterManager;
            this.SpSiteUrl = SpSiteUrl;
        }

        public void StartSyncProcedure()
        {
            using (var clientContext = new ClientContext(SpSiteUrl))
            {
                clientContext.Credentials = spContextCredentialsServiceManager.SpCredentials;
                var web = clientContext.Web;

                // Getting AppFileModel objects from the SharePoint's Shared Documents
                var filesInSP = sharedDocsFilesGetterManager.getAppFileModelObject();

                syncWithDbRepoManager.UpdateFilesInDb(filesInSP);
                Console.WriteLine($"Sync process completed {DateTime.Now}");
            }
        }
    }
}
