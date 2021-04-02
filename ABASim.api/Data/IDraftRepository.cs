using System.Collections.Generic;
using System.Threading.Tasks;
using ABASim.api.Dtos;
using ABASim.api.Models;

namespace ABASim.api.Data
{
    public interface IDraftRepository
    {
         Task<bool> AddDraftRanking(AddDraftRankingDto draftRanking);

         Task<IEnumerable<DraftPlayerDto>> GetDraftBoardForTeamId(GetRosterQuickViewDto dto);

         Task<bool> RemoveDraftRanking(RemoveDraftRankingDto draftRanking);

         Task<bool> MovePlayerRankingUp(AddDraftRankingDto ranking);

         Task<bool> MovePlayerRankingDown(AddDraftRankingDto ranking);

         Task<bool> BeginInitialDraft(int leagueId);

         Task<DraftTracker> GetDraftTracker(int leagueId);

         Task<IEnumerable<InitialDraft>> GetInitialDraftPicks();

         Task<bool> MakeDraftPick(InitialDraftPicksDto draftPick);

         Task<bool> MakeAutoPick(InitialDraftPicksDto draftPick);

         Task<IEnumerable<DraftPickDto>> GetInitialDraftPicksForPage(int page);

         Task<DashboardDraftPickDto> GetDashboardDraftPick(GetDashboardPickDto pickSpot);

         Task<InitialDraft> GetCurrentInitialDraftPick(int leagueId);

         Task<IEnumerable<InitialPickSalaryDto>> GetInitialDraftSalaryDetails();
    }
}