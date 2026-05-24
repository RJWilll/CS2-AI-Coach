using CounterStrike2GSI;
using CounterStrike2GSI.EventMessages;
using Microsoft.Web.WebView2.Core;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;


namespace CS2Coach
{
    public partial class ScreenshotRecivever
    {


            [LibraryImport("user32.dll", EntryPoint = "FindWindowW", StringMarshalling = StringMarshalling.Utf16)]
            private static partial IntPtr FindWindow(string? lpClassName, string? lpWindowName);

            [LibraryImport("user32.dll", EntryPoint = "GetWindowRect")]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static partial bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

            [LibraryImport("user32.dll", EntryPoint = "PrintWindow")]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static partial bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);
        

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;
        }

        string BASEDIR = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
        Queue<Mat> Screenshots;
        int numOfFrames = 5;
        int fps = 1;

        public bool isCapture = false;

        public ScreenshotRecivever()
        {
            this.Screenshots = new Queue<Mat>();
        }

        public async void MakeScreenshots()
        {
            IntPtr hWnd = FindWindow(null, "Counter-Strike 2");

            GetWindowRect(hWnd, out RECT rect);
            int screenWidth = rect.Right - rect.Left;
            int screenHeight = rect.Bottom - rect.Top;

            Mat matImage = new Mat();

            Rectangle captureArea = new Rectangle(0, 0, screenWidth, screenHeight);

            while (isCapture)
            {

                using (Bitmap bitmap = new Bitmap(screenWidth, screenHeight))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        IntPtr hdc = g.GetHdc();
                        PrintWindow(hWnd, hdc, 0);
                        g.ReleaseHdc(hdc);
                    }

                    matImage = new Mat();
                    matImage = BitmapConverter.ToMat(bitmap);
                    Cv2.Resize(matImage, matImage, new OpenCvSharp.Size(640, 360)); // Resize to 640x360 for faster processing
                    
                    this.Screenshots.Enqueue(matImage);

                    if (this.Screenshots.Count > this.numOfFrames)
                    {
                        this.Screenshots.Dequeue();
                    }
                }

                await Task.Delay(1000 / fps);
            }

        }

        public async void StartCapture()
        {
            isCapture = true;
            MakeScreenshots();
        }

        public void EndCapture()
        {
            isCapture = false;
        }

        public List<Mat> GetImages()
        {
            return this.Screenshots.ToList();
        }

        public void SaveImages()
        {
            int i = 0;
            foreach (Mat mat in this.GetImages())
            {
                Cv2.ImWrite($"{BASEDIR}\\output{i++}.png", mat);
            }
        }
    }
}
