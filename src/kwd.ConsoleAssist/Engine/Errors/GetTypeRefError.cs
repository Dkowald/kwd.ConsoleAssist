using System;

namespace kwd.ConsoleAssist.Engine.Errors
{
    /// <summary>
    /// Raised if generator unable to resolve type to
    /// syntax node.
    /// </summary>
    public class GetTypeRefError : Exception
    {
        /// <summary>
        /// Create error: 
        /// </summary>
        public GetTypeRefError(Type attempted)
            : base(GetMessage(attempted))
        {
            Attempted = attempted;
        }

        /// <summary>
        /// The attempted type.
        /// </summary>
        public readonly Type Attempted;

        private static string GetMessage(Type type) =>
            $"Failed to resolve type {type.Name}"+
         " verify your project has PreserveCompilationContext set to true";
    }
}
