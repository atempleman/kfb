namespace ABASim.api.Models
{
    public class PlayerContract
    {
        public int Id { get; set; }

        public int PlayerId { get; set; }

        public int TeamId { get; set; }

        public int YearOne { get; set; }

        public int GuranteedOne { get; set; }

        public int YearTwo { get; set; }

        public int GuranteedTwo { get; set; }

        public int YearThree { get; set; }

        public int GuranteedThree { get; set; }

        public int YearFour { get; set; }

        public int GuranteedFour { get; set; }

        public int YearFive { get; set; }

        public int GuranteedFive { get; set; }

        public int TeamOption { get; set; }

        public int PlayerOption { get; set; }

        public int LeagueId { get; set; }
    }
}