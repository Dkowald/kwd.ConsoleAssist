using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MoreLinq.Extensions;

namespace kwd.ConsoleAssist.Engine.Generator
{
    /// <summary>
    /// Extension to simplify reading run time type info.
    /// </summary>
    public static class TypeHelper
    {
        static readonly BindingFlags ActionMethodFlags =
            BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public;

        /// <summary>
        /// Type members used for action (and sub-commands)
        /// </summary>
        public static IEnumerable<MethodInfo> ActionMethods(this Type type)
            => type.GetMethods(ActionMethodFlags)
                .Where(x => !x.IsSpecialName)
                .DistinctBy(x => x.Name);

        /// <summary>
        /// Overloads for the given action method
        /// </summary>
        public static IEnumerable<MethodInfo> Overloads(this MethodInfo op)
            => op.DeclaringType == null? throw new Exception("no declaring type for op")
                : op.DeclaringType.GetMethods(ActionMethodFlags)
                    .Where(x => x.Name == op.Name);

        /// <summary>
        /// True if type is a Task
        /// </summary>
        public static bool IsTask(this Type type) =>
            typeof(Task).IsAssignableFrom(type);

        /// <summary>
        /// Unwrap Type&lt;T&gt; (return self if not Task).
        /// </summary>
        public static Type UnwrapTask(this Type type)
        {
            if (typeof(Task).IsAssignableFrom(type))
            {
                return type.IsGenericType ? 
                    type.GenericTypeArguments.Single() : 
                    typeof(void);
            }
            
            return type;
        }
    }
}