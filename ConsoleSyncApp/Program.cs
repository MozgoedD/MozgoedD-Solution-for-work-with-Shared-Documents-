using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.SharePoint.Client;
using System.Linq;
using System.Security;
using System.IO;
using ConsoleSyncApp.Models;
using System.Collections.Generic;
using ConsoleSyncApp.Services.Concrete;

namespace ConsoleSyncApp
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Please enter a link to the config file");
                return 0;
            }
            var configPath = args[0];
            var userSettingsBuilderManager = new UserSettingsBuilderFromJson(configPath);
            var settings = userSettingsBuilderManager.GetUserSettings();
            var spContextCredentialsServiceManager = new SpContextCredentialsService(settings.SpAccountLogin, settings.SpAccountPassword);
            var syncWithDbRepoServiceManager = new SyncWithDbRepo();
            var spSyncProcedureManager = new SyncProcedure(spContextCredentialsServiceManager, syncWithDbRepoServiceManager,
                settings.SpSiteUrl, settings.SpSiteSharedDocsName);

            Console.WriteLine("Program starting...\nPress any button to end");
            StartSync(spSyncProcedureManager);
            Console.ReadKey();
            return 1;
        }

        static async void StartSync(SyncProcedure syncProcedureManager)
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    Console.WriteLine($"New next synchronization procedure according to shedule {DateTime.Now}");
                    syncProcedureManager.StartSyncProcedure();

                    // Waiting in minutes before next synchronization procedure
                    Thread.Sleep(1 * 60000);
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
                                Console.WriteLine($"{fileInDB.Name} has been deleted from mvc app database!");
                            }
                        }
                        foreach (AppFileModel fileInSP in filesInSP)
                        {
                            if (!filesInDB.Exists(f => f.Name == fileInSP.Name))
                            {
                                filesContext.Files.Add(fileInSP);
                                filesContext.SaveChanges();
                                Console.WriteLine($"{fileInSP.Name} has been added to mvc app database!");
                            }
                            else
                            {
                                AppFileModel fileToUpdate = filesInDB.Where(f => f.Name == fileInSP.Name).FirstOrDefault();
                                if (!fileToUpdate.File.SequenceEqual(fileInSP.File))
                                {
                                    filesContext.Files.Remove(fileToUpdate);
                                    filesContext.Files.Add(fileInSP);
                                    filesContext.SaveChanges();
                                    Console.WriteLine($"{fileInSP.Name} has been updated in mvc app database!");
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
