﻿using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace kwd.ConsoleAssist.BasicConsole
{
    /// <summary>
    /// Implement a console.
    /// </summary>
    public class DefaultConsole : IConsole
    {
        /// <inheritdoc />
        public IConsoleStream Out { get; } = new ConsoleStream(Console.Out);

        /// <inheritdoc />
        public IConsoleStream Error { get; } = new ConsoleStream(Console.Error);

        /// <inheritdoc />
        public IConsoleRead Read => new ConsolePrompt();

        /// <inheritdoc />
        public IDisposable Color(ConsoleColor font, ConsoleColor? background = null) =>
            new TempColor(font, background);

        private class ConsoleStream : IConsoleStream
        {
            private readonly TextWriter _wr;
            public ConsoleStream(TextWriter wr)
            {
                _wr = wr;
            }

            /// <inheritdoc />
            public async Task WriteLine(params string[] text)
            {
                foreach (var line in text)
                {
                    await _wr.WriteLineAsync(line);
                }
            }

            public async Task Write(string text)
            {
                await _wr.WriteAsync(text);
            }
        }

        private class ConsolePrompt : IConsoleRead
        {
            /// <inheritdoc />
            public ConsoleKeyInfo ReadKey(CancellationToken cancel, bool intercept = false)
            {
                while (!Console.KeyAvailable)
                {
                    cancel.ThrowIfCancellationRequested();
                    Thread.Sleep(150);
                }

                return Console.ReadKey(intercept);
            }

            /// <inheritdoc />
            public string PromptSecret(string prompt, CancellationToken cancel)
            {
                cancel.ThrowIfCancellationRequested();

                Console.Write(prompt);
                Console.Write(' ');

                return CancellableRead('*', cancel);
            }

            /// <inheritdoc />
            public string PromptString(string prompt, CancellationToken cancel)
            {
                Console.Write(prompt);
                Console.Write(' ');

                return CancellableRead(null, cancel);
            }

            private string CancellableRead(char? maskChar, CancellationToken cancel)
            {
                cancel.ThrowIfCancellationRequested();

                var data = new StringBuilder();
                
                while (true)
                {
                    var key = ReadKey(cancel, true);

                    if (key.Key == ConsoleKey.Backspace)
                    {
                        if (data.Length > 0)
                            data.Remove(data.Length - 1, 1);
                        Console.Write("\b \b");
                        continue;
                    }
                    
                    if (key.Key == ConsoleKey.Enter)
                    {
                        Console.WriteLine();
                        break;
                    }

                    //ignores control key
                    if (!char.IsControl(key.KeyChar))
                    {
                        data.Append(key.KeyChar);

                        Console.Write(maskChar ?? key.KeyChar);
                    }
                }
                
                return data.ToString();
            }
        }
    }
}