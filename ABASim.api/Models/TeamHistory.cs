namespace ABASim.api.Models
{
    public class TeamHistory
    {
        public int Id { get; set; }

        public int LeagueId { get; set; }

        public int SeasonId { get; set; }

        public int TeamId { get; set; }

        public int Wins { get; set; }

        public int Losses { get; set; }
    }
}