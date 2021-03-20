namespace ABASim.api.Dtos
{
    public class QuickViewPlayerDto
    {
        public int PlayerId { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public int PGPosition { get; set; }

        public int SGPosition { get; set; }

        public int SFPosition { get; set; }

        public int PFPosition { get; set; }

        public int CPosition { get; set; }

         public int GamesStats { get; set; }

         public int OrebsStats { get; set; }

        public int DrebsStats { get; set; }
    
        public int AstStats { get; set; }

         public int PtsStats { get; set; }
    }
}