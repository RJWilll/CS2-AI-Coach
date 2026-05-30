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
        public event EventHandler NewMatchStarted;
        public event EventHandler OnDeath;
        public GameStateListener gsi;
        public string gsiReport;
        bool buffer = true;
        public string myId = string.Empty;
        public int curRound;

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

            this.curRound = int.Parse(DatabaseHandler.GetLastRoundFromMatch(DatabaseHandler.GetLastMatchID())["round_number"].ToString());

            GSIReportUpdated = delegate { };
            NewMatchStarted = delegate { };
            OnDeath = delegate { };

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
            if((state.Round.Phase == CounterStrike2GSI.Nodes.Phase.Over) && buffer) // If round is over
            {
                CreateGSIReport(state);
                Debug.WriteLine("\n\n" + state);
                buffer = false;
                this.curRound++;
            }

            if (state.Round.Phase == CounterStrike2GSI.Nodes.Phase.Live && state.Player.State.Health > 0 && state.Player.SteamID == myId) // If round just started, reset buffer to allow report to be created at end of round.
            {
                buffer = true;
            }

            if (state.Map.Round < this.curRound)
            {
                NewMatchStarted(this.curRound, new EventArgs());
                this.curRound = state.Map.Round;
            }

            if(state.Player.State.Health <= 0 && state.Player.SteamID == myId)
            {
                OnDeath(state, new EventArgs());
            }
        }

        public void CreateGSIReport(GameState state)
        {
            this.GSIReport =
                $"{{\n" +
                $"\"map\": \"{state.Map.Name}\",\n" +
                $"\"side\": \"{state.Player.Team}\",\n" +
                $"\"round_number\": {state.Map.Round},\n" +
                $"\"ct_score\": {state.Map.CTStatistics.Score},\n" +
                $"\"t_score\": {state.Map.TStatistics.Score},\n" +
                $"\"player_money\": {state.Player.State.Money},\n" +
                $"\"kills\": {state.Player.MatchStats.Kills},\n" +
                $"\"assists\": {state.Player.MatchStats.Assists},\n" +
                $"\"player_deaths\": {state.Player.MatchStats.Deaths},\n" +
                $"\"player_assists\": {state.Player.MatchStats.Assists},\n" +
                $"\"player_round_kills\": {state.Player.State.RoundKills},\n" +
                $"\"damage_taken\": {state.Player.State.RoundTotalDamage},\n" +
                $"\"survived\": \"{(state.Player.State.Health > 0 ? "true" : "false")}\",\n" +
                $"\"death_x\": \"{state.Player.Position.X}\",\n" +
                $"\"death_y\": \"{state.Player.Position.Y}\",\n" +
                $"\"weapons\": \"{state.Player.Weapons}\",\n" +
                $"\"ct_consecutive_losses\": {state.Map.CTStatistics.ConsecutiveRoundLosses},\n" +
                $"\"t_consecutive_losses\": {state.Map.TStatistics.ConsecutiveRoundLosses},\n" +
                $"}}";
        }

        public void WriteReportToConsole()
        {
            Debug.WriteLine(this.GSIReport);
        }
    }
}
