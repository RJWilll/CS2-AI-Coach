namespace CS2Coach
{
    using CounterStrike2GSI;
    using CounterStrike2GSI.EventMessages;
    using Google.GenAI;
    using Microsoft.Web.WebView2.Core;
    using System;
    using System.IO;
    using System.Windows.Forms;

    public partial class Form1 : Form
    {
        GSIReciever reciever;
        ScreenshotRecivever screenshotRecivever;

        public Form1()
        {
            InitializeComponent();
            reciever = new GSIReciever();
            this.screenshotRecivever = new ScreenshotRecivever();

            this.Load += Form1_Load;
            this.screenshotRecivever.StartCapture();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await webView.EnsureCoreWebView2Async(null);

            string sampleHTML = "C:\\Users\\reedj\\random_git_repos\\CS2-AI-Coach\\CS2Coach\\CS2Coach\\HTML\\home.html";

            webView.CoreWebView2.Navigate(sampleHTML);

            await Task.Delay(10000);
            reciever.WriteReportToConsole();
            
        }


        public static async Task foo()
        {
                var client = new Client();
                var response = await client.Models.GenerateContentAsync(
                  model: "gemini-3-flash-preview", contents: "Explain how AI works in a few words"
                );
                Console.WriteLine(response.Candidates[0].Content.Parts[0].Text);
        }
        


    }
}
