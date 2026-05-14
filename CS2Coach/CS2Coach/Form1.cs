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
        GSIReciever reciever;
        public Form1()
        {
            InitializeComponent();
            reciever = new GSIReciever();
            this.Load += Form1_Load;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await webView.EnsureCoreWebView2Async(null);

            string sampleHTML = "C:\\Users\\reedj\\random_git_repos\\CS2-AI-Coach\\CS2Coach\\CS2Coach\\HTML\\home.html";

            webView.CoreWebView2.Navigate(sampleHTML);
        }

    }
}
