using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using CounterStrike2GSI;
using CounterStrike2GSI.EventMessages;
using OpenCvSharp;
using OpenCvSharp.Extensions;


namespace CS2Coach
{
    public class ScreenshotRecivever
    {
        Queue<Mat> Screenshots;
        int numOfFrames = 10;
        int fps = 5;

        bool isCapture = false;

        public ScreenshotRecivever()
        {
            this.Screenshots = new Queue<Mat>();
        }

        public async void MakeScreenshots()
        {
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            Mat matImage = new Mat();

            Rectangle captureArea = new Rectangle(0, 0, screenWidth, screenHeight);

            while (isCapture)
            {

                using (Bitmap bitmap = new Bitmap(screenWidth, screenHeight))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(captureArea.Location, System.Drawing.Point.Empty, captureArea.Size);
                    }

                    matImage = new Mat();
                    matImage = BitmapConverter.ToMat(bitmap);
                    
                    this.Screenshots.Enqueue(matImage);

                    if (this.Screenshots.Count > this.numOfFrames)
                    {
                        this.Screenshots.Dequeue();
                    }
                }

                await Task.Delay(1000000 / fps);
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
                Cv2.ImWrite($"C:\\Users\\reedj\\random_git_repos\\CS2-AI-Coach\\CS2Coach\\CS2Coach\\output{i++}.png", mat);
            }
        }
    }
}
