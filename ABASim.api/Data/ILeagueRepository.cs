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
         
         Task<League> GetLeague();

         Task<bool> CheckLeagueCode(string leagueCode);

         Task<IEnumerable<LeagueState>> GetLeagueStates();

         Task<LeagueState> GetLeagueStateForId(int stateId);

         Task<IEnumerable<NextDaysGameDto>> GetNextDaysGamesForPreseason(int leagueId);

         Task<IEnumerable<CurrentDayGamesDto>> GetTodaysGamesForPreason(int leagueId);

         Task<IEnumerable<NextDaysGameDto>> GetNextDaysGamesForSeason(int leagueId);

         Task<IEnumerable<CurrentDayGamesDto>> GetTodaysGamesForSeason(int leagueId);

         Task<IEnumerable<CurrentDayGamesDto>> GetFirstRoundGamesForToday(int leagueId);

         Task<IEnumerable<StandingsDto>> GetStandingsForLeague();

         Task<IEnumerable<StandingsDto>> GetStandingsForConference(int conference);

         Task<IEnumerable<StandingsDto>> GetStandingsForDivision(int division);

         Task<IEnumerable<ScheduleDto>> GetScheduleForDisplay(int day);

         Task<IEnumerable<PlayoffScheduleDto>> GetPlayoffScheduleForDisplay(int day);

         Task<IEnumerable<TransactionDto>> GetTransactions();

         Task<IEnumerable<PlayByPlay>> GetGamePlayByPlay(int gameId);

         Task<IEnumerable<PlayByPlayPlayoff>> GetGamePlayByPlayPlayoffs(int gameId);

         Task<GameDetailsDto> GetPreseasonGameDetails(int gameId);

         Task<GameDetailsDto> GetSeasonGameDetails(int gameId);

         Task<GameDetailsDto> GetPlayoffGameDetails(int gameId);

         Task<IEnumerable<LeaguePointsDto>> GetLeagueScoring();

         Task<IEnumerable<LeagueReboundingDto>> GetLeagueRebounding();

         Task<IEnumerable<LeagueDefenceDto>> GetLeagueDefence();

         Task<IEnumerable<LeagueOtherDto>> GetLeagueOther();

         Task<IEnumerable<LeagueLeaderPointsDto>> GetPointsLeagueLeaders(int page);

         Task<IEnumerable<LeagueLeaderPointsDto>> GetPlayoffsPointsLeagueLeaders(int page);

         Task<IEnumerable<LeagueLeaderPointsDto>> GetTopFivePoints(int leagueId);

         Task<IEnumerable<LeagueLeaderAssistsDto>> GetAssistsLeagueLeaders(int page);

         Task<IEnumerable<LeagueLeaderAssistsDto>> GetPlayoffAssistsLeagueLeaders(int page);

         Task<IEnumerable<LeagueLeaderAssistsDto>> GetTopFiveAssists(int leagueId);

         Task<IEnumerable<LeagueLeaderReboundsDto>> GetReboundsLeagueLeaders(int page);

         Task<IEnumerable<LeagueLeaderReboundsDto>> GetPlayoffReboundsLeagueLeaders(int page);

         Task<IEnumerable<LeagueLeaderReboundsDto>> GetTopFiveRebounds(int leagueId);

         Task<IEnumerable<LeagueLeaderBlocksDto>> GetBlocksLeagueLeaders(int page);

         Task<IEnumerable<LeagueLeaderBlocksDto>> GetPlayoffBlocksLeagueLeaders(int page);

         Task<IEnumerable<LeagueLeaderBlocksDto>> GetTopFiveBlocks(int leagueId);

         Task<IEnumerable<LeagueLeaderStealsDto>> GetStealsLeagueLeaders(int page);

         Task<IEnumerable<LeagueLeaderStealsDto>> GetPlayoffStealsLeagueLeaders(int page);

         Task<IEnumerable<LeagueLeaderStealsDto>> GetTopFiveSteals(int leagueId);

         Task<IEnumerable<LeagueLeaderMinutesDto>> GetMinutesLeagueLeaders(int page);

         Task<IEnumerable<LeagueLeaderMinutesDto>> GetPlayoffMinutesLeagueLeaders(int page);

         Task<IEnumerable<LeagueLeaderFoulsDto>> GetFoulsLeagueLeaders(int page);

         Task<IEnumerable<LeagueLeaderFoulsDto>> GetPlayoffFoulsLeagueLeaders(int page);

         Task<IEnumerable<LeagueLeaderTurnoversDto>> GetTurnoversLeagueLeaders(int page);

         Task<IEnumerable<LeagueLeaderTurnoversDto>> GetPlayoffTurnoversLeagueLeaders(int page);

         int GetCountOfPointsLeagueLeaders();

         int GetCountOfPointsLeagueLeadersPlayoffs();

         Task<IEnumerable<PlayoffSummaryDto>> GetPlayoffSummariesForRound(GetPlayoffSummaryDto round);

         Task<Team> GetChampion();

         Task<IEnumerable<TransactionDto>> GetYesterdaysTransactions();

         Task<IEnumerable<LeaguePlayerInjuryDto>> GetLeaguePlayerInjuries();

         Task<IEnumerable<VotesDto>> GetMvpTopFive();

         Task<IEnumerable<VotesDto>> GetSixthManTopFive();

         Task<IEnumerable<VotesDto>> GetDpoyTopFive();

         Task<IEnumerable<VotesDto>> GetAllNBATeams();
    }
}