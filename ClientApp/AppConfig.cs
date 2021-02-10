using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace ClientApp
{
    public class AppConfig
    {
        public string ClientSecret { get; set; }
        public string TenantId { get; set; }
        public string Instance { get; set; }
        public string BaseUrl { get; set; }
        public string ClientId { get; set; }
        public string ResourceId { get; set; }
        public string Authority { get { return string.Format(CultureInfo.InvariantCulture, Instance, TenantId); } }
        //public AppConfig() { }
        public static AppConfig GetConfig(string path)
        {
            IConfiguration configuration;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path);

            configuration = builder.Build();

            return configuration.Get<AppConfig>();
        }
    }
}
