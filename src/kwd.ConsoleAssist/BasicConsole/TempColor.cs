using System;

namespace kwd.ConsoleAssist.BasicConsole
{
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
        
        public TempColor(ConsoleColor? font, ConsoleColor? background)
        {
            _font = Console.ForegroundColor;
            _background = Console.BackgroundColor;

            Console.ForegroundColor = font ?? _font;
            Console.BackgroundColor = background ?? _background;
        }

        public TempColor(ConsoleColor font):this(font, Console.BackgroundColor){}

        /// <inheritdoc />
        public void Dispose()
        {
            Console.ForegroundColor = _font;
            Console.BackgroundColor = _background;
        }
    }
}