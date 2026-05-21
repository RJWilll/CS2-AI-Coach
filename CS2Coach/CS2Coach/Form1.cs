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
    using CS2CoachLibrary;
    using Newtonsoft.Json.Linq;

    public partial class CS2Coach : Form
    {
        GSIReciever reciever;
        ScreenshotRecivever screenshotRecivever;
        string apikey;
        string steamID;


        public CS2Coach()
        {
            InitializeComponent();
            this.screenshotRecivever = new ScreenshotRecivever();
            this.apikey = string.Empty; // Set your API key here from UI
            this.reciever = new GSIReciever(string.Empty);
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
            string aiReport = await GeminiHandler.GetAIReport(gsiReport, screenshots, apikey);
            this.richTextBox1.Text = aiReport;

            //Add to database
            int matchID = 12;
            int roundID = 1;
            JObject jsiReport = JObject.Parse(gsiReport);
            DatabaseHandler.InsertRound(matchID, jsiReport, aiReport, "N/A");

            if (DatabaseHandler.GetMatch(matchID).Count == 0)
            {
                DatabaseHandler.InsertMatch(matchID, steamID, DateTime.Now, jsiReport);
            }
            else
            {
                DatabaseHandler.UpdateMatchResult(matchID, jsiReport);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.screenshotRecivever.StartCapture();
            this.richTextBox2.Text = "Started coach.";
            this.reciever.myId = steamID = this.textBox2.Text;
            this.apikey = this.textBox1.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.screenshotRecivever.EndCapture();
            this.richTextBox2.Text = "Stopped coach.";
        }
    }
}
