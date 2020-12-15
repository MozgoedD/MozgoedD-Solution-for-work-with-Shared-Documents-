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
        string SpSiteUrl;
        string SpSiteSharedDocsName;
        public SyncProcedure(ISpContextCredentialsService spContextCredentialsServiceManager, ISyncWithDbRepo syncWithDbRepoManager,
            string SpSiteUrl, string SpSiteSharedDocsName)
        {
            this.spContextCredentialsServiceManager = spContextCredentialsServiceManager;
            this.syncWithDbRepoManager = syncWithDbRepoManager;
            this.SpSiteUrl = SpSiteUrl;
            this.SpSiteSharedDocsName = SpSiteSharedDocsName;
        }

        public void StartSyncProcedure()
        {
            using (var clientContext = new ClientContext(SpSiteUrl))
            {
                clientContext.Credentials = spContextCredentialsServiceManager.SpCredentials;
                var web = clientContext.Web;

                // Getting AppFileModel objects from the SharePoint's Shared Documents
                var filesInSP = getAppFileModelObject();

                syncWithDbRepoManager.UpdateFilesInDb(filesInSP);
                Console.WriteLine($"Sync process completed {DateTime.Now}");
            }
        }

        List<AppFileModel> getAppFileModelObject()
        {
            using (var clientContext = new ClientContext(SpSiteUrl))
            {
                clientContext.Credentials = spContextCredentialsServiceManager.SpCredentials;
                var web = clientContext.Web;
                var sharedDocuments = web.Lists.GetByTitle(SpSiteSharedDocsName);
                var filesCollection = sharedDocuments.RootFolder.Files;
                clientContext.Load(filesCollection);
                clientContext.ExecuteQuery();

                var filesInSP = new List<AppFileModel>();
                Console.WriteLine($"Files in SharedDocuments:");
                foreach (var spFile in filesCollection)
                {
                    clientContext.Load(spFile);
                    Console.WriteLine($"        {spFile.Name}");
                    var spFileContent = spFile.OpenBinaryStream();
                    clientContext.ExecuteQuery();
                    if (spFileContent.Value != null)
                    {
                        var fileContent = new byte[spFileContent.Value.Length];
                        using (BinaryReader br = new BinaryReader(spFileContent.Value))
                        {
                            fileContent = br.ReadBytes((int)spFileContent.Value.Length);
                        }
                        filesInSP.Add(new AppFileModel
                        {
                            Name = spFile.Name,
                            AuthorId = "SharePointAuthorId",
                            File = fileContent
                        });
                    }
                }
                return filesInSP;
            }
        }
    }
}
