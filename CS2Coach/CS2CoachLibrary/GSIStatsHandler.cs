using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace CS2CoachLibrary
{
    public static class GSIStatsHandler
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

        public static int GetTotalKills(List<JObject> matches)
        {
            int kills = 0;
            foreach (JObject item in matches)
            {
                kills += int.Parse(DatabaseHandler.GetLastRoundFromMatch(int.Parse(item["id"].ToString()))["kills"].ToString());
            }
            return kills;
        }

        public static int GetTotalAssists(List<JObject> matches)
        {
            int assists = 0;
            foreach (JObject item in matches)
            {
                assists += int.Parse(DatabaseHandler.GetLastRoundFromMatch(int.Parse(item["id"].ToString()))["assists"].ToString());
            }
            return assists;
        }

        public static int GetKillsFromOneMatch(JObject match)
        {
            return int.Parse(DatabaseHandler.GetLastRoundFromMatch(int.Parse(match["id"].ToString()))["kills"].ToString());
        }

        public static int GetAssistsFromOneMatch(JObject match)
        {
            return int.Parse(DatabaseHandler.GetLastRoundFromMatch(int.Parse(match["id"].ToString()))["assists"].ToString());
        }
    }
}
