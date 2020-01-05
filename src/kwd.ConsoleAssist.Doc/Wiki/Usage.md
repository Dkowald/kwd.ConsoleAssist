# How to use
 
 1. Create a model to represent your Command line.

 2. Use EngineSettings to identify the model and create wrapper class.
   
 3. Use ConfigureCommandLine to add host service to execute the wrapper.

 4. 
 The CLI is describes by a CliApp class structure.
 The CliApp must support only a cut-down public interface.

  CliApp must 
    1. All public methods describe actions on the Cli, they must return void; a Task 
       or a child action handler class.
    2. The special method Run is used as the default action for a class.

# App model
The CLI is modeled as a normal .NET class with the following conventions

  All public methods which follow the patterns are valid commands.
    
    Run is the default action for a sub command (class)
    
     void Run() - default run action.
     void Run(posVar[]) - default run action, consuming remaining posVars.
     
    Actions are methods that take zero-or-more pos-Vars and return void.
    
      void Action(posVar[]) - named action with optional pos vars.
    
    Sub-Commands are methods that return a object.
     If provided their arguments are injected via DI.

     T Cmd(A a) - sub command Cmd which uses an injected A.
     T Cmd() - sub command Cmd just returns a T for use.
  
    posVar[] is 1 or more string arguments, supplied as positional values
    
    
     All methods can have the async version,
      returning a task and taking an optional canceler.
    

    Action method can overload the number of posVars they accept.
       For this, a greedy approach is used; consume as many posVar as can.
    
     It is invalid to have a default action; Run(posVar[]) and any other
        sub-command or action.
     Since then it is not obvious which to call run or sub-command.
     (may want to remove this constraint...)
 
# Usage
 
 Steps to create a MyApp cli.

  -- Create MyApp.Exe cli project.
  -- Create MyApp.Lib to contain cli execution code.
  -- Add kwd.Cli package to get CLI tooling.

  ### Debug build
 Build with DEBUG to setup the CLI app.
 Use a CliConfig with the entry class 

 Configure MyApp to use kwd.Cli with Myapp.Lib main entry class.

 Build MyApp.exe with debug to generate cli actions.

 Debug build generates a support file so release build doesn't have to 
	analyse the CLR types.



     ## Configure for utility console app
         By default methods that return null or void will continue to execute.

         You can over-ride this behaviour by providing a default ExitCode.

         ```
         //set exit code to Zero by default
         EngineSettings.DefaultExitCode = 0;
         ```
