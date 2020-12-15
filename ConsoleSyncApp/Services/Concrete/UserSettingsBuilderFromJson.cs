using ConsoleSyncApp.Models;
using ConsoleSyncApp.Services.Abstract;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
