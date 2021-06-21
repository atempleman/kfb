using ABASim.api.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ABASim.api.Data
{

    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<DepthChart> DepthCharts { get; set; }

        public DbSet<DraftPick> DraftPicks { get; set; }

        public DbSet<GameBoxScore> GameBoxScores { get; set; }

        public DbSet<GameResult> GameResults { get; set; }

        public DbSet<InitialDraft> InitialDrafts { get; set; }

        public DbSet<League> Leagues { get; set; }

        public DbSet<LeagueState> LeagueStates { get; set; }

        public DbSet<Player> Players { get; set; }

        public DbSet<PlayerContract> PlayerContracts { get; set; }

        public DbSet<PlayerRating> PlayerRatings { get; set; }

        public DbSet<PlayerTendancy> PlayerTendancies { get; set; }

        public DbSet<Roster> Rosters { get; set; }

        public DbSet<Schedule> Schedules { get; set; }

        public DbSet<Standing> Standings { get; set; }

        public DbSet<Team> Teams { get; set; }

        public DbSet<TeamDraftPick> TeamDraftPicks { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<ContactForm> ContactForms { get; set; }

        public DbSet<PlayerGrading> PlayerGradings { get; set; }

        public DbSet<DraftRanking> DraftRankings { get; set; }

        public DbSet<DraftTracker> DraftTrackers { get; set; }

        public DbSet<PlayerTeam> PlayerTeams { get; set; }

        public DbSet<PreseasonSchedule> PreseasonSchedules { get; set; }

        public DbSet<PreseasonGameResult> PreseasonGameResults { get; set; }

        public DbSet<CoachSetting> CoachSettings { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<PlayByPlay> PlayByPlays { get; set; }

        public DbSet<PlayerStat> PlayerStats { get; set; }

        public DbSet<PlayerStatsPlayoff> PlayerStatsPlayoffs { get; set; }

        public DbSet<Trade> Trades { get; set; }

        public DbSet<TradeMessage> TradeMessages { get; set; }

        public DbSet<PlayoffSeries> PlayoffSerieses { get; set; }

        public DbSet<PlayByPlayPlayoff> PlayByPlayPlayoffs { get; set; }

        public DbSet<PlayoffBoxScore> PlayoffBoxScores { get; set; }

        public DbSet<PlayoffResult> PlayoffResults { get; set; }

        public DbSet<SchedulesPlayoff> SchedulesPlayoffs { get; set; }

        public DbSet<GlobalChat> GlobalChats { get; set; }

        public DbSet<InboxMessage> InboxMessages { get; set; }

        public DbSet<AutoPickOrder> AutoPickOrders { get; set; }

        public DbSet<InjurySeverity> InjurySeverities { get; set; }

        public DbSet<InjuryType> InjuryTypes { get; set; }

        public DbSet<PlayerInjury> PlayerInjuries { get; set; }

        public DbSet<CareerPlayerStat> PlayerCareerStats { get; set; }

        public DbSet<CareerPlayerStatsPlayoff> PlayerCareerStatsPlayoffs { get; set; }

        public DbSet<SalaryCap> SalaryCaps { get; set; }

        public DbSet<TeamSalaryCap> TeamSalaryCaps { get; set; }

        public DbSet<InitialDraftContract> InitialDraftContracts { get; set; }

        public DbSet<MvpVote> MvpVoting { get; set; }

        public DbSet<SixthManVote> SixthManVoting { get; set; }

        public DbSet<DpoyVote> DpoyVoting { get; set; }

        public DbSet<TeamStrategy> TeamStrategies { get; set; }

        public DbSet<OffensiveStrategy> OffensiveStrategies { get; set; }

        public DbSet<DefensiveStrategy> DefensiveStrategies { get; set; }

        public DbSet<ContractOffer> ContractOffers { get; set; }

        public DbSet<FreeAgentDecision> FreeAgencyDecisions { get; set; }

        public DbSet<WaivedContract> WaivedPlayerContracts { get; set; }

        public DbSet<HistoricalTeamRecord> HistoricalTeamRecords { get; set; }

        public DbSet<AwardWinner> AwardWinners { get; set; }

        public DbSet<RetiredPlayer> RetiredPlayers { get; set; }

        public DbSet<MidLevelException> MidLevelExceptions { get; set; }

        public DbSet<BiAnnualException> BiAnnualExceptions { get; set; }

        public DbSet<WaiverWire> WaiverWires { get; set; }

        public DbSet<LeagueConfig> LeagueConfigs { get; set; }

        public DbSet<UpComingDraftPlayer> UpComingDraftPlayers { get; set; }

        public async void CreateNewLeauge(League league)
        {
            var leagueName = new SqlParameter("@leagueName", league.LeagueName);
            var seasonId = new SqlParameter("@seasonId", league.Year);
            var leagueCode = new SqlParameter("@leagueCode", league.LeagueCode);
            await this.Database.ExecuteSqlCommandAsync("exec spCreateNewLeague @leagueName, @seasonId, @leagueCode", leagueName, seasonId, leagueCode);
        }

        public async void RunInNewDraftees(League league, LeagueConfig lconfig)
        {
            int nextSeasonYear = 0;
            if (league.Year == 1314) {
                nextSeasonYear = 1415;
            } else if (league.Year == 1415) {
                nextSeasonYear = 1516;
            } else if (league.Year == 1516) {
                nextSeasonYear = 1617;
            } else if (league.Year == 1617) {
                nextSeasonYear = 1718;
            } else if (league.Year == 1718) {
                nextSeasonYear = 1819;
            } else if (league.Year == 1819) {
                nextSeasonYear = 1920;
            } else if (league.Year == 1920) {
                nextSeasonYear = 2021;
            }

            var currentSeasonId = new SqlParameter("@currentSeasonId", league.Year);
            var nextSeasonId = new SqlParameter("@nextSeasonId", nextSeasonYear);
            var leagueId = new SqlParameter("@leagueId", league.Id);
            await this.Database.ExecuteSqlCommandAsync("exec spNewDraftees @currentSeasonId, @nextSeasonId, @leagueId", currentSeasonId, nextSeasonId, leagueId);
        }
    }
}