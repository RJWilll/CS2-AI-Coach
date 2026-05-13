namespace CS2Coach
{
    using System;
    using System.IO;
    using System.Windows.Forms;
    using Microsoft.Web.WebView2.Core;
    using CounterStrike2GSI;
    using CounterStrike2GSI.EventMessages;

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.Load += Form1_Load;
            SetUpGSL();

        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await webView.EnsureCoreWebView2Async(null);

            string sampleHTML = "C:\\Users\\reedj\\random_git_repos\\CS2-AI-Coach\\CS2Coach\\CS2Coach\\HTML\\home.html";

            webView.CoreWebView2.Navigate(sampleHTML);
        }

        public void SetUpGSL()
        {
            GameStateListener gsl = new GameStateListener(3000); // For localhost:3000
            gsl.GenerateGSIConfigFile("CS2Coach");
            gsl.GameEvent += OnGameEvent; // Subscribe to event for when round concludes.

            if(!gsl.Start())
            {
                System.Diagnostics.Debug.WriteLine("GSL Could not start");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("GSL Started");
            }
        }

        void OnGameEvent(CS2GameEvent game_event)
        {
            if(game_event is RoundConcluded)
            {
                System.Diagnostics.Debug.WriteLine("Round Ended");
                
            }
        }


    }
}
