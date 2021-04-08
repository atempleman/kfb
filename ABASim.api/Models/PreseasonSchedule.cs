namespace ABASim.api.Models
{
    public class PreseasonSchedule
    {
        public int Id { get; set; }

        public int AwayId { get; set; }

        public int HomeId { get; set; }

        public int Day { get; set; }

        public int LeagueId { get; set; }

        public int GameId { get; set; }
    }
}