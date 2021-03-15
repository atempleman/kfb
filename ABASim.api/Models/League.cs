namespace ABASim.api.Models
{
    public class League
    {
        public int Id { get; set; }

        public int StateId { get; set; }

        public int Day { get; set; }

        public int Year { get; set; }

        public string LeagueName { get; set; }

        public string LeagueCode { get; set; }
    }
}