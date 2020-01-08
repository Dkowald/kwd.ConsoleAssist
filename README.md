# Overview
 A Library to help create Console applications on .NET Core.

 Unlike other console helper approaches, ConsoleAssist uses a code-first model to describe the command line, 
  leveraging .NET Configuration for most of the command line parsing.

# Quick Use

Create a.NET console app.

```
dotnet new console
```

Create Your app class with a int Run() method.

```c#
namespace myProject.App{
    public class MyApp{
        public int Run(){
          Console.WriteLine("Hellow Console Assist");
          return 0;
	    }
    }
}
```

Create and start .NET Generic host and run
```c#
using kwd.ConsoleAssist;

public static async void Main(string[] args) {

    //The root console app; and the project base namespace.
    var settings = new EngineSettings(typeof(MyApp), "myProject");

    //Build console wrapper.
    #if DEBUG
    //Create in-memory, and generate source file
    var wrapper = settings.BuildDebug();
    #else
    //Use previously generated wrapper.
    var wrapper = settings.Build();
    #endif

    //generic .NET core host.
    var host = Host.CreateDefaultBuilder();
    
    //All the normal host setup bits.
    //host.ConfigureServices();

    //Add console execution engine.
    host.ConfigureCommandLine(wrapper, args);

    //Run as console.
    await host.RunConsoleAsync();
}
```
