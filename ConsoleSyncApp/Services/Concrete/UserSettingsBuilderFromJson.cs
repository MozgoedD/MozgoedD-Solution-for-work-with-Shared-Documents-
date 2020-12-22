using ConsoleSyncApp.Models;
using ConsoleSyncApp.Services.Abstract;
using Microsoft.Extensions.Configuration;

namespace ConsoleSyncApp.Services.Concrete
{
    public class UserSettingsBuilderFromJson : IUserSettignsBuilder
    {
        IConfigurationRoot configuration;

        public UserSettingsBuilderFromJson(string configPath)
        {
            configuration = new ConfigurationBuilder()
                .AddJsonFile(configPath)
                .Build();
        }

        public Settings GetUserSettings()
        {
            var settings = new Settings();
            configuration.Bind(settings);
            return settings;
        }
    }
}
