using System.Collections.Generic;
using System.Threading.Tasks;
using ABASim.api.Dtos;
using ABASim.api.Models;

namespace ABASim.api.Data
{
    public interface IPlayerRepository
    {
         Task<IEnumerable<DraftPlayerDto>> GetInitialDraftPlayerPool(int page);

         Task<IEnumerable<DraftPlayerDto>> GetInitialDraftPlayerPool();

         Task<IEnumerable<DraftPlayerDto>> DraftPoolFilterByPosition(int pos);

         Task<IEnumerable<DraftPlayerDto>> FilterInitialDraftPlayerPool(string value);

         Task<IEnumerable<Player>> GetFilteredFreeAgents(PlayerLeagueDto value);

         Task<IEnumerable<Player>> GetFreeAgentsByPos(PlayerIdLeagueDto pos);

         Task<IEnumerable<CareerStatsDto>> GetCareerStats(PlayerIdLeagueDto dto);

         Task<IEnumerable<Player>> FilterPlayers(string value);

         Task<IEnumerable<Player>> FilterByPosition(PlayerIdLeagueDto pos);

         Task<Player> GetPlayerForId(int playerId);

         Task<IEnumerable<Player>> GetAllPlayers(int leagueId);

         Task<IEnumerable<Player>> GetFreeAgents(int leagueId);

         Task<CompletePlayerDto> GetCompletePlayer(PlayerIdLeagueDto dto);

         Task<int> GetCountOfDraftPlayers();

         Task<Player> GetPlayerForName(PlayerLeagueDto dto);

         Task<PlayerContractQuickViewDto> GetContractForPlayer(PlayerIdLeagueDto dto);

         Task<PlayerContract> GetFullContractForPlayer(PlayerIdLeagueDto dto);

         Task<IEnumerable<RetiredPlayer>> GetRetiredPlayers();

         Task<DetailedRetiredPlayerDto> GetDetailRetiredPlayer(int playerId);
    }
}