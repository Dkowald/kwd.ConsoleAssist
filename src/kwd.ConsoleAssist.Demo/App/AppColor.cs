using System;
using kwd.ConsoleAssist.BasicConsole;
using static System.ConsoleColor;

namespace kwd.ConsoleAssist.Demo.App
{
    public static class AppColor{

        public static IDisposable Error(this IConsole con)
            => con.Color(Red);

        public static IDisposable ErrorTxt(this IConsole con)
            => con.Color(DarkYellow);

        public static IDisposable H1(this IConsole con)
            => con.Color(DarkBlue);

        public static IDisposable Dir(this IConsole con)
            => con.Color(Blue);

        public static IDisposable File(this IConsole con)
            => con.Color(Green);
    }
}