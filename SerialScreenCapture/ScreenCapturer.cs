using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace SerialScreenCapture
{
    public class ScreenCapturer
    {
        private readonly string _outputDirectory;
        private readonly ImageFormat _imageFormat;
        private int _nextFileNum = 1;

        public ScreenCapturer(string outputDirectory, ImageFormat imageFormat)
        {
            _outputDirectory = outputDirectory;
            _imageFormat = imageFormat;
        }

        public void Capture()
        {
            try
            {
                var fileNum = (_nextFileNum++).ToString("D2");
                var fileExtension = _imageFormat.ToString().ToLower();
                var fileName = String.Format("screen-{0}.{1}", fileNum, fileExtension);
                var filePath = Path.Combine(_outputDirectory, fileName );
                using (var image = getDesktopImage())
                {
                    image.Save(filePath, _imageFormat);   
                }
                NonBlockingConsole.Write(" + " + fileNum);
            }
            catch (Exception e)
            {
                NonBlockingConsole.WriteLine(e);
            }
        }

        private static Bitmap getDesktopImage()
        {
            // The "virtual screen" includes all monitors.
            var bmp = new Bitmap(
                SystemInformation.VirtualScreen.Width,
                SystemInformation.VirtualScreen.Height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(
                    SystemInformation.VirtualScreen.Left,
                    SystemInformation.VirtualScreen.Top,
                    0, 0, bmp.Size);
            }
            return bmp;
        }
    }
}