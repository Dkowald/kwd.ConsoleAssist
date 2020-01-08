using System;

namespace kwd.ConsoleAssist.BasicConsole
{
    /// <summary>
    /// Set console colors, reverting to current on dispose.
    /// </summary>
    public class TempColor : IDisposable
    {
        private readonly ConsoleColor _font;
        private readonly ConsoleColor _background;

        /// <summary>
        /// Invert foreground / background colors.
        /// </summary>
        public static TempColor Invert() =>
            new TempColor(Console.BackgroundColor, Console.ForegroundColor);

        /// <summary>
        /// Red text on current background.
        /// </summary>
        public static TempColor Red()=>
            new TempColor(Console.BackgroundColor, ConsoleColor.Red);
        
        /// <summary>
        /// Assign text and background color to console.
        /// </summary>
        public TempColor(ConsoleColor? font, ConsoleColor? background = null)
        {
            _font = Console.ForegroundColor;
            _background = Console.BackgroundColor;

            Console.ForegroundColor = font ?? _font;
            Console.BackgroundColor = background ?? _background;
        }

        /// <summary>
        /// Revert to previous colors.
        /// </summary>
        public void Dispose()
        {
            Console.ForegroundColor = _font;
            Console.BackgroundColor = _background;
        }
    }
}