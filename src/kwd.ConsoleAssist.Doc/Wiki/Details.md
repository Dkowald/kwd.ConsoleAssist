## Details
 There is a single entry point class used to describe the command line 
   action's and arguments.
 In MyApp.Cli configure kwd.Cli.CliConfig to use this type.

 Each valid action returns a simple response reporing COUT and CERROR text.
 If the action fails it throws an exception.
 The exception is caught and mapped into an output with error code.

### Use console for output.
 The libary includes a simple *IConsole* that you can use.
 But you can swap this out to suite.

### no positional arguments
  Arguments / Options / Switches are all arguments
  starting with -- or - (where there is a short form).
  No positional arguments, so the order doesnt matter.
  
### all arguments have a value.
 The ':' is used to seperate a parameter option from its value.
 If there is no ':' or it is just space after the ':'
 Then the code must provide the default value to be used.

### action parameters mid cli.
 This cli processor allows for actions to get arguments
  in the middle of the command line.
 e.g: myApp list -f where -s:fred*
 Is read as:
  Call the List method with argument f true
  On the result call the Where action with argument s == fred

### The long and the short.
 All derived options are the long version -- .
 Use an option alias to define the sort version.
  short version alias' are global to the app; so cannot give different meaning to them
  for different actions.
 https://www.quora.com/What-is-the-difference-between-and-command-line-options

### No support for the bare-double
 Some cli's use a 
 [bare-double](https://unix.stackexchange.com/questions/11376/what-does-double-dash-mean-also-known-as-bare-double-dash)
 to indicate end of options.

 This CLI builder Does NOT use that format.

 There is no support for defining a variable value without the variable.
 So forms like myApp -v -- other-bits doesnt exist. 
 
 **todo: revisit**

### No space in arguments.
 Some cli's use a space between option and value to 
  define the value of an option.

 This CLI Does NOT use that format.
 For simplicity, the option and its value must be joind with a '**:**' delimiter.
 
### Spacy variables
 Not (directly) supported.
 If the option argument has a space in it; use a [percent-encoded](https://en.wikipedia.org/wiki/Percent-encoding) form.
 e.g. **MyApp -f:c://temp/my path**
 Use percent encoding to get the value.
 e.g **MyApp -f:c://temp/my%20path**
 
 As a result: all variable values are assumed to be percent encoded.

 Handy helper: to encode percent use %25.
