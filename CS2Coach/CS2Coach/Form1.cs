namespace CS2Coach
{
    using System;
    using System.IO;
    using System.Windows.Forms;
    using Microsoft.Web.WebView2.Core;

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.Load += Form1_Load;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await webView.EnsureCoreWebView2Async(null);

            string sampleHTML = "C:\\Users\\reedj\\random_git_repos\\CS2-AI-Coach\\CS2Coach\\CS2Coach\\HTML\\";

            webView.CoreWebView2.Navigate(sampleHTML);
        }




    }
}
