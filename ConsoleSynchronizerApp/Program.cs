using Microsoft.Extensions.Configuration;
using System;
using ConsoleSynchronizerApp.Classes;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.SharePoint.Client;
using System.Linq;
using System.Security;

namespace ConsoleSynchronizerApp
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Please enter a link to cinfig base");
                return 0;
            }

            string configPath = args[0];

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile(configPath)
                .Build();

            var settings = new Settings();
            configuration.Bind(settings);



            Console.WriteLine("Program starting..\nTo end press any button");

            StartSync(settings);

            Console.ReadKey();

            return 1;
        }

        static async void StartSync(Settings settings)
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    SyncProcess(settings);
                    Thread.Sleep(10 * 60 * 1000);
                }
            });
        }

        static void SyncProcess(Settings settings)
        {
            try
            {
                using (var clientContext = new ClientContext(settings.SpSiteUrl))
                {
                    SecureString secPass = new SecureString();
                    foreach (char c in settings.SpAccountPassword.ToCharArray())
                    {
                        secPass.AppendChar(c);
                    }
                    //CredentialCache = GCCollectionMode = new CredentialCache();

                    NetworkCredential Credentials = new NetworkCredential(settings.SpAccountLogin, secPass);

                    Console.WriteLine($"{settings.SpAccountLogin} {settings.SpAccountPassword} {settings.SpSiteUrl}");

                    clientContext.Credentials = Credentials;
                    Web web = clientContext.Web;
                    List sharedDocuments = web.Lists.GetByTitle(settings.SpSiteSharedDocsName);

                    clientContext.Load(sharedDocuments);
                    clientContext.ExecuteQuery();

                    foreach (File file in sharedDocuments.RootFolder.Files)
                    {
                        clientContext.Load(file);
                        clientContext.ExecuteQuery();
                        Console.WriteLine($"SP file {file.Name}");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }
    }
}
