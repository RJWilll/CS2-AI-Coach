using System;
using System.IO;
using CounterStrike2GSI;
using CounterStrike2GSI.EventMessages;
using System.Diagnostics;

namespace CS2CoachLibrary
{
    public class GSIReciever
    {
        public event EventHandler GSIReportUpdated;
        public GameStateListener gsi;
        public string gsiReport;
        bool buffer = true;
        public string myId = string.Empty;

        public string GSIReport
        {
            get
            {
                return gsiReport;
            }
            set
            {
                this.gsiReport = value;
                GSIReportUpdated(gsiReport, new EventArgs());
            }
        }

        public GSIReciever(string id)
        {
            gsi = new GameStateListener(3000); // For localhost:3000
            this.gsi.GenerateGSIConfigFile("CS2Coach");
            this.gsi.NewGameState += OnGameEvent; // Subscribe to event for when round concludes.
            this.gsiReport = "Empty Report";
            this.myId = id;

            GSIReportUpdated = delegate { };

            if (!gsi.Start())
            {
                System.Diagnostics.Debug.WriteLine("GSL Could not start");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("GSL Started");
            }
        }

        void OnGameEvent(GameState state) // Pops at every game event, but we only care about round conclusion for now.
        {
            if((state.Round.Phase == CounterStrike2GSI.Nodes.Phase.Over || (state.Player.State.Health <= 0 && state.Player.SteamID == myId)) && buffer) // If round is over
            {
                CreateGSIReport(state);
                Debug.WriteLine("\n\n" + state);
                buffer = false;
            }

            if(state.Round.Phase == CounterStrike2GSI.Nodes.Phase.Live && state.Player.State.Health > 0 && state.Player.SteamID == myId) // If round just started, reset buffer to allow report to be created at end of round.
            {
                buffer = true;
            }
        }

        public void CreateGSIReport(GameState state)
        {
            this.GSIReport =
                $"{{\n" +
                $"\"map\": \"{state.Map}\"," +
                $"\"side\": \"{state.Player.Team}\"," +
                $"\"round_number\": {state.Map.Round}," +
                $"\"ct_score\": {state.Map.CTStatistics.Score}," +
                $"\"t_score\": {state.Map.TStatistics.Score}," +
                $"\"player_money\": {state.Player.State.Money}," +
                $"\"kills\": {state.Player.MatchStats.Kills}," +
                $"\"player_deaths\": {state.Player.MatchStats.Deaths}," +
                $"\"player_assists\": {state.Player.MatchStats.Assists}," +
                $"\"player_round_kills\": {state.Player.State.RoundKills}," +
                $"\"damage_taken\": {state.Player.State.RoundTotalDamage}," +
                $"\"survived\": {state.Player.State.Health > 0}," +
                $"\"death_x\": {state.Player.Position.X}," +
                $"\"death_y\": {state.Player.Position.Y}," +
                $"\"weapons\": \"{state.Player.Weapons}\"," +
                $"\"ct_consecutive_losses\": {state.Map.CTStatistics.ConsecutiveRoundLosses}," +
                $"\"t_consecutive_losses\": {state.Map.TStatistics.ConsecutiveRoundLosses}," +
                $"}}";
        }

        public void WriteReportToConsole()
        {
            Debug.WriteLine(this.GSIReport);
        }
    }
}
