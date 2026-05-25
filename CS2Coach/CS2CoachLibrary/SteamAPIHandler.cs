using Steam;
using SteamWebAPI2;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Models;
using SteamWebAPI2.Utilities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Text;

namespace CS2CoachLibrary
{
    public class SteamAPIHandler
    {
        SteamWebInterfaceFactory factory;

        public SteamAPIHandler()
        {
            factory = new SteamWebInterfaceFactory(""); //NEED TO REMOVE BEFORE PUSHING TO GITHUB
        }

        public async Task<string> GetNameAsync(string steamId)
        {
            var steamWebInterface = factory.CreateSteamWebInterface<SteamUser>(new HttpClient());

            var summary = await steamWebInterface.GetPlayerSummaryAsync(ulong.Parse(steamId));

            return summary.Data.Nickname;
        }
    }
}
