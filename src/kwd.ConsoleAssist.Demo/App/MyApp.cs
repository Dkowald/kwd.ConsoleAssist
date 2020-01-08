using System;
using System.Net.Http;
using System.Threading.Tasks;

using kwd.ConsoleAssist.BasicConsole;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace kwd.ConsoleAssist.Demo.App
{
    public class MyApp
    {
        private readonly AppConfig _cfg;
        private readonly IConsole _con;
        
        private readonly HttpClient _nuget;

        public MyApp(IOptions<AppConfig> cfg, IConsole con,
            IHttpClientFactory clientFactory)
        {
            _cfg = cfg.Value;
            _con = con;

            _nuget = clientFactory.CreateClient("Nuget");
        }

        /// <summary>The default entry method for a cli model object.</summary>
        public int Run()
        {
            var text = _cfg.Verbose == true ? 
                "Hello from Console Assist " : 
                "Hello world";

            Console.WriteLine(text);

            return 0; //exit code
        }

        /// <summary>the 'version' sub command </summary>
        public int Version()
        {
            var version = GetType().Assembly.GetName().Version?.ToString() ?? "?";

            Console.WriteLine($"Version: {version}");

            return 0;
        }

        /// <summary>hand off the work to MySubCommand</summary>
        public MySubCommand Sub1(MySubCommand action) => action;

        /// <summary>Command with variable</summary>
        public int WithArg(string arg1)
        {
            Console.WriteLine($"Hello {arg1}");
            return 0;
        }

        /// <summary>Command with 2 X variable</summary>
        public int WithArg(string arg1, string arg2)
        {
            Console.WriteLine($"Hello {arg1}, {arg2}");
            return 0;
        }

        /// <summary>Add login interactive</summary>
        public MyShell Login(MyShell action) => action;

        /// <summary>Ask nuget for latest package version</summary>
        public async Task Latest()
        {
            //see https://docs.microsoft.com/en-us/nuget/api/registration-base-url-resource
            var url = "https://api.nuget.org/v3" +
                "/registration3" + 
                _cfg.PackageName +
                "/index.json";

            var resp = await _nuget.GetAsync(url);
            
            if (!resp.IsSuccessStatusCode)
            {
                using (TempColor.Red())
                {
                    await _con.Error.WriteLine("Failed http get on nuget", 
                        "Status : " + resp.StatusCode,
                        "Url : " + url);
                    return;
                }
            }

            var body = await resp.Content.ReadAsStringAsync();

            dynamic data = JsonConvert.DeserializeObject<JObject>(body);
            var latestVer = data.items[0].upper;

            using(new TempColor(ConsoleColor.Green))
                await _con.Out.WriteLine("Latest version: " + latestVer);
        }
    }
}