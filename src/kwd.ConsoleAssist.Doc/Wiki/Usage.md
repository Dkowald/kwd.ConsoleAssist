# Usage overview
 
 Build a console app with .NET Generic host
 the general approach is to 
 1. Create the CLI wrapper.
 2. Create the host;
 3. Register host service(s);
 4. Configure commandline
 4. Execute host via RunConsoleAsync.

Following is a series of steps to introduce how to
use ConsoleAssist to create a demo application.

The final code can be found in the project git repo.

## Basic console app.

**New project**
```console
mkdir Demo
cd Demo
dotnet new console
dotnet add package kwd.ConsoleAssist
```

### Turn on PreserveCompilationContext
Add this to your project; needed for the code generator.
```
<!--needed for run-time compile.-->
<PreserveCompilationContext>true</PreserveCompilationContext>
```

### Create script to call exe
Add this to your project, if you'd like a easy way 
to run the exe during development.
```
<Target Name="createRunScript" AfterTargets="Build">
  <ItemGroup>
    <Line Include="#!/bin/bash" />
    <Line Include="app=./bin/$(Configuration)/$(TargetFramework)/$(AssemblyName).exe" />
    <Line Include="$app $@" />
  </ItemGroup>

  <WriteLinesToFile File="run" Lines="@(Line)" Overwrite="true" />
</Target>
```

**Program.cs**
```cs
using kwd.ConsoleAssist;
namespace Demo
{
  public class Program
  {
    public static async Task Main()
    {
      //Create wrapper
      var settings = new EngineSettings(typeof(MyApp));

      var wrapper = settings.BuildDebug();

      //Create generic host.
      var host = Host.CreateDefaultBuilder();

      //Add console execution engine.
      host.ConfigureCommandLine(wrapper, args);
  
      //Run as console.
      await host.RunConsoleAsync();
    }
  }
}
```

The command line is handled by a Model:

**MyApp.cs**
```cs
using System;
namespace Demo
{
  //The command line model.
  public class MyApp
  {
    /// <summary>The default entry method for a cli model object.</summary>
    public int Run()
    {
      Console.WriteLine("Hello world");
      return 0; //exit code
	}
  }
}
```

```console
./run
Hello world
```

## Add config switch
For command line switches, re-use .NET configuration.

**AppConfig.cs**
```cs
public class AppConfig
{
  public bool? Verbose {get; set;}
}
```

** Program.cs **
```cs
//Add config option to host.
host.ConfigureServices((ctx, svc) =>
{
  svc.AddOptions()
     .Configure<AppConfig>(ctx.Configuration);
};
```

** MyApp.cs **
```cs
public class MyApp
{
  private AppConfig _cfg;

  public MyApp(IOptions<AppConfig> cfg)
  {
    _cfg = cfg;
  }

  public int Run()
  {        
    var text = _cfg.Verbose == true ? 
      "Hello from Console Assist " : 
      "Hello world";

    Console.WriteLine(text);

    return 0;
  }
}
```

```console
./run --Verbose=true
Hello from Console Assist
```

You can use environment variables to set switches
```console
export verbose=true
./run
Hello from Console Assist
```
Note: consider using AddEnvironmentVariables with a per-app prefix.

Or json 

**appsettings.json**
```json
{
  "Verbose": true
}
```

.NET configuration is based on key-value pairs,
so the command line should use __--Verbose=true__

## Add sub command
Model a sub command as a method.

**MyApp.cs**
```cs
public class MyApp
{
  //the 'version' sub command 
  public int Version()
  {
    var version = GetType().Assembly.GetName().Version?.ToString() ?? "?";

    Console.WriteLine($"Version: {version}");

    return 0;
  }
}
```

```console
./run version
Version: 1.0.0.0
```

## Inject a sub command.
A Sub command can return an object for further processing.
This way a more complex app can be broken into smaller classes.

The object is creating usign the generic host DI container.

**MySubCommand.cs**
```cs
public class MySubCommand
{
  private readonly AppConfig _cfg;

  public MySubCommand(IOptions<AppConfig> cfg)
  {
    _cfg = cfg.Value;
  }

  public int Run()
  {
    Console.WriteLine($"Hello from sub action: {nameof(MySubCommand)}");
  }
}
```

**MyApp.cs**
```cs
public class MyApp
{
  /// <summary>hand off the work to MySubCommand</summary>
  public MySubCommand Sub1(MySubCommand action) => action;
}
```

command line actions are case ignorant.

```console
./run sub1
Hello from sub action: MySubCommand
```

## Read positional arguments as variables.
Include a string parameter to capture an argument.

**MyApp.cs**
```cs
public class MyApp
{
  /// <summary>Command with variable</summary>
  public int WithArg(string arg1)
  {
    Console.WriteLine($"Hello {arg1}");
    return 0;
  }
}
```

```console
./run withArg fred
Hello fred
```

## Greedy positional variables
Overload a command method to take extra (optional) positional variables.

```cs
public class MyApp 
{
  public int WithArg(string arg1, string arg2)
  {
    Console.WriteLine($"Hello {arg1}, {arg2}");
    return 0;
  }
}
```

```console
./run Witharg fred smith
Hello fred, smith
```
## Interactive console
Create a interactive console by returning null from command.

Interactive re-runs the model with updated (user provided) arguments.

For switches to be updated, use AddUpdatableCommandLine.

**Program.cs**
```cs
class Program
{
  public static async Task Main(string[] args){

    //Use Updatable command line source.
    host.ConfigureAppConfiguration(x =>
      x.AddUpdatableCommandLine(args, new Dictionary<string, string>
      { 
        //add short-form options as normal.
        { "-v", "Verbose" }
      })
    );
  }
}
```
**AppSession.cs**
```cs
//simple app state
public class AppSession
{
  public string Name { get; set;} = "";
}
```

**Program.cs**
```cs
//Add application service(s)
host.ConfigureServices( (ctx, svc) =>
{
  svc.AddSingleton<AppSession>();
});
```

**MyShell.cs**
```cs
public class MyShell
{
  private readonly AppConfig _cfg;
  private readonly AppSession _session;

  //Use option snapshot to get updated config.
  public MyShell(IOptionsSnapshot<AppConfig> cfg, AppSession session)
  {
    _cfg = cfg.Value;
    _session = session;
  }

  //if void return engine will prompt for user input.
  // input is re-processed as a normal command line.
  public void SetName(string arg)
  {    
    Console.WriteLine($"Hello {arg}");

    if (_cfg.Verbose == true)
      Console.WriteLine($"Replacing old name '{_session.Name}'");

    _session.Name = arg;
  }
}
```

**MyApp.cs**
```cs
public class MyApp
{
  /// <summary>Add login interactive</summary>
  public MyShell Login(MyShell action) => action;
}
```

```console
./run login setname fred
Hello fred
>login setname sue -v=true
Hello sue
Repacing old name fred
> {ctl-c}
```

## Console and more interactive control
There are a couple of support services provided
 for more controll.
 
 Use IConsole (or roll your own) for console interaction.

 Use ICommandLineArguments to control interactive flow.

**AppSession.cs**
```cs
public class AppSession
{
  public string? Password {get; set;}
}
```

**MyShell.cs**
 ```cs
public class MyShell
{
  private readonly AppSession _session;
  private readonly IConsole _console;
  private readonly ICommandLineArguments _args;

  public MyShell(AppSession session,
          ICommandLineArguments args,
          IConsole console)
  {
    _args = _args; 
    _console = console;
  }

  public void SetPwd(CancellationToken cancel)
  {
    //Color me blue
    using var clr = _console.Color(ConsoleColor.Blue);

    //Prompt for secret data.
    _session.Password = _console.Read.PromptSecret("Enter password: ", cancel);

    //in-code input next command (forward to another action).
    _args.SetNext("login");
  }

  //Collect input or finish run.
  public int? Run()
  {
    if(_session.Name == string.Empty)
    {
      Console.WriteLine("Enter: login setName <name>");
      return null;
    }

    if(_session.Password is null){
      _args.SetNext($"login {nameof(SetPwd)}")
      return null;
   }

   Console.WriteLine($"Hello {_session.Name}, your password is set.");

   return 0;
 }
}
 ```

```console
./run login setname fred
Hello fred
>login
Enter password: ***
Hello fred, your password is set.
```

## Use async.
Model methods can return void, int, int?,
Task, Task<int> or Task<int?>.

```console
dotnet add package Microsoft.Extensions.Http
```

**Program.cs**
```cs
class Program
{
  static async Task Main(string[] args)
  {
    host.ConfigureServices((ctx, svc) =>
    {
      //http client factory
      svc.AddHttpClient();
    });
  }
}
```

**MyApp.cs**
```cs
class MyApp
{
  private readonly HttpClient _nuget;

  public MyApp(IOptions<AppConfig> cfg, IConsole con, 
            ICommandLineArguments cmdLine,
            IHttpClientFactory clientFactory)
  {
    _nuget = clientFactory.CreateClient("NuGet");
  }

  /// <summary>Ask nuget for latest package version</summary>
  public async Task Latest()
  {
    var url = "https://api.nuget.org/v3" +
              "/registration3" +
              _cfg.PackageName +
              "/index.json";

    var resp = await _nuget.GetAsync(url);
    if (!resp.IsSuccessStatusCode)
      { throw new Exception("Cannot find apckage on nuget"); }
    
    var body = await resp.Content.ReadAsStringAsync();

    dynamic data = JsonConvert.DeserializeObject<JObject>(body);
    var latestVer = data.items[0].upper;
    await _con.Out.WriteLine("Latest version:" + latesVer);
  }
}
```

**AppConfig.cs**
```cs
public class AppConfig{
  public string? PackageName { get; set; }
}
```

**appsettings.json**
```json
{
  "PackageName": "consoleassist",
}
```

```console
./run latest
Latest Version: X.X.X
```

## Use generated wrapper for release
EngineSettings will return either an in-memory wrapper, or the 
generated type. 
For non-debug build, swap to the generated file.

** Program.cs **
```cs
#if DEBUG
var wrapper = settings.BuildDebug();
#else
var wrapper = settings.Build();
#endif
```
