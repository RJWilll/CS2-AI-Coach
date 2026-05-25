using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SteamWebAPI2;
using CS2CoachLibrary;

namespace CS2CoachWeb.Pages
{
    public class UserModel : PageModel
    {
        public string steamId = string.Empty;
        public string name = string.Empty;

        public async Task OnGetAsync(string q)
        {
            var steamHandler = new SteamAPIHandler();
            steamId = q;
            name = await steamHandler.GetNameAsync(steamId);
        }
    }
}
