using System.Collections.Generic;
using System.Threading.Tasks;
using ABASim.api.Dtos;
using ABASim.api.Models;

namespace ABASim.api.Data
{
    public interface ILeagueRepository
    {
        Task<bool> CheckForAvailablePrivateTeams();

         Task<League> GetLeagueForUserId(int userId);
         
         Task<League> GetLeague(int leagueId);

         Task<bool> CheckLeagueCode(string leagueCode);

         Task<IEnumerable<LeagueState>> GetLeagueStates();

         Task<LeagueState> GetLeagueStateForId(int stateId);

         Task<IEnumerable<NextDaysGameDto>> GetNextDaysGamesForPreseason(int leagueId);

         Task<IEnumerable<CurrentDayGamesDto>> GetTodaysGamesForPreason(int leagueId);

         Task<IEnumerable<NextDaysGameDto>> GetNextDaysGamesForSeason(int leagueId);

         Task<IEnumerable<CurrentDayGamesDto>> GetTodaysGamesForSeason(int leagueId);

         Task<IEnumerable<CurrentDayGamesDto>> GetFirstRoundGamesForToday(int leagueId);

         Task<IEnumerable<StandingsDto>> GetStandingsForLeague(int leagueId);

         Task<IEnumerable<StandingsDto>> GetStandingsForConference(int value, int leagueId);

         Task<IEnumerable<StandingsDto>> GetStandingsForDivision(int value, int leagueId);

         Task<IEnumerable<ScheduleDto>> GetScheduleForDisplay(int day, int leagueId);

         Task<IEnumerable<PlayoffScheduleDto>> GetPlayoffScheduleForDisplay(int day, int leagueId);

         Task<IEnumerable<TransactionDto>> GetTransactions(int leagueId);

         Task<IEnumerable<PlayByPlay>> GetGamePlayByPlay(int gameId, int leagueId);

         Task<IEnumerable<PlayByPlayPlayoff>> GetGamePlayByPlayPlayoffs(int gameId, int leagueId);

         Task<GameDetailsDto> GetPreseasonGameDetails(int gameId, int leagueId);

         Task<GameDetailsDto> GetSeasonGameDetails(int gameId, int leagueId);

         Task<GameDetailsDto> GetPlayoffGameDetails(int gameId, int leagueId);

         Task<IEnumerable<LeaguePointsDto>> GetLeagueScoring();

         Task<IEnumerable<LeagueReboundingDto>> GetLeagueRebounding();

         Task<IEnumerable<LeagueDefenceDto>> GetLeagueDefence();

         Task<IEnumerable<LeagueOtherDto>> GetLeagueOther();

         Task<IEnumerable<LeagueLeaderPointsDto>> GetPointsLeagueLeaders(int page, int leagueId);

         Task<IEnumerable<LeagueLeaderPointsDto>> GetPlayoffsPointsLeagueLeaders(int page, int leagueId);

         Task<IEnumerable<LeagueLeaderPointsDto>> GetTopFivePoints(int leagueId);

         Task<IEnumerable<LeagueLeaderAssistsDto>> GetAssistsLeagueLeaders(int page, int leagueId);

         Task<IEnumerable<LeagueLeaderAssistsDto>> GetPlayoffAssistsLeagueLeaders(int page, int leagueId);

         Task<IEnumerable<LeagueLeaderAssistsDto>> GetTopFiveAssists(int leagueId);

         Task<IEnumerable<LeagueLeaderReboundsDto>> GetReboundsLeagueLeaders(int page, int leagueId);

         Task<IEnumerable<LeagueLeaderReboundsDto>> GetPlayoffReboundsLeagueLeaders(int page, int leagueId);

         Task<IEnumerable<LeagueLeaderReboundsDto>> GetTopFiveRebounds(int leagueId);

         Task<IEnumerable<LeagueLeaderBlocksDto>> GetBlocksLeagueLeaders(int page, int leagueId);

         Task<IEnumerable<LeagueLeaderBlocksDto>> GetPlayoffBlocksLeagueLeaders(int page, int leagueId);

         Task<IEnumerable<LeagueLeaderBlocksDto>> GetTopFiveBlocks(int leagueId);

         Task<IEnumerable<LeagueLeaderStealsDto>> GetStealsLeagueLeaders(int page, int leagueId);

         Task<IEnumerable<LeagueLeaderStealsDto>> GetPlayoffStealsLeagueLeaders(int page, int leagueId);

         Task<IEnumerable<LeagueLeaderStealsDto>> GetTopFiveSteals(int leagueId);

         Task<IEnumerable<LeagueLeaderMinutesDto>> GetMinutesLeagueLeaders(int page, int leagueId);

         Task<IEnumerable<LeagueLeaderMinutesDto>> GetPlayoffMinutesLeagueLeaders(int page, int leagueId);

         Task<IEnumerable<LeagueLeaderFoulsDto>> GetFoulsLeagueLeaders(int page, int leagueId);

         Task<IEnumerable<LeagueLeaderFoulsDto>> GetPlayoffFoulsLeagueLeaders(int page, int leagueId);

         Task<IEnumerable<LeagueLeaderTurnoversDto>> GetTurnoversLeagueLeaders(int page, int leagueId);

         Task<IEnumerable<LeagueLeaderTurnoversDto>> GetPlayoffTurnoversLeagueLeaders(int page, int leagueId);

         Task<int> GetCountOfPointsLeagueLeaders(int leagueId);

         Task<int> GetCountOfPointsLeagueLeadersPlayoffs(int leagueId);

         Task<IEnumerable<PlayoffSummaryDto>> GetPlayoffSummariesForRound(int round, int leagueId);

         Task<Team> GetChampion(int leagueId);

         Task<IEnumerable<TransactionDto>> GetYesterdaysTransactions(int leagueId);

         Task<IEnumerable<LeaguePlayerInjuryDto>> GetLeaguePlayerInjuries(int leagueId);

         Task<IEnumerable<VotesDto>> GetMvpTopFive(int leagueId);

         Task<IEnumerable<VotesDto>> GetSixthManTopFive(int leagueId);

         Task<IEnumerable<VotesDto>> GetDpoyTopFive(int leagueId);

         Task<IEnumerable<VotesDto>> GetAllNBATeams(int leagueId);
    }
}