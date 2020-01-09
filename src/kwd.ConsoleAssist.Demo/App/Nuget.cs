using System;
using System.Net.Http;
using System.Threading.Tasks;
using kwd.ConsoleAssist.BasicConsole;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace kwd.ConsoleAssist.Demo.App
{
    public class NuGet
    {
        private readonly AppConfig _cfg;
        private readonly IConsole _console;
        private readonly HttpClient _nuget;

        public NuGet(IOptions<AppConfig> cfg, IConsole console, 
            IHttpClientFactory clientFactory)
        {
            _cfg = cfg.Value;
            _console = console;
            _nuget = clientFactory.CreateClient(nameof(NuGet));
        }

        /// <summary>Ask nuget for latest package version</summary>
        public async Task<int> Latest(string package)
        {
            //see https://docs.microsoft.com/en-us/nuget/api/registration-base-url-resource
            var url = "https://api.nuget.org/v3" +
                      "/registration3/" + package.ToLower() + "/index.json";

            var resp = await _nuget.GetAsync(url);
            
            if (!resp.IsSuccessStatusCode)
            {
                using (TempColor.Red())
                {
                    await _console.Error.WriteLine("Failed http get on nuget", 
                        "Status : " + resp.StatusCode,
                        "Url : " + url);
                    return 500;
                }
            }

            var body = await resp.Content.ReadAsStringAsync();

            dynamic data = JsonConvert.DeserializeObject<JObject>(body);
            var latestVer = data.items[0].upper;

            using(new TempColor(ConsoleColor.Green))
                await _console.Out.WriteLine(
                    $"Package '{package}'",
                    "Latest version: " + latestVer);

            return 0;
        }

        /// <summary>Ask nuget for latest package version
        /// (use configured package name)</summary>
        public async Task<int> Latest()
        {
            var package = _cfg.PackageName ?? 
                          throw new Exception("");

            return await Latest(package);
        }
    }
}