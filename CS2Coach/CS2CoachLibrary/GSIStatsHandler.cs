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

        public static int GetTotalDeaths(List<JObject> matches)
        {
            int deaths = 0;

            foreach (JObject item in matches)
            {
                foreach (JObject item2 in DatabaseHandler.GetMatchRounds(int.Parse(item["id"].ToString())))
                {
                    if (item2["survived"].ToString() == "false")
                    {
                        deaths++;
                    }
                }
            }

            return deaths;
        }

        public static float AverageDamageTakenPerRound(List<JObject> matches)
        {
            int totalDamage = 0;
            int totalRounds = 0;
            foreach (JObject item in matches)
            {
                foreach (JObject item2 in DatabaseHandler.GetMatchRounds(int.Parse(item["id"].ToString())))
                {
                    totalDamage += int.Parse(item2["damage_taken"].ToString());
                    totalRounds++;
                }
            }
            return (float)totalDamage / totalRounds; 
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
