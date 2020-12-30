using Core.Models;
using DAL.Abstract;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Concrete
{
    public class SharedPointDocumentService : ISharedPointDocumentService
    {
        ISpContextCredentialsService _spContextCredentialsServiceManager;
        string spSiteUrl;
        string spSiteSharedDocsName;

        public SharedPointDocumentService(ISpContextCredentialsService spContextCredentialsServiceManager,
            string spSiteUrl, string spSiteSharedDocsName)
        {
            _spContextCredentialsServiceManager = spContextCredentialsServiceManager;
            this.spSiteUrl = spSiteUrl;
            this.spSiteSharedDocsName = spSiteSharedDocsName;
        }
        public List<AppFileModel> getAppFileModelObject()
        {
            using (var clientContext = new ClientContext(spSiteUrl))
            {
                clientContext.Credentials = _spContextCredentialsServiceManager.SpCredentials;
                var web = clientContext.Web;
                var sharedDocuments = web.Lists.GetByTitle(spSiteSharedDocsName);
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

        public void UploadFileToSP(AppFileModel file)
        {
            try
            {
                using (var clientContext = new ClientContext(spSiteUrl))
                {
                    clientContext.Credentials = _spContextCredentialsServiceManager.SpCredentials;
                    Web web = clientContext.Web;
                    List sharedDocuments = web.Lists.GetByTitle(spSiteSharedDocsName);

                    FileCreationInformation fileCreationInformation = new FileCreationInformation
                    {
                        Url = file.Name,
                        Content = file.File,
                    };

                    var fileToUpload = sharedDocuments.RootFolder.Files.Add(fileCreationInformation);
                    sharedDocuments.Update();
                    clientContext.Load(sharedDocuments);
                    clientContext.ExecuteQuery();

                    Debug.WriteLine($"MESSAGE SP: FileInSp {file.Name} has been Uploaded to SharedDocs!");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"MY MESSAGE: {e.Message}");
            }
        }
        public void DeleteFileFromSP(AppFileModel file)
        {
            try
            {
                using (var clientContext = new ClientContext(spSiteUrl))
                {
                    clientContext.Credentials = _spContextCredentialsServiceManager.SpCredentials;
                    Web web = clientContext.Web;
                    List sharedDocuments = web.Lists.GetByTitle(spSiteSharedDocsName);

                    var fileToDelete = web.GetFileByServerRelativeUrl($"/sites/docspage/{spSiteSharedDocsName}/" + file.Name);
                    clientContext.Load(fileToDelete);

                    fileToDelete.DeleteObject();
                    clientContext.ExecuteQuery();
                    Debug.WriteLine($"MESSAGE SP: FileInSp {file.Name} has been Deleted from SharedDocs!");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"MY MESSAGE: {e.Message} {e}");
            }
        }
    }
}
