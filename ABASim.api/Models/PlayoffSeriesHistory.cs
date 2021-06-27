namespace ABASim.api.Models
{
    public class PlayoffSeriesHistory
    {
        public int Id { get; set; }
        
        public int LeagueId { get; set; }

        public int SeasonId { get; set; }

        public int Round { get; set; }

        public int HomeTeamId { get; set; }

        public int AwayTeamId { get; set; }

        public int HomeWins { get; set; }

        public int AwayWins { get; set; }

        public int Conference { get; set; }
    }
}