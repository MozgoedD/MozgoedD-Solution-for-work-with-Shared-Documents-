using SharePointDAL.Abstract;
using SharePointDAL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.SharePoint.Client;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointDAL.Concrete
{
    public class SharedDocsGetter : ISharedDocsGetter
    {
        ISpContextCredentialsService _spContextCredentialsServiceManager;
        string SpSiteUrl;
        string SpSiteSharedDocsName;

        public SharedDocsGetter(ISpContextCredentialsService spContextCredentialsServiceManager,
            string SpSiteUrl, string SpSiteSharedDocsName)
        {
            _spContextCredentialsServiceManager = spContextCredentialsServiceManager;
            this.SpSiteUrl = SpSiteUrl;
            this.SpSiteSharedDocsName = SpSiteSharedDocsName;
        }

        public List<AppFileModel> getAppFileModelObject()
        {
            using (var clientContext = new ClientContext(SpSiteUrl))
            {
                clientContext.Credentials = _spContextCredentialsServiceManager.SpCredentials;
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
