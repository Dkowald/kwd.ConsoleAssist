# Overview
 A code-first style Command Line Interface (CLI) approach for .NET .

 Using a convention compliant app class (or set of classes); 
  build a not-too-bad  CLI.

 Unlike other libraries, this focuses on converting a class to a command line.
 To do this, it is restrictive on what can and cannot be done.

 If you need more complex CLI parsing see [[Alternates]].

## Usage outline

 This library leverages the .NET core generic host. 
 So your console app leverages an IoC container.

 Create a EngineSettings object to describe your App model.

 Call EngineSettings BuildDebug to generate a wrapper around your App model.
 This also compiles the wrapper and loads it into an in-memory assembly.

 For release builds, use EngineSettings Build() method to return the 
 previously generated wrapper.
 
 Use ConfigureCommandLine to register a Host service that will execute the 
 wrapper.
  

# Project setup.
The generated file should be included in the project,
but is not needed for a non-DEBUG build.

```
  <ItemGroup>
    <Compile Condition="'$(Configuration)' != 'Debug'"
      Remove="TestHelpers\DemoCli.cli.cs" />
    <None Condition="'$(Configuration)' != 'Debug'"
             Remove="TestHelpers\DemoCli.cli.cs" />
  </ItemGroup>
```

