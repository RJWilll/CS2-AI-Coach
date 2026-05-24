using Microsoft.Data.Sqlite;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace CS2CoachLibrary
{
    public static class DatabaseHandler
    {
        public static string DB_PATH = $"Data Source={Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.Parent.FullName}\\cs2coach.db";

        public static void Initialize()
        {
            using var con = new SqliteConnection(DB_PATH);
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText = """
                CREATE TABLE IF NOT EXISTS matches (
                    id          INTEGER PRIMARY KEY,
                    steam_id          INTEGER,
                    date        TEXT    NOT NULL,
                    map         TEXT    NOT NULL,
                    result      TEXT,           -- 'win' or 'loss', filled at match end
                    score       TEXT            -- e.g. '13-8', filled at match end
                );

                CREATE TABLE IF NOT EXISTS rounds (
                    match_id        INTEGER NOT NULL,
                    round_number    INTEGER NOT NULL,
                    side            TEXT    NOT NULL,    -- 'CT' or 'T'
                    survived        INTEGER NOT NULL,    -- 0 or 1
                    kills           INTEGER NOT NULL,
                    damage_taken    INTEGER NOT NULL,
                    death_x         REAL,               -- null if survived
                    death_y         REAL,               -- null if survived
                    weapons          TEXT,
                    coaching_bullets TEXT,              -- JSON array of strings
                    mistake_tags    TEXT,               -- JSON array e.g. ["positioning","peeking"]
                    FOREIGN KEY (match_id) REFERENCES matches(id)
                );
            """;
            cmd.ExecuteNonQuery();
        }

        public static void DeleteDatabase()
        {
            if (File.Exists(DB_PATH))
                File.Delete(DB_PATH);
        }


        public static void ClearDatabase()
        {
            using var con = new SqliteConnection(DB_PATH);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = """
                DELETE FROM rounds;
                DELETE FROM matches;
            """;
            cmd.ExecuteNonQuery();
        }

        public static void InsertMatch(int id, string steamId, DateTime date, JObject gsiReport)
        {
            using var con = new SqliteConnection(DB_PATH);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = """
                INSERT INTO matches (id, steam_id, date, map, result, score)
                VALUES ($id, $steam_id, $date, $map, $result, $score);
            """;
            cmd.Parameters.AddWithValue("$id", id);
            cmd.Parameters.AddWithValue("$steam_id", steamId);
            cmd.Parameters.AddWithValue("$date", date.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("$map", gsiReport["map"].ToString());
            cmd.Parameters.AddWithValue("$result", "N/A");
            cmd.Parameters.AddWithValue("$score", $"CT: {gsiReport["ct_score"].ToString()}, T: {gsiReport["t_score"].ToString()}" );
            cmd.ExecuteNonQuery();
        }


        public static void InsertRound(int matchId, JObject roundData, string coachingBulletsJson, string mistakeTagsJson)
        {
            using var con = new SqliteConnection(DB_PATH);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = """
                INSERT INTO rounds (match_id, round_number, side, survived, kills, damage_taken, death_x, death_y, weapons, coaching_bullets, mistake_tags)
                VALUES ($match_id, $round_number, $side, $survived, $kills, $damage_taken, $death_x, $death_y, $weapons, $coaching_bullets, $mistake_tags);
            """;
            cmd.Parameters.AddWithValue("$match_id", matchId);
            cmd.Parameters.AddWithValue("$round_number", roundData["round_number"].Value<int>());
            cmd.Parameters.AddWithValue("$side", roundData["side"].Value<string>());
            cmd.Parameters.AddWithValue("$survived", roundData["survived"].Value<bool>());
            cmd.Parameters.AddWithValue("$kills", roundData["kills"].Value<int>());
            cmd.Parameters.AddWithValue("$damage_taken", roundData["damage_taken"].Value<int>());
            if (roundData["survived"].Value<bool>())
            {
                cmd.Parameters.AddWithValue("$death_x", DBNull.Value);
                cmd.Parameters.AddWithValue("$death_y", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("$death_x", roundData["death_x"].Value<float?>());
                cmd.Parameters.AddWithValue("$death_y", roundData["death_y"].Value<float?>());
            }
            cmd.Parameters.AddWithValue("$weapons", roundData["weapons"].Value<string>());
            cmd.Parameters.AddWithValue("$coaching_bullets", coachingBulletsJson);
            cmd.Parameters.AddWithValue("$mistake_tags", mistakeTagsJson);
            cmd.ExecuteNonQuery();
        }

        public static void UpdateMatchResult(int matchId, JObject gsiReport)
        {
            using var con = new SqliteConnection(DB_PATH);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = """
                UPDATE matches
                SET result = $result, score = $score
                WHERE id = $id;
            """;
            cmd.Parameters.AddWithValue("$result", "N/A");
            cmd.Parameters.AddWithValue("$score", $"CT: {gsiReport["ct_score"].ToString()}, T: {gsiReport["t_score"].ToString()}");
            cmd.Parameters.AddWithValue("$id", matchId);
            cmd.ExecuteNonQuery();
        }

        public static JObject? GetMatch(int matchId)
        {
            string temp = string.Empty;
            using var con = new SqliteConnection(DB_PATH);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = """
                SELECT * FROM matches WHERE id = $id;
            """;
            cmd.Parameters.AddWithValue("$id", matchId);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                temp = $"{{\n\"id\": {reader["id"]},\n \"steam_id\": \"{reader["steam_id"]}\",\n \"date\": \"{reader["date"]}\",\n \"map\": \"{reader["map"]}\",\n \"result\": \"{reader["result"]}\",\n \"score\": \"{reader["score"]}\"\n}}";
            }

            if(temp != string.Empty)
            {
                return JObject.Parse(temp);
            }
            else
            {
                return new JObject();
            }
        }

        public static List<JObject> GetMatchRounds(int matchId)
        {
            using var con = new SqliteConnection(DB_PATH);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = """
                SELECT * FROM rounds WHERE match_id = $match_id;
            """;
            cmd.Parameters.AddWithValue("$match_id", matchId);
            var reader = cmd.ExecuteReader();
            var rounds = new List<JObject>();
            while (reader.Read())
            {
                rounds.Add(new JObject
                {
                    ["round_number"] = reader["round_number"].ToString(),
                    ["side"] = reader["side"].ToString(),
                    ["survived"] = reader["survived"].ToString(),
                    ["kills"] = reader["kills"].ToString(),
                    ["damage_taken"] = reader["damage_taken"].ToString(),
                    ["death_x"] = reader["death_x"].ToString(),
                    ["death_y"] = reader["death_y"].ToString(),
                    ["weapons"] = reader["weapons"].ToString()
                });
            }
            return rounds;
        }
    }
}