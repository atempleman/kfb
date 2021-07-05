using System.Collections.Generic;
using System.Threading.Tasks;
using ABASim.api.Dtos;
using ABASim.api.Models;

namespace ABASim.api.Data
{
    public interface IDraftRepository
    {
         Task<bool> AddDraftRanking(AddDraftRankingDto draftRanking);

         Task<IEnumerable<DraftPlayerDto>> GetDraftBoardForTeamId(int teamId, int leagueId);

         Task<bool> RemoveDraftRanking(RemoveDraftRankingDto draftRanking);

         Task<bool> MovePlayerRankingUp(AddDraftRankingDto ranking);

         Task<bool> MovePlayerRankingDown(AddDraftRankingDto ranking);

         Task<bool> BeginInitialDraft(int leagueId);

         Task<DraftTracker> GetDraftTracker(int leagueId);

         Task<IEnumerable<InitialDraft>> GetInitialDraftPicks(int leagueId);

         Task<bool> MakeDraftPick(InitialDraftPicksDto draftPick);

         Task<bool> MakeAutoPick(InitialDraftPicksDto draftPick);

         Task<IEnumerable<DraftPickDto>> GetInitialDraftPicksForPage(int round, int leagueId);

         Task<DashboardDraftPickDto> GetDashboardDraftPick(int pick, int leagueId);

         Task<InitialDraft> GetCurrentInitialDraftPick(int leagueId);

         Task<IEnumerable<InitialPickSalaryDto>> GetInitialDraftSalaryDetails();

        Task<IEnumerable<RegularDraftContract>> GetRegularDraftSalaryDetails();
    }
}