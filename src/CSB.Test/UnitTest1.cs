using System;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using System.IO;
namespace CSB.Test
{
    public class UnitTest1
    {


        private void ConsumeConfiguration(IConfiguration configuration, bool clone)
        {
             Assert.Equal("myItem1", configuration["Item1"]);
             Assert.Equal("myItem2", configuration["Item2"]);

            if (clone)
            {
                //same code like class WebHostBuilder /useSettings
                //copied from https://github.com/aspnet/Hosting/blob/dev/src/Microsoft.AspNetCore.Hosting/WebHostBuilder.cs
                IConfiguration builder = new ConfigurationBuilder().AddEnvironmentVariables(prefix: "XXXXXXXXX_").Build();


                //same code like in 
                //https://github.com/aspnet/Hosting/blob/master/src/Microsoft.AspNetCore.Hosting.Abstractions/HostingAbstractionsWebHostBuilderExtensions.cs
                //Method: public static IWebHostBuilder UseConfiguration(this IWebHostBuilder hostBuilder, IConfiguration configuration) 

                foreach (var setting in configuration.AsEnumerable())
                {
                    //hostBuilder.UseSetting(setting.Key, setting.Value); can be tranlsated into:
                    builder[setting.Key] = setting.Value;
                }

                ConsumeConfiguration(builder, clone: false);
            }
        }

        [Fact]
        public void Json_SimpleAppSettings()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("simpleappsettings.json", optional: false);

            IConfiguration config =  builder.Build();
            ConsumeConfiguration(config,clone:true);
        }
        [Fact]
        public void Json_AppSettings_With_Sections()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettingsWithSection.json", optional: false);

            IConfiguration config = builder.Build();
            IConfiguration section = config.GetSection("SectionA");
            int i = System.Linq.Enumerable.Count(section.GetChildren());
            ConsumeConfiguration(section, clone:true);
        }




    }
}
