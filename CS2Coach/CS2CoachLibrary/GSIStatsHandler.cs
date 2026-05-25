using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace CS2CoachLibrary
{
    internal static class GSIStatsHandler
    {
        public static float GetMatchWinRate(List<JObject> matches)
        {
            int wins = 0;
            foreach (JObject item in matches)
            {
                if (item["result"].ToString() == "win")
                {
                    wins++;
                }
            }

            return (float)wins / matches.Count();
        }

        public static int GetTotalDeaths(List<JObject> rounds)
        {
            int deaths = 0;

            foreach (JObject item in rounds)
            {
                deaths += int.Parse(item["survived"].ToString());
            }

            return deaths;
        }

        public static float AverageDamageTakenPerRound(List<JObject> rounds)
        {
            int totalDamage = 0;
            foreach (JObject item in rounds)
            {
                totalDamage += int.Parse(item["damage_taken"].ToString());
            }
            return (float)totalDamage / rounds.Count();
        }

        public static int NumberRoundsPlayed(List<JObject> rounds)
        {
            return rounds.Count();
        }

        public static int NumberMatchesPlayed(List<JObject> matches)
        {
            return matches.Count();
        }

        public static int TotalMatchWins(List<JObject> matches)
        {
            int wins = 0;
            foreach (JObject item in matches)
            {
                if (item["result"].ToString() == "win")
                {
                    wins++;
                }
            }
            return wins;
        }

    }
}
