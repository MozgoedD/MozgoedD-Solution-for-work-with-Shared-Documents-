using Core.Models;
using DAL.Abstract;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Concrete
{
    public class SharePointUserService : ISharePointUserService
    {
        private readonly ISpContextCredentialsService _spContextCredentialsServiceManager;
        private readonly string spSiteUrl;
        private readonly string spSiteListName;

        public SharePointUserService(ISpContextCredentialsService spContextCredentialsServiceManager,
            string spSiteUrl, string spSiteListName)
        {
            _spContextCredentialsServiceManager = spContextCredentialsServiceManager;
            this.spSiteUrl = spSiteUrl;
            this.spSiteListName = spSiteListName;
        }

        public int AddUserToSpList(AppUser user)
        {
            try
            {
                using (var clientContext = new ClientContext(spSiteUrl))
                {
                    clientContext.Credentials = _spContextCredentialsServiceManager.SpCredentials;
                    Web web = clientContext.Web;
                    List usersList = web.Lists.GetByTitle(spSiteListName);
                    ListItemCreationInformation listItemCreationInformation = new ListItemCreationInformation();
                    ListItem userToAdd = usersList.AddItem(listItemCreationInformation);
                    userToAdd["FirstName"] = user.FirstName;
                    userToAdd["SecondName"] = user.SecondName;
                    userToAdd["Patronymic"] = user.Patronymic;
                    userToAdd["Email"] = user.Email;
                    userToAdd["DOB"] = user.DOB.ToString();
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
                using (var clientContext = new ClientContext(spSiteUrl))
                {
                    clientContext.Credentials = _spContextCredentialsServiceManager.SpCredentials;
                    Web web = clientContext.Web;
                    List usersList = web.Lists.GetByTitle(spSiteListName);
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
                using (var clientContext = new ClientContext(spSiteUrl))
                {
                    clientContext.Credentials = _spContextCredentialsServiceManager.SpCredentials;
                    Web web = clientContext.Web;
                    List usersList = web.Lists.GetByTitle(spSiteListName);
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
