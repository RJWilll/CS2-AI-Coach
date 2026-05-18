using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using CounterStrike2GSI;
using CounterStrike2GSI.EventMessages;
using System.Diagnostics;

namespace CS2Coach
{
    public class GSIReciever
    {
        public event EventHandler GSIReportUpdated;
        public GameStateListener gsi;
        public string gsiReport;
        bool buffer = true;
        string myId = string.Empty;

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
            this.myId = myId;

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
                $"GSI Report:" +
                $"Map: {state.Map}," +
                $"Team: {state.Player.Team}," +
                $"CTScore: {state.Map.CTStatistics.Score}," +
                $"TScore: {state.Map.TStatistics.Score}," +
                $"Player Money: {state.Player.State.Money}," +
                $"Player Kills: {state.Player.MatchStats.Kills}," +
                $"Player Deaths: {state.Player.MatchStats.Deaths}," +
                $"Player Assists: {state.Player.MatchStats.Assists}," +
                $"Player Round Kills: {state.Player.State.RoundKills}," +
                $"CT Consecutive Round Losses {state.Map.CTStatistics.ConsecutiveRoundLosses}," +
                $"T Consecutive Round Losses {state.Map.TStatistics.ConsecutiveRoundLosses},";
        }

        public void WriteReportToConsole()
        {
            Debug.WriteLine(this.GSIReport);
        }
    }
}
