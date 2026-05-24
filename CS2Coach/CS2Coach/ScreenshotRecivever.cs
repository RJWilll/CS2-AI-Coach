using CounterStrike2GSI;
using CounterStrike2GSI.EventMessages;
using CounterStrike2GSI.Nodes;
using Microsoft.Web.WebView2.Core;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static System.Resources.ResXFileRef;


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

        [LibraryImport("user32.dll", EntryPoint = "GetDC")]
        private static partial IntPtr GetDC(IntPtr hWnd);

        [LibraryImport("gdi32.dll", EntryPoint = "CreateCompatibleDC")]
        private static partial IntPtr CreateCompatibleDC(IntPtr hdc);

        [LibraryImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
        private static partial IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [LibraryImport("gdi32.dll", EntryPoint = "SelectObject")]
        private static partial IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [LibraryImport("gdi32.dll", EntryPoint = "BitBlt")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int width, int height,
            IntPtr hdcSrc, int xSrc, int ySrc, uint dwRop);

        [LibraryImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool DeleteObject(IntPtr hObject);

        [LibraryImport("gdi32.dll", EntryPoint = "DeleteDC")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool DeleteDC(IntPtr hdc);

        [LibraryImport("user32.dll", EntryPoint = "ReleaseDC")]
        private static partial int ReleaseDC(IntPtr hWnd, IntPtr hdc);

        const uint SRCCOPY = 0x00CC0020;

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

            Mat matImage;

            while (isCapture)
            {
                IntPtr hdcScreen = GetDC(IntPtr.Zero); // capture from screen, not window
                IntPtr hdcMem = CreateCompatibleDC(hdcScreen);
                IntPtr hBitmap = CreateCompatibleBitmap(hdcScreen, screenWidth, screenHeight);
                IntPtr hOld = SelectObject(hdcMem, hBitmap);

                BitBlt(hdcMem, 0, 0, screenWidth, screenHeight, hdcScreen, rect.Left, rect.Top, SRCCOPY);

                SelectObject(hdcMem, hOld);

                matImage = new Mat();

                using (Bitmap bitmap = Image.FromHbitmap(hBitmap))
                    matImage = BitmapToMat(bitmap);

                Cv2.Resize(matImage, matImage, new OpenCvSharp.Size(640, 360)); // Resize to 640x360 for faster processing
                    
                this.Screenshots.Enqueue(matImage);

                if (this.Screenshots.Count > this.numOfFrames)
                {
                    this.Screenshots.Dequeue();
                }

                DeleteObject(hBitmap);
                DeleteDC(hdcMem);
                ReleaseDC(IntPtr.Zero, hdcScreen);


                await Task.Delay(1000 / fps);
            }
        }

        private static Mat BitmapToMat(Bitmap bitmap)
        {
            BitmapData data = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            long byteCount = (long)data.Stride * bitmap.Height;

            Mat mat = new Mat(bitmap.Height, bitmap.Width, MatType.CV_8UC4);
            unsafe
            {
                Buffer.MemoryCopy(
                        (void*)data.Scan0,   
                        (void*)mat.Data,    
                        byteCount,           
                        byteCount);      
            }

            bitmap.UnlockBits(data);

            Mat result = new Mat();
            Cv2.CvtColor(mat, result, ColorConversionCodes.BGRA2BGR);
            return result;
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
