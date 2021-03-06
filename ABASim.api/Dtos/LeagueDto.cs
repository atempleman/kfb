namespace ABASim.api.Dtos
{
    public class LeagueDto
    {
        public int Id { get; set; }

        public int StateId { get; set; }

        public int Day { get; set; }

        public string State { get; set; }

        public int Year { get; set; }

        public string LeagueName { get; set; }

        public string LeagueCode { get; set; }

        public int SeasonId { get; set; }
    }
}