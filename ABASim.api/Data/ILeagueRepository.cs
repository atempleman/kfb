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

         Task<IEnumerable<StandingsDto>> GetStandingsForConference(GetStandingLeagueDto conference);

         Task<IEnumerable<StandingsDto>> GetStandingsForDivision(GetStandingLeagueDto division);

         Task<IEnumerable<ScheduleDto>> GetScheduleForDisplay(GetScheduleLeagueDto day);

         Task<IEnumerable<PlayoffScheduleDto>> GetPlayoffScheduleForDisplay(GetScheduleLeagueDto day);

         Task<IEnumerable<TransactionDto>> GetTransactions(int leagueId);

         Task<IEnumerable<PlayByPlay>> GetGamePlayByPlay(GameLeagueDto dto);

         Task<IEnumerable<PlayByPlayPlayoff>> GetGamePlayByPlayPlayoffs(GameLeagueDto dto);

         Task<GameDetailsDto> GetPreseasonGameDetails(GameLeagueDto dto);

         Task<GameDetailsDto> GetSeasonGameDetails(GameLeagueDto dto);

         Task<GameDetailsDto> GetPlayoffGameDetails(GameLeagueDto dto);

         Task<IEnumerable<LeaguePointsDto>> GetLeagueScoring();

         Task<IEnumerable<LeagueReboundingDto>> GetLeagueRebounding();

         Task<IEnumerable<LeagueDefenceDto>> GetLeagueDefence();

         Task<IEnumerable<LeagueOtherDto>> GetLeagueOther();

         Task<IEnumerable<LeagueLeaderPointsDto>> GetPointsLeagueLeaders(GetStatLeagueDto page);

         Task<IEnumerable<LeagueLeaderPointsDto>> GetPlayoffsPointsLeagueLeaders(GetStatLeagueDto page);

         Task<IEnumerable<LeagueLeaderPointsDto>> GetTopFivePoints(int leagueId);

         Task<IEnumerable<LeagueLeaderAssistsDto>> GetAssistsLeagueLeaders(GetStatLeagueDto page);

         Task<IEnumerable<LeagueLeaderAssistsDto>> GetPlayoffAssistsLeagueLeaders(GetStatLeagueDto page);

         Task<IEnumerable<LeagueLeaderAssistsDto>> GetTopFiveAssists(int leagueId);

         Task<IEnumerable<LeagueLeaderReboundsDto>> GetReboundsLeagueLeaders(GetStatLeagueDto page);

         Task<IEnumerable<LeagueLeaderReboundsDto>> GetPlayoffReboundsLeagueLeaders(GetStatLeagueDto page);

         Task<IEnumerable<LeagueLeaderReboundsDto>> GetTopFiveRebounds(int leagueId);

         Task<IEnumerable<LeagueLeaderBlocksDto>> GetBlocksLeagueLeaders(GetStatLeagueDto page);

         Task<IEnumerable<LeagueLeaderBlocksDto>> GetPlayoffBlocksLeagueLeaders(GetStatLeagueDto page);

         Task<IEnumerable<LeagueLeaderBlocksDto>> GetTopFiveBlocks(int leagueId);

         Task<IEnumerable<LeagueLeaderStealsDto>> GetStealsLeagueLeaders(GetStatLeagueDto page);

         Task<IEnumerable<LeagueLeaderStealsDto>> GetPlayoffStealsLeagueLeaders(GetStatLeagueDto page);

         Task<IEnumerable<LeagueLeaderStealsDto>> GetTopFiveSteals(int leagueId);

         Task<IEnumerable<LeagueLeaderMinutesDto>> GetMinutesLeagueLeaders(GetStatLeagueDto page);

         Task<IEnumerable<LeagueLeaderMinutesDto>> GetPlayoffMinutesLeagueLeaders(GetStatLeagueDto page);

         Task<IEnumerable<LeagueLeaderFoulsDto>> GetFoulsLeagueLeaders(GetStatLeagueDto page);

         Task<IEnumerable<LeagueLeaderFoulsDto>> GetPlayoffFoulsLeagueLeaders(GetStatLeagueDto page);

         Task<IEnumerable<LeagueLeaderTurnoversDto>> GetTurnoversLeagueLeaders(GetStatLeagueDto page);

         Task<IEnumerable<LeagueLeaderTurnoversDto>> GetPlayoffTurnoversLeagueLeaders(GetStatLeagueDto page);

         int GetCountOfPointsLeagueLeaders(int leagueId);

         int GetCountOfPointsLeagueLeadersPlayoffs(int leagueId);

         Task<IEnumerable<PlayoffSummaryDto>> GetPlayoffSummariesForRound(GetPlayoffSummaryDto round);

         Task<Team> GetChampion();

         Task<IEnumerable<TransactionDto>> GetYesterdaysTransactions(int leagueId);

         Task<IEnumerable<LeaguePlayerInjuryDto>> GetLeaguePlayerInjuries(int leagueId);

         Task<IEnumerable<VotesDto>> GetMvpTopFive(int leagueId);

         Task<IEnumerable<VotesDto>> GetSixthManTopFive(int leagueId);

         Task<IEnumerable<VotesDto>> GetDpoyTopFive(int leagueId);

         Task<IEnumerable<VotesDto>> GetAllNBATeams(int leagueId);
    }
}