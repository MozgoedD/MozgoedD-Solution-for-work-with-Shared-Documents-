using ClientWebApp.Services.Abstract;
using ClientWebApp.Services.Concrete;
using DAL.SharePoint.Abstract;
using DAL.SharePoint.Concrete;
using Ninject;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ClientWebApp.Infrastructure
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        private IKernel ninjectKernel;
        public NinjectControllerFactory()
        {
            ninjectKernel = new StandardKernel();
            AddBindings();
        }
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return controllerType == null ? null : (IController)ninjectKernel.Get(controllerType);
        }
        private void AddBindings()
        {
            ninjectKernel.Bind<IEmailService>().To<EmailService>();
            ninjectKernel.Bind<ISharedDocsCDService>().To<SharedDocsCDService>()
                .WithConstructorArgument("SpSiteUrl", ConfigurationManager.AppSettings["SpSiteUrl"])
                .WithConstructorArgument("SpAccountLogin", ConfigurationManager.AppSettings["SpAccountLogin"])
                .WithConstructorArgument("SpAccountPassword", ConfigurationManager.AppSettings["SpAccountPassword"])
                .WithConstructorArgument("SpSiteSharedDocsName", ConfigurationManager.AppSettings["SpSiteSharedDocsName"]);
            ninjectKernel.Bind<ISharePointUserManagerService>().To<SharePointUserManagerService>();
        }
    }
}