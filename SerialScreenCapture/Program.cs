using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SerialScreenCapture
{
    internal class Program
    {
        private static ScreenCapturer _capturer;
        private static string _outputDirectory;

        public static void Main(string[] args)
        {
            _outputDirectory = args.FirstOrDefault()
                ?? (Path.Combine(Environment.CurrentDirectory, "Screenshots"));

            Directory.CreateDirectory(_outputDirectory);

            _capturer = new ScreenCapturer(
                _outputDirectory,
                ImageFormat.Png);

            printInstructions();

            Hook.CreateHook(KeyHandler);
            Application.Run();
            Hook.DestroyHook();
        }

        private static void printInstructions()
        {
            var defaultForeground = Console.ForegroundColor;

            Console.WriteLine("OUTPUT DIRECTORY:");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(_outputDirectory);
            Console.ForegroundColor = defaultForeground;
            Console.WriteLine();

            Console.Write("Press ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[Pause Break]");
            Console.ForegroundColor = defaultForeground;
            Console.Write(" to capture and save a screenshot\n");

            Console.Write("Press ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[Scroll Lock]");
            Console.ForegroundColor = defaultForeground;
            Console.Write(" to open the output directory in Explorer\n");
        }

        public static void KeyHandler(IntPtr wParam, IntPtr lParam)
        {
            var key = Marshal.ReadInt32(lParam);
            var vk = (Hook.VK) key;
            switch (vk)
            {
                case Hook.VK.VK_PAUSE:
                    // By using a task, we can prevent the capture of one screenshot from blocking another.
                    // Thanks to asynchrony, we get rapid-fire screenshots!
                    Task.Run(() => _capturer.Capture());
                    break;
                case Hook.VK.VK_SCROLL:
                    Task.Run(() => openOutputDirectory());
                    break;
            }
        }

        private static void openOutputDirectory()
        {
            System.Diagnostics.Process.Start("CMD.exe", "/C explorer " + _outputDirectory);
        }
    }
}