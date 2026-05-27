using CS2CoachLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SteamWebAPI2.Models;
using System.Xml.Linq;

namespace CS2CoachWeb.Pages
{
    public class RoundModel : PageModel
    {
        public string matchID { get; set; }

        public void OnGet(string q)
        {
            matchID = q;
        }
    }
}
