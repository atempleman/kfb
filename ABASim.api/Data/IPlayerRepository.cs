using System.Collections.Generic;
using System.Threading.Tasks;
using ABASim.api.Dtos;
using ABASim.api.Models;

namespace ABASim.api.Data
{
    public interface IPlayerRepository
    {
         Task<IEnumerable<DraftPlayerDto>> GetInitialDraftPlayerPoolPage(int page, int leagueId);

         Task<IEnumerable<DraftPlayerDto>> GetInitialDraftPlayerPool(int leagueId);

         Task<IEnumerable<DraftSelectionPlayerDto>> GetInitialDraftSelectionPlayerPool(int leagueId);

         Task<IEnumerable<DraftPlayerDto>> DraftPoolFilterByPosition(int pos, int leagueId);

         Task<IEnumerable<DraftPlayerDto>> FilterInitialDraftPlayerPool(string value, int leagueId);

         Task<IEnumerable<Player>> GetFilteredFreeAgents(string filter, int leagueId);

         Task<IEnumerable<Player>> GetFreeAgentsByPos(int pos, int leagueId);

         Task<IEnumerable<CareerStatsDto>> GetCareerStats(int playerId, int leagueId);

         Task<IEnumerable<Player>> FilterPlayers(string value);

         Task<IEnumerable<Player>> FilterByPosition(int filter, int leagueId);

         Task<Player> GetPlayerForId(int playerId);

         Task<IEnumerable<Player>> GetAllPlayers(int leagueId);

         Task<IEnumerable<Player>> GetFreeAgents(int leagueId);

         Task<CompletePlayerDto> GetCompletePlayer(int playerId, int leagueId);

         Task<int> GetCountOfDraftPlayers(int leagueId);

         Task<IEnumerable<Player>> GetAllUpcomingPlayers(int leagueId);

         Task<int> GetCountOfDraftPlayersUpcoming(int leagueId);

         Task<Player> GetPlayerForName(string playername, int leagueId);

         Task<PlayerContractQuickViewDto> GetContractForPlayer(int playerId, int leagueId);

         Task<PlayerContract> GetFullContractForPlayer(int playerId, int leagueId);

         Task<IEnumerable<RetiredPlayer>> GetRetiredPlayers();

         Task<DetailedRetiredPlayerDto> GetDetailRetiredPlayer(int playerId);
    }
}