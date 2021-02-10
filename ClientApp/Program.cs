using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //IdentityModelEventSource.ShowPII = true
            AppConfig config = AppConfig.GetConfig("appsettings.json");
            Console.WriteLine($"Authority: {config.Authority}");
            RunAsync().GetAwaiter().GetResult();
            Console.ReadLine();
        }

        private static async Task RunAsync()
        {
            AppConfig config = AppConfig.GetConfig("appsettings.json");
            IConfidentialClientApplication app;
            app = ConfidentialClientApplicationBuilder.Create(config.ClientId)
                .WithClientSecret(config.ClientSecret)
                .WithAuthority(new Uri(config.Authority))
                .Build();

            string[] resourceIds = new string[] { config.ResourceId };
            AuthenticationResult result = null;

            try
            {
                result = await app.AcquireTokenForClient(resourceIds).ExecuteAsync();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Token Acquired\n");
                Console.WriteLine(result.AccessToken);
                Console.ResetColor();
            }
            catch(MsalClientException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }

            if (!string.IsNullOrEmpty(result.AccessToken))
            {
                var httpsClient = new HttpClient();
                var defaultRequestheaders = httpsClient.DefaultRequestHeaders;

                if(defaultRequestheaders.Accept == null || !defaultRequestheaders.Accept.Any(m => m.MediaType == "application/json"))
                {
                    httpsClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                }

                defaultRequestheaders.Authorization =
                    new AuthenticationHeaderValue("bearer", result.AccessToken);

                HttpResponseMessage response = await httpsClient.GetAsync(config.BaseUrl);
                Console.WriteLine("Is Success? "+response.IsSuccessStatusCode);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(json);
                    Console.ReadKey();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Failed to call API: {response.StatusCode}");
                    string content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Result: {content}");
                    Console.ResetColor();
                }

            }
        }
    }
}
