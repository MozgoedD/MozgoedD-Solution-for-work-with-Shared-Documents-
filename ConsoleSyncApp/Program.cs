using System;

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

            var ninjectInit = new NinjectInitializer();
            var spSyncProcedureManager = ninjectInit.GetSyncProcedure(configPath);

            Console.WriteLine("Program starting...\nPress any button to end");
            spSyncProcedureManager.StartSync();
            Console.ReadKey();
            return 1;
        }
    }
}
