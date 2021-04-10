using System.Collections.Generic;
using System.Threading.Tasks;
using ABASim.api.Dtos;
using ABASim.api.Models;

namespace ABASim.api.Data
{
    public interface IAdminRepository
    {
         Task<bool> UpdateLeagueState(int status, int leagueId);

         Task<bool> RemoveTeamRegistration(int teamId, int leagueId);

         Task<bool> RunInitialDraftLottery(int leagueId);

         Task<bool> RunDayRollOver(int leagueId);

         Task<bool> CheckGamesRun(int leagueId);

         Task<bool> ChangeDay(int day, int leagueId);

         Task<bool> BeginPlayoffs(int leagueId);

         Task<bool> BeginConferenceSemis(int leagueId);

         Task<bool> BeginConferenceFinals(int leagueId);

         Task<bool> BeginFinals(int leagueId);

         Task<bool> GenerateAutoPickOrder(int leagueId);

         Task<bool> EndSeason(int leagueId);

         Task<bool> RunTeamDraftPicks(int leagueId);

         Task<bool> GenerateInitialContracts(int leagueId);

         Task<bool> ResetGame(int gameId, int leagueId);

         Task<IEnumerable<CurrentDayGamesDto>> GetGamesForRreset(int leagueId);

         Task<bool> RolloverSeasonCareerStats(int leagueId);

         Task<bool> SaveSeasonHistoricalRecords(int leagueId);

         Task<bool> ContractUpdates(int leagueId);

         Task<bool> UpdateTeamSalaries(int leagueId);

         Task<bool> GenerateDraftLottery(int leagueId);

         Task<bool> DeletePlayoffData(int leagueId);

         Task<bool> DeletePreseasonData(int leagueId);

         Task<bool> DeleteTeamSettings(int leagueId);

         Task<bool> DeleteAwardsData(int leagueId);

         Task<bool> DeleteOtherSeasonData(int leagueId);

         Task<bool> DeleteSeasonData(int leagueId);

         Task<bool> ResetStandings(int leagueId);

         Task<bool> RolloverLeague(int leagueId);

         Task<bool> ResetLeague(int leagueId);

         Task<bool> GenerateInitialSalaryCaps(int leagueId);
    }
}