using ClientWebApp.Models;
using ClientWebApp.Services.Abstract;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;

namespace ClientWebApp.Services.Concrete
{
    public class SharePointUserManagerService : ISharePointUserManagerService
    {
        readonly string SpSiteUrl;
        readonly string SpAccountLogin;
        readonly string SpAccountPassword;
        readonly string SpSiteListName;
        readonly NetworkCredential Credentials;

        public SharePointUserManagerService()
        {
            SpSiteUrl = ConfigurationManager.AppSettings["SpSiteUrl"];
            SpAccountLogin = ConfigurationManager.AppSettings["SpAccountLogin"];
            SpAccountPassword = ConfigurationManager.AppSettings["SpAccountPassword"];
            SpSiteListName = ConfigurationManager.AppSettings["SpSiteListName"];
            Credentials = new NetworkCredential(SpAccountLogin, SpAccountPassword);
        }

        public int AddUserToSpList(AppUser user)
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

        public void ApproveUser(AppUser user)
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

        public void DeleteUser(AppUser user)
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
    }
}