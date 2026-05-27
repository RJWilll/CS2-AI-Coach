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
        int matchID;


        public CS2Coach()
        {
            InitializeComponent();
            this.screenshotRecivever = new ScreenshotRecivever();
            this.apikey = string.Empty; // Set your API key here from UI
            this.reciever = new GSIReciever(string.Empty);

            this.reciever.GSIReportUpdated += OnGSIReportUpdated;
            this.reciever.NewMatchStarted += OnNewMatch;
            this.reciever.OnDeath += OnNewMatch; // For simplicity, we treat death as a new match for now. This can be refined to better suit actual match flow.

            this.matchID = DatabaseHandler.GetLastMatchID();
            DatabaseHandler.Initialize();

            //For transparency, set a unique color as the background and set that color as the transparency key
            this.BackColor = Color.Magenta;
            this.TransparencyKey = Color.Magenta;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
        }

        async void OnGSIReportUpdated(object sender, EventArgs e)
        {
            //Get GSI report and screenshots, then get AI report and update UI
            string gsiReport = reciever.GSIReport;
            List<Mat> screenshots = this.screenshotRecivever.GetImages();
            string aiReport = await GeminiHandler.GetAIReport(gsiReport, screenshots, apikey);
            this.SetText(aiReport);

            //Add to database
            JObject jsiReport = JObject.Parse(gsiReport);

            if (DatabaseHandler.GetMatch(matchID).Count == 0)
            {
                DatabaseHandler.InsertMatch(matchID, steamID, DateTime.Now, jsiReport);
            }
            else
            {
                DatabaseHandler.UpdateMatchResult(matchID, jsiReport);
            }

            DatabaseHandler.InsertRound(matchID, jsiReport, aiReport, "N/A");

            //incase need to start capture on new round, can be refined to better suit actual match flow.
            this.screenshotRecivever.StartCapture();
        }

        void OnNewMatch(object sender, EventArgs e)
        {
            matchID = matchID++;
        }

        void OnDeath(object sender, EventArgs e)
        {
            this.screenshotRecivever.EndCapture();
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

        delegate void SetTextCallback(string text);

        private void SetText(string text)
        {
            if (this.richTextBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.richTextBox1.Text = text;
            }
        }
    }
}
