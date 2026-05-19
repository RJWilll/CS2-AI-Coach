using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;

namespace CS2CoachLibrary
{
    public static class DatabaseHandler
    {
        public static string DB_PATH = "Data Source=cs2coach.db";

        public static void Initialize()
        {
                using var con = new SqliteConnection(DB_PATH);
                con.Open();

                var cmd = con.CreateCommand();
                cmd.CommandText = """
                CREATE TABLE IF NOT EXISTS matches (
                    id          INTEGER PRIMARY KEY AUTOINCREMENT,
                    date        TEXT    NOT NULL,
                    map         TEXT    NOT NULL,
                    result      TEXT,           -- 'win' or 'loss', filled at match end
                    score       TEXT            -- e.g. '13-8', filled at match end
                );

                CREATE TABLE IF NOT EXISTS rounds (
                    id              INTEGER PRIMARY KEY AUTOINCREMENT,
                    match_id        INTEGER NOT NULL,
                    round_number    INTEGER NOT NULL,
                    side            TEXT    NOT NULL,    -- 'CT' or 'T'
                    survived        INTEGER NOT NULL,    -- 0 or 1
                    kills           INTEGER NOT NULL,
                    damage          INTEGER NOT NULL,
                    death_x         REAL,               -- null if survived
                    death_y         REAL,               -- null if survived
                    weapon          TEXT,
                    coaching_bullets TEXT,              -- JSON array of strings
                    mistake_tags    TEXT,               -- JSON array e.g. ["positioning","peeking"]
                    FOREIGN KEY (match_id) REFERENCES matches(id)
                );
            """;
                cmd.ExecuteNonQuery();
        }
    }
}
