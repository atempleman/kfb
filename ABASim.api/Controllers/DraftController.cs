using System.Threading.Tasks;
using ABASim.api.Data;
using Microsoft.AspNetCore.Mvc;
using ABASim.api.Dtos;
using ABASim.api.Models;
using System;

namespace ABASim.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DraftController : ControllerBase
    {
        private readonly IDraftRepository _repo;

        public DraftController(IDraftRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("adddraftrank")]
        public async Task<IActionResult> AddDraftRanking(AddDraftRankingDto ranking)
        {
            var added = await _repo.AddDraftRanking(ranking);
            return Ok(added);
        }

        [HttpGet("getdraftboard")]
        public async Task<IActionResult> GetDraftBoardForTeam(string teamId, string leagueId)
        {
            var draftBoard = await _repo.GetDraftBoardForTeamId(Int32.Parse(teamId), Int32.Parse(leagueId));
            return Ok(draftBoard);
        }

        [HttpPost("removedraftrank")]
        public async Task<IActionResult> RemoveDraftRanking(RemoveDraftRankingDto removeRanking)
        {
	        var removed = await _repo.RemoveDraftRanking(removeRanking);
	        return Ok(removed);
        }	

        [HttpPost("moveup")]
        public async Task<IActionResult> MoveRankingUp(AddDraftRankingDto draftPlayer)
        {
            var draftBoard = await _repo.MovePlayerRankingUp(draftPlayer);
            return Ok(draftBoard);
        }

        [HttpPost("movedown")]
        public async Task<IActionResult> MoveRankingDown(AddDraftRankingDto draftPlayer)
        {
            var draftBoard = await _repo.MovePlayerRankingDown(draftPlayer);
            return Ok(draftBoard);
        }

        // [HttpPost("initialdraftselection")]
        // public async Task<IActionResult> MakeDraftPick(InitialDraftPicksDto draftPick)
        // {
        //     var selectionMade = await _repo.MakeDraftPick(draftPick);
        //     return Ok(selectionMade);
        // }

        [HttpPost("initialdraftselection")]
        public async Task<IActionResult> InitialDraftSelection(InitialDraftPicksDto draftPick)
        {
            var selectionMade = await _repo.MakeDraftPick(draftPick);
            return Ok(selectionMade);
        }

        [HttpPost("makeautopick")]
        public async Task<IActionResult> MakeAutoPick(InitialDraftPicksDto draftPick)
        {
            var selectionMade = await _repo.MakeAutoPick(draftPick);
            return Ok(selectionMade);
        }

        [HttpGet("beginInitialDraft/{leagueId}")]
        public async Task<IActionResult> BeginInitialDraft(int leagueId)
        {
            var result = await _repo.BeginInitialDraft(leagueId);
            return Ok(result);
        }

        [HttpGet("getdrafttracker/{leagueId}")]
        public async Task<IActionResult> GetDraftTracker(int leagueId)
        {
            var tracker = await _repo.GetDraftTracker(leagueId);
            return Ok(tracker);
        }

        [HttpGet("getinitialdraftpicks/{leagueId}")]
        public async Task<IActionResult> GetInitialDraftPicks(int leagueId)
        {
            var draftPicks = await _repo.GetInitialDraftPicks(leagueId);
            return Ok(draftPicks);
        }

        [HttpGet("getcurrentinitialdraftpick/{leagueId}")]
        public async Task<IActionResult> GetCurrentInitialDraftPick(int leagueId)
        {
            var draftPick = await _repo.GetCurrentInitialDraftPick(leagueId);
            return Ok(draftPick);
        }

        [HttpGet("getinitialdraftpicksforround")]
        public async Task<IActionResult> GetInitialDraftPicksForRound(string round, string leagueId)
        {
            var draftPicks = await _repo.GetInitialDraftPicksForPage(Int32.Parse(round), Int32.Parse(leagueId));
            return Ok(draftPicks);
        }

        [HttpGet("getdashboardcurrentpick")]
        public async Task<IActionResult> GetDashboardCurrentPick(string pick, string leagueId)
        {
            var draftPicks = await _repo.GetDashboardDraftPick(Int32.Parse(pick), Int32.Parse(leagueId));
            return Ok(draftPicks);
        }

        [HttpGet("getinitialdraftsalarydetails")]
        public async Task<IActionResult> GetInitialDraftSalaryDetails()
        {
            var draftPicks = await _repo.GetInitialDraftSalaryDetails();
            return Ok(draftPicks);
        }
    }
}