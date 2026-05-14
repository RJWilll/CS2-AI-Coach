using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using CounterStrike2GSI;
using CounterStrike2GSI.EventMessages;

namespace CS2Coach
{
    public class GSIReciever
    {
        public event EventHandler GSIReportUpdated;
        public GameStateListener gsi;
        public string gsiReport;

        public string GSIReport
        {
            get
            {
                return gsiReport;
            }
            set
            {
                this.gsiReport = value;
                GSIReportUpdated(this, new EventArgs());
            }
        }

        public GSIReciever()
        {
            gsi = new GameStateListener(3000); // For localhost:3000
            this.gsi.GenerateGSIConfigFile("CS2Coach");
            this.gsi.NewGameState += OnGameEvent; // Subscribe to event for when round concludes.

            if (!gsi.Start())
            {
                System.Diagnostics.Debug.WriteLine("GSL Could not start");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("GSL Started");
            }
        }

        void OnGameEvent(GameState state)
        {
            if(state.Round.Phase == CounterStrike2GSI.Nodes.Phase.Over) // If round is over
            {
                CreateGSIReport(state);
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



    }
}
