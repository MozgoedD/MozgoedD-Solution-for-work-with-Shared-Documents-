//using ClientWebApp.Models;
//using ClientWebApp.Services.Abstract;
//using DAL.Models;
//using Microsoft.SharePoint.Client;
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Diagnostics;
//using System.Linq;
//using System.Net;
//using System.Web;

//namespace ClientWebApp.Services.Concrete
//{
//    public class SharePointSharedDocsManagerService : ISharePointSharedDocsManagerService
//    {
//        readonly string SpSiteUrl;
//        readonly string SpAccountLogin;
//        readonly string SpAccountPassword;
//        readonly string SpSiteSharedDocsName;
//        readonly NetworkCredential Credentials;

//        public SharePointSharedDocsManagerService()
//        {
//            SpSiteUrl = ConfigurationManager.AppSettings["SpSiteUrl"];
//            SpAccountLogin = ConfigurationManager.AppSettings["SpAccountLogin"];
//            SpAccountPassword = ConfigurationManager.AppSettings["SpAccountPassword"];
//            SpSiteSharedDocsName = ConfigurationManager.AppSettings["SpSiteSharedDocsName"];
//            Credentials = new NetworkCredential(SpAccountLogin, SpAccountPassword);
//        }
//        public void UploadFileToSP(AppFileModel file)
//        {
//            try
//            {
//                using (var clientContext = new ClientContext(SpSiteUrl))
//                {
//                    clientContext.Credentials = Credentials;
//                    Web web = clientContext.Web;
//                    List sharedDocuments = web.Lists.GetByTitle(SpSiteSharedDocsName);

//                    FileCreationInformation fileCreationInformation = new FileCreationInformation
//                    {
//                        Url = file.Name,
//                        Content = file.File,
//                    };

//                    File fileToUpload = sharedDocuments.RootFolder.Files.Add(fileCreationInformation);
//                    sharedDocuments.Update();
//                    clientContext.Load(sharedDocuments);
//                    clientContext.ExecuteQuery();

//                    Debug.WriteLine($"MESSAGE SP: FileInSp {file.Name} has been Uploaded to SharedDocs!");
//                }
//            }
//            catch (Exception e)
//            {
//                Debug.WriteLine($"MY MESSAGE: {e.Message}");
//            }
//        }
//        public void DeleteFileFromSP(AppFileModel file)
//        {
//            try
//            {
//                using (var clientContext = new ClientContext(SpSiteUrl))
//                {
//                    clientContext.Credentials = Credentials;
//                    Web web = clientContext.Web;
//                    List sharedDocuments = web.Lists.GetByTitle(SpSiteSharedDocsName);

//                    File fileToDelete = web.GetFileByServerRelativeUrl($"/sites/docspage/{SpSiteSharedDocsName}/" + file.Name);
//                    clientContext.Load(fileToDelete);

//                    fileToDelete.DeleteObject();
//                    clientContext.ExecuteQuery();
//                    Debug.WriteLine($"MESSAGE SP: FileInSp {file.Name} has been Deleted from SharedDocs!");
//                }
//            }
//            catch (Exception e)
//            {
//                Debug.WriteLine($"MY MESSAGE: {e.Message} {e}");
//            }
//        }
//    }
//}