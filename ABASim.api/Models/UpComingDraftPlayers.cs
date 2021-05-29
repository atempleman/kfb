namespace ABASim.api.Models
{
    public class UpComingDraftPlayers
    {
        public int Id { get; set; }

        public int LeagueId { get; set; }

        public int PlayerId { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public int Age { get; set; }

        public int PGPosition { get; set; }

        public int SGPosition { get; set; }

        public int SFPosition { get; set; }

        public int PFPosition { get; set; }

        public int CPosition { get; set; }
    }
}