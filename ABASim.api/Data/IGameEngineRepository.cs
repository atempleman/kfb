using System.Collections.Generic;
using System.Threading.Tasks;
using ABASim.api.Dtos;
using ABASim.api.Models;

namespace ABASim.api.Data
{
    public interface IGameEngineRepository
    {
        //  Task<bool> AddDraftRanking(AddDraftRankingDto draftRanking);
         Task<Team> GetTeam(int teamId, int leagueId);

         Task<Player> GetPlayer(int playerId, int leagueId);

         Task<PlayerRating> GetPlayerRating(int playerId, int leagueId);

         Task<PlayerTendancy> GetPlayerTendancy(int playerId, int leagueId);

         Task<IEnumerable<Roster>> GetRoster(int teamId, int leagueId);

         Task<IEnumerable<DepthChart>> GetDepthChart(int teamId, int leagueId);

         Task<int> GetLatestGameId();

         Task<bool> SaveTeamsBoxScore(int gameId, List<BoxScore> boxScores);

         Task<bool> SaveTeamsBoxScorePlayoffs(int gameId, List<BoxScore> boxScores);

         Task<IEnumerable<BoxScore>> GetBoxScoresForGameId(int gameId, int leagueId);

         Task<IEnumerable<BoxScore>> GetBoxScoresForGameIdPlayoffs(int gameId, int leagueId);

         Task<bool> SavePlayByPlays(List<PlayByPlay> playByPlays);

         Task<bool> SavePlayByPlaysPlayoffs(List<PlayByPlay> playByPlays);

         Task<bool> SavePreseasonResult(int awayScore, int homeScore, int winningTeamId, int gameId, int leagueId);

         Task<bool> SaveSeasonResult(int awayScore, int homeScore, int winningTeamId, int gameId, int losingTeamId, int leagueId);

         Task<bool> SavePlayoffResult(int awayScore, int homeScore, int winningTeamId, int gameId, int losingTeamId, int leagueId);

         List<InjuryType> GetInjuryTypesForSeverity(int severity);

         Task<bool> SaveInjury(List<PlayerInjury> injuries);

         Task<PlayerInjury> GetPlayerInjury(int playerId, int leagueId);

         Task<IEnumerable<CoachSetting>> GetCoachSettings(int teamId, int leagueId);

         Task<TeamStrategy> GetTeamStrategies(int teamId, int leagueId);

         Task<bool> MvpVotes(List<BoxScore> boxScores);

         Task<bool> DpoyVotes(List<BoxScore> boxScores);

         Task<bool> SixthManVotes(List<BoxScore> boxScores, List<int> homeStarters, List<int> awayStarters);
    }
}