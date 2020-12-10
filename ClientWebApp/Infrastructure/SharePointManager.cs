using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Microsoft.SharePoint.Client;
using System.Net;
using ClientWebApp.Models;
using System.Diagnostics;

namespace ClientWebApp.Infrastructure
{
    public static class SharePointManager
    {
        static readonly string SpSiteUrl;
        static readonly string SpAccountLogin;
        static readonly string SpAccountPassword;
        static readonly string SpSiteListName;
        static readonly string SpSiteSharedDocsName;
        static readonly NetworkCredential Credentials;

        static SharePointManager()
        {
            SpSiteUrl = ConfigurationManager.AppSettings["SpSiteUrl"];
            SpAccountLogin = ConfigurationManager.AppSettings["SpAccountLogin"];
            SpAccountPassword = ConfigurationManager.AppSettings["SpAccountPassword"];
            SpSiteListName = ConfigurationManager.AppSettings["SpSiteListName"];
            SpSiteSharedDocsName = ConfigurationManager.AppSettings["SpSiteSharedDocsName"];
            Credentials = new NetworkCredential(SpAccountLogin, SpAccountPassword);
        }

        public static int AddUserToSpList(AppUser user)
        {
            try
            {
                using (var clientContext = new ClientContext(SpSiteUrl))
                {
                    clientContext.Credentials = Credentials;
                    Web web = clientContext.Web;
                    List usersList = web.Lists.GetByTitle(SpSiteListName);
                    ListItemCreationInformation listItemCreationInformation = new ListItemCreationInformation();
                    ListItem userToAdd = usersList.AddItem(listItemCreationInformation);
                    userToAdd["FirstName"] = user.FirstName;
                    userToAdd["SecondName"] = user.SecondName;
                    userToAdd["Patronymic"] = user.Patronymic;
                    userToAdd["Email"] = user.Email;
                    userToAdd["DOB"] = user.DOB;
                    userToAdd["Gender"] = user.Gender.ToString();
                    userToAdd["Workplace"] = user.Workplace;
                    userToAdd["JobPosition"] = user.JobPosition;
                    userToAdd["Country"] = user.Country;
                    userToAdd["City"] = user.City;
                    userToAdd["IsApproved"] = false;

                    userToAdd.Update();
                    clientContext.Load(userToAdd);
                    clientContext.ExecuteQuery();
                    return userToAdd.Id;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"MY MESSAGE: {e.Message}");
            }
            return -1;
        }

        public static void ApproveUser(AppUser user)
        {
            try
            {
                using (var clientContext = new ClientContext(SpSiteUrl))
                {
                    clientContext.Credentials = Credentials;
                    Web web = clientContext.Web;
                    List usersList = web.Lists.GetByTitle(SpSiteListName);
                    ListItem userToApprove = usersList.GetItemById(user.SharePointId);
                    userToApprove["IsApproved"] = true;
                    userToApprove.Update();
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"MY MESSAGE: {e.Message}");
            }
        }

        public static void DeleteUser(AppUser user)
        {
            try
            {
                using (var clientContext = new ClientContext(SpSiteUrl))
                {
                    clientContext.Credentials = Credentials;
                    Web web = clientContext.Web;
                    List usersList = web.Lists.GetByTitle(SpSiteListName);
                    ListItem userToDelete = usersList.GetItemById(user.SharePointId);
                    userToDelete.DeleteObject();
                    userToDelete.Update();
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"MY MESSAGE: {e.Message}");
            }
        }

        public static void UploadFileToSP(AppFileModel file)
        {
            try
            {
                using (var clientContext = new ClientContext(SpSiteUrl))
                {
                    clientContext.Credentials = Credentials;
                    Web web = clientContext.Web;
                    List sharedDocuments = web.Lists.GetByTitle(SpSiteSharedDocsName);

                    FileCreationInformation fileCreationInformation = new FileCreationInformation
                    {
                        Url = file.Name,
                        Content = file.File,
                    };

                    File fileToUpload = sharedDocuments.RootFolder.Files.Add(fileCreationInformation);
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

        public static void DeleteFileFromSP(AppFileModel file)
        {
            try
            {
                using (var clientContext = new ClientContext(SpSiteUrl))
                {
                    clientContext.Credentials = Credentials;
                    Web web = clientContext.Web;
                    List sharedDocuments = web.Lists.GetByTitle(SpSiteSharedDocsName);

                    File fileToDelete = web.GetFileByServerRelativeUrl($"/sites/docspage/{SpSiteSharedDocsName}/" + file.Name);
                    clientContext.Load(fileToDelete);

                    fileToDelete.DeleteObject();
                    clientContext.ExecuteQuery();
                    Debug.WriteLine($"MESSAGE SP: FileInSp {file.Name} has been Deleted from SharedDocs!");

                    //clientContext.Load(sharedDocuments.RootFolder.Files.Where(f => f.Name == file.Name).FirstOrDefault());
                    //clientContext.ExecuteQuery();

                    //File fileToDelete = sharedDocuments.RootFolder.Files.Where(f => f.Name == file.Name).FirstOrDefault();

                    //fileToDelete.DeleteObject();
                    //sharedDocuments.Update();
                    //clientContext.Load(sharedDocuments);
                    //clientContext.ExecuteQuery();
                    //Debug.WriteLine($"MESSAGE SP: FileInSp {fileToDelete.Name} Deleted!");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"MY MESSAGE: {e.Message} {e}");
            }
        }

    }
}