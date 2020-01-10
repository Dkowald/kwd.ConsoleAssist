# Overview
 
Unlike other console libraries; this uses a application model the command line.

Switches are handled as normal .NET Configuration. 

Methods on this application model correspond to positional arguments.

Methods can also take string arguments, to consume positional arguments as variables.
 
The model is wrapped in a (roslyn) code-generated class to facilitate execution.

With this approach, you lose the flexability from parsing the 
command line; but gain the simplicity to represent the command line as an App model.
 
 If you need more complex CLI parsing see [[Alternates]].

 ## General features

See [[Usage]] for getting started. 

Uses .NET Generic host with custom IHostService to process command line.

Uses .NET configuration for command line switches.

Uses application class(es) to model the command line.

Application model created using the generic host DI container.

Async friendly.

Supports interactive and batch style console applications.

Uses Roslyn to generate a wrapper class.

## Limitations
Modeling the command line with an application model has some down-sides:

### No flag style switch
Switches are via .NET configuration; which only uses key/value settings.
So bool switches (or flags) like -q for quite not supported.

But since .NET configuration also uses appsettings.json, 
  and environment variables; 
work-around this verbosity by using an environment variable.

### Build error when changing model.
While developing if you change the application model,
the generated wrapper source may become un-compilable.

Simply delete the generated file. It will get re-generated on teh next debug run.

### Cannot have Run with arguments if there are other commands

Consider
```cs
public class MyCmd{
  public int Run(string arg1) => 0;

  public void Other() => 0;
}
``
This causes problems since the first positional argument
could be used as a parameter to run or as sub command Other.

In theory the engine could fall-back to use run if the position argument
doesnt match a sub command.

But that still leaves an interface thats confusing.
And means run will never be called with argument other.