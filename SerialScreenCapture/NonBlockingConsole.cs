using System;
using System.Collections.Concurrent;
using System.Threading;

namespace SerialScreenCapture
{
    public static class NonBlockingConsole
    {
        private static readonly BlockingCollection<string> Queue = new BlockingCollection<string>();

        static NonBlockingConsole()
        {
            var thread = new Thread(
                () =>
                {
                    while (true)
                    {
                        Console.Write(Queue.Take());
                    }
                }) {IsBackground = true};
            thread.Start();
        }

        public static void WriteLine(Exception e)
        {
            Queue.Add(e.StackTrace + Environment.NewLine);
        }

        public static void Write(string str)
        {
            Queue.Add(str);
        }
    }
}