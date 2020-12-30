using BLL.Abstract;
using BLL.Concrete;
using DAL.Abstract;
using DAL.Concrete;
using Ninject;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Configuration;
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
            // todo: check constructor args!!! Spec for email!
            ninjectKernel.Bind<ISpContextCredentialsService>().To<SpContextCredentialsService>()
                .WithConstructorArgument("spAccountLogin", ConfigurationManager.AppSettings["SpAccountLogin"])
                .WithConstructorArgument("spAccountPassword", ConfigurationManager.AppSettings["SpAccountPassword"]);

            ninjectKernel.Bind<ISharedPointDocumentService>().To<SharedPointDocumentService>()
                .WithConstructorArgument("spContextCredentialsServiceManager", ninjectKernel.Get<ISpContextCredentialsService>())
                .WithConstructorArgument("spSiteUrl", ConfigurationManager.AppSettings["SpSiteUrl"])
                .WithConstructorArgument("spSiteSharedDocsName", ConfigurationManager.AppSettings["SpSiteSharedDocsName"]);

            ninjectKernel.Bind<IEmailService>().To<EmailService>()
                .WithConstructorArgument("smtpMailSection", (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp"));

            ninjectKernel.Bind<ISharePointUserService>().To<SharePointUserService>()
                .WithConstructorArgument("spContextCredentialsServiceManager", ninjectKernel.Get<ISpContextCredentialsService>())
                .WithConstructorArgument("spSiteUrl", ConfigurationManager.AppSettings["SpSiteUrl"])
                .WithConstructorArgument("spSiteListName", ConfigurationManager.AppSettings["SpSiteListName"]);

            ninjectKernel.Bind<IIdentityManager>().To<IdentityManager>();

            ninjectKernel.Bind<IUnitOfWork>().To<UnitOfWork>();

            // Controller Bindings
            ninjectKernel.Bind<IAdminService>().To<AdminService>()
                .WithConstructorArgument("emailManager", ninjectKernel.Get<IEmailService>())
                .WithConstructorArgument("spUserManager", ninjectKernel.Get<ISharePointUserService>())
                .WithConstructorArgument("identityManager", ninjectKernel.Get<IIdentityManager>());

            ninjectKernel.Bind<ILoginService>().To<LoginService>()
                .WithConstructorArgument("spUserManager", ninjectKernel.Get<ISharePointUserService>())
                .WithConstructorArgument("identityManager", ninjectKernel.Get<IIdentityManager>());

            ninjectKernel.Bind<IDocsPageService>().To<DocsPageService>()
                .WithConstructorArgument("identityManager", ninjectKernel.Get<IIdentityManager>())
                .WithConstructorArgument("unitOfWork", ninjectKernel.Get<IUnitOfWork>())
                .WithConstructorArgument("spFileManager", ninjectKernel.Get<ISharedPointDocumentService>());
        }
    }
}