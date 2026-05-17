namespace CS2Coach
{
    using CounterStrike2GSI;
    using CounterStrike2GSI.EventMessages;
    using Google.GenAI;
    using Microsoft.Web.WebView2.Core;
    using OpenCvSharp;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Windows.Forms;


    public partial class CS2Coach : Form
    {
        GSIReciever reciever;
        ScreenshotRecivever screenshotRecivever;



        public CS2Coach()
        {
            InitializeComponent();
            reciever = new GSIReciever();
            this.screenshotRecivever = new ScreenshotRecivever();
            this.reciever.GSIReportUpdated += OnGSIReportUpdated;

            //this.Load += Form1_Load;
            this.BackColor = Color.Magenta;
            this.TransparencyKey = Color.Magenta;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
        }

        async void OnGSIReportUpdated(object sender, EventArgs e)
        {
            if (!screenshotRecivever.isCapture)
            {
                return;
            }
            string gsiReport = reciever.GSIReport;
            List<Mat> screenshots = this.screenshotRecivever.GetImages();
            string aiReport = await GeminiHandler.GetAIReport(gsiReport, screenshots);
            this.richTextBox1.Text = aiReport;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.screenshotRecivever.StartCapture();
            this.richTextBox2.Text = "Started capturing screenshots.";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.screenshotRecivever.EndCapture();
            this.richTextBox2.Text = "Stopped capturing screenshots.";
        }
    }
}
