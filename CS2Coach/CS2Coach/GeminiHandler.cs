using CounterStrike2GSI;
using CounterStrike2GSI.EventMessages;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Web.WebView2.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace CS2Coach
{
    public class GeminiHandler
    {
        public static async Task<string> GetAIReport(string gsiReport, List<Mat> images, string apiKey)
        {
            var content = FormatContent(gsiReport, images);

            var client = new Client(apiKey: apiKey);
            var response = await client.Models.GenerateContentAsync(
              model: "gemini-2.5-flash", contents: content
            );

            return response.Text;
        }

        public static async Task<string> Prompt(string prompt, string apiKey)
        {
            var client = new Client(apiKey: apiKey);

            var response = await client.Models.GenerateContentAsync(
              model: "gemini-2.5-flash", contents: prompt
            );

            return response.Text;
        }

        public static List<Google.GenAI.Types.Content> FormatContent(string gsiReport, List<Mat> images)
        {
            var result = new List<Google.GenAI.Types.Content>();
            var parts = new List<Part>();

            parts.Add(new Part { Text = "You are a helpful assistant for Counter Strike 2 players. You are given a game report and screenshots. " +
                "In quick bullet points (1-2 sentences) only give tips on the round and gameplay. Use screenshots in assessment, specifically aim " +
                "and position, but avoid stating specific screenshot numbers. When necessary, state where to better use utility and if should have used utility differently.\n" });
            parts.Add(new Part { Text = gsiReport });


            byte[] imageBytes;

            foreach (Mat img in images)
            {
                Cv2.ImEncode(".png", img, out imageBytes);

                parts.Add(new Part
                {
                    InlineData = new Blob
                    {
                        MimeType = "image/png",
                        Data = imageBytes
                    }
                });          
            }

            result.Add(new Content
            {
                Role = "user",
                Parts = parts
            });
            return result;
        }


    }
}
