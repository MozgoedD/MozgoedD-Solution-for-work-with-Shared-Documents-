using Microsoft.Extensions.Configuration;
using System;
using ConsoleSyncApp.Classes;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.SharePoint.Client;
using System.Linq;
using System.Security;
using System.IO;
using ConsoleSyncApp.Models;
using System.Collections.Generic;

namespace ConsoleSyncApp
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
                    Thread.Sleep(2 * 60 * 1000);
                }
            });
        }

        static void SyncProcess(Settings settings)
        {
            try
            {
                Console.WriteLine($"Sync process started accrotding to shedule {DateTime.Now}");
                using (var clientContext = new ClientContext(settings.SpSiteUrl))
                {
                    SecureString secPass = new SecureString();
                    foreach (char c in settings.SpAccountPassword.ToCharArray())
                    {
                        secPass.AppendChar(c);
                    }

                    NetworkCredential Credentials = new NetworkCredential(settings.SpAccountLogin, secPass);
                    clientContext.Credentials = Credentials;
                    Web web = clientContext.Web;
                    List sharedDocuments = web.Lists.GetByTitle(settings.SpSiteSharedDocsName);
                    clientContext.Load(sharedDocuments);

                    FileCollection filesCollection = sharedDocuments.RootFolder.Files;
                    clientContext.Load(filesCollection);
                    clientContext.ExecuteQuery();

                    List<AppFileModel> filesInSP = new List<AppFileModel>();
                    List<AppFileModel> filesInDB = new List<AppFileModel>();
                    Console.WriteLine($"Files in SharedDocuments:");

                    foreach (Microsoft.SharePoint.Client.File spFile in filesCollection)
                    {
                        clientContext.Load(spFile);
                        ClientResult<Stream> spFileContent = spFile.OpenBinaryStream();
                        clientContext.ExecuteQuery();
                        Console.WriteLine($"        {spFile.Name}");
                        
                        byte[] fileContent = new byte[spFileContent.Value.Length];
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
                    using (AppDbContext filesContext = new AppDbContext())
                    {
                        filesInDB = filesContext.Files.ToList();
                        foreach (AppFileModel fileInDB in filesInDB)
                        {
                            if (!filesInSP.Exists(f => f.Name == fileInDB.Name))
                            {
                                AppFileModel fileDB = filesContext.Files.Find(fileInDB.Id);
                                filesContext.Files.Remove(fileDB);
                                filesContext.SaveChanges();
                                Console.WriteLine($"{fileInDB.Name} has been deleted!");
                            }
                        }
                        foreach (AppFileModel fileInSP in filesInSP)
                        {
                            if (!filesInDB.Exists(f => f.Name == fileInSP.Name))
                            {
                                filesContext.Files.Add(fileInSP);
                                filesContext.SaveChanges();
                                Console.WriteLine($"{fileInSP.Name} has been added!");
                            }
                            else
                            {
                                AppFileModel fileToUpdate = filesInDB.Where(f => f.Name == fileInSP.Name).FirstOrDefault();
                                if (!fileToUpdate.File.SequenceEqual(fileInSP.File))
                                {
                                    filesContext.Files.Remove(fileToUpdate);
                                    filesContext.Files.Add(fileInSP);
                                    filesContext.SaveChanges();
                                    Console.WriteLine($"{fileInSP.Name} has been updated!");
                                }
                            }
                        }
                    }
                }
                Console.WriteLine($"Sync process completed {DateTime.Now}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
