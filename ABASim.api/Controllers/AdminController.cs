using System;
using System.Threading.Tasks;
using ABASim.api.Data;
using ABASim.api.Dtos;
using ABASim.api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ABASim.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository _repo;
        private readonly IGameEngineRepository _gameRepo;
        public AdminController(IAdminRepository repo, IGameEngineRepository gameRepo)
        {
            _repo = repo;
            _gameRepo = gameRepo;
        }

        [HttpGet("updateleaguestatus")]
        public async Task<bool> UpdateLeagueStatus(string status, string leagueId)
        {
            var updated = await _repo.UpdateLeagueState(Int32.Parse(status), Int32.Parse(leagueId));
            return updated;
        }

        [HttpGet("removeteamrego")]
        public async Task<bool> RemoveTeamRegistration(string teamId, string leagueId)
        {
            var updated = await _repo.RemoveTeamRegistration(Int32.Parse(teamId), Int32.Parse(leagueId));
            return updated;
        }

        [HttpGet("runinitialdraftlottery/{leagueId}")]
        public async Task<bool> RunInitialDraftLottery(int leagueId)
        {
            var runLottery = await _repo.RunInitialDraftLottery(leagueId);

            // Now need to setup the auto pick rankings
            var autoPicksSet = await _repo.GenerateAutoPickOrder(leagueId);

            return runLottery;
        }

        [HttpGet("checkgamesrun/{leagueId}")]
        public async Task<IActionResult> CheckDaysGamesRun(int leagueId)
        {
            var result = await _repo.CheckGamesRun(leagueId);
            return Ok(result);
        }

        [HttpGet("rolloverday/{leagueId}")]
        public async Task<bool> RollOverDay(int leagueId)
        {
            var result = await _repo.RunDayRollOver(leagueId);
            return result;
        }

        [HttpGet("changeday")]
        public async Task<bool> ChangeDay(string day, string leagueId)
        {
            var result = await _repo.ChangeDay(Int32.Parse(day), Int32.Parse(leagueId));
            return result;
        }

        [HttpGet("beginplayoffs/{leagueId}")]
        public async Task<bool> BeginPlayoffs(int leagueId)
        {
            var result = await _repo.BeginPlayoffs(leagueId);
            return result;
        }

        [HttpGet("beginconfsemis/{leagueId}")]
        public async Task<bool> BeginConferenceSemis(int leagueId)
        {
            var result = await _repo.BeginConferenceSemis(leagueId);
            return result;
        }

        [HttpGet("beginconffinals/{leagueId}")]
        public async Task<bool> BeginConferenceFinals(int leagueId)
        {
            var result = await _repo.BeginConferenceFinals(leagueId);
            return result;
        }

        [HttpGet("beginfinals/{leagueId}")]
        public async Task<bool> BeginFinals(int leagueId)
        {
            var result = await _repo.BeginFinals(leagueId);
            return result;
        }

        [HttpGet("endseason/{leagueId}")]
        public async Task<bool> EndSeason(int leagueId)
        {
            var result = await _repo.EndSeason(leagueId);
            return result;
        }

        [HttpGet("runteamdraftpicks/{leagueId}")]
        public async Task<bool> RunTeamDraftPicks(int leagueId)
        {
            var result = await _repo.RunTeamDraftPicks(leagueId);
            return result;
        }

        [HttpGet("generateinitialcontracts/{leagueId}")]
        public async Task<bool> GenerateInitialContracts(int leagueId)
        {
            var result = await _repo.GenerateInitialContracts(leagueId);
            return result;
        }

        [HttpGet("testautopickordering/{leagueId}")]
        public async Task<bool> TestAutoPickOrder(int leagueId)
        {
            var result = await _repo.GenerateAutoPickOrder(leagueId);
            return result;
        }

        [HttpGet("getgamesforreset/{leagueId}")]
        public async Task<IActionResult> GetGamesForRreset(int leagueId)
        {
            var nextGames = await _repo.GetGamesForRreset(leagueId);
            return Ok(nextGames);
        }

        [HttpGet("resetgame")]
        public async Task<bool> ResetGame(string gameId, string leagueId)
        {
            var result = await _repo.ResetGame(Int32.Parse(gameId), Int32.Parse(leagueId));
            return result;
        }

        [HttpGet("rolloverseasonstats/{leagueId}")]
        public async Task<bool> RolloverSeasonStats(int leagueId)
        {
            var result = await _repo.RolloverSeasonCareerStats(leagueId);
            return result;
        }

        [HttpGet("rolloverawards/{leagueId}")]
        public async Task<bool> RolloverAwards(int leagueId)
        {
            var result = await _repo.SaveSeasonHistoricalRecords(leagueId);
            return result;
        }

        [HttpGet("rollovercontractupdates/{leagueId}")]
        public async Task<bool> RolloverContractUpdates(int leagueId)
        {
            var result = await _repo.ContractUpdates(leagueId);
            var result2 = await _repo.UpdateTeamSalaries(leagueId);
            return result;
        }

        [HttpGet("generatedraft/{leagueId}")]
        public async Task<bool> GenerateDraft(int leagueId)
        {
            var result = await _repo.GenerateDraftLottery(leagueId);
            return result;
        }

        [HttpGet("deletepreseasonplayoffs/{leagueId}")]
        public async Task<bool> DeletePreseasonPlayoffs(int leagueId)
        {
            var result = await _repo.DeletePlayoffData(leagueId);
            result = await _repo.DeletePreseasonData(leagueId);
            return result;
        }

        [HttpGet("deleteteamsettings/{leagueId}")]
        public async Task<bool> DeleteTeamSettings(int leagueId)
        {
            var result = await _repo.DeleteTeamSettings(leagueId);
            return result;
        }

        [HttpGet("deleteawards/{leagueId}")]
        public async Task<bool> DeleteAwards(int leagueId)
        {
            var result = await _repo.SaveSeasonHistoricalRecords(leagueId);
            result = await _repo.DeleteAwardsData(leagueId);
            return result;
        }

        [HttpGet("deleteother/{leagueId}")]
        public async Task<bool> DeleteOther(int leagueId)
        {
            var result = await _repo.DeleteOtherSeasonData(leagueId);
            return result;
        }

        [HttpGet("deleteseason/{leagueId}")]
        public async Task<bool> DeleteSeason(int leagueId)
        {
            var result = await _repo.DeleteSeasonData(leagueId);
            return result;
        }

        [HttpGet("resetstandings/{leagueId}")]
        public async Task<bool> ResetStandings(int leagueId)
        {
            var result = await _repo.ResetStandings(leagueId);
            return result;
        }

        [HttpGet("rolloverleague/{leagueId}")]
        public async Task<bool> RolloverLeague(int leagueId)
        {
            var result = await _repo.RolloverLeague(leagueId);
            return result;
        }

        [HttpGet("resetleague/{leagueId}")]
        public async Task<bool> ResetLeague(int leagueId)
        {
            var result = await _repo.ResetLeague(leagueId);
            return result;
        }

        [HttpGet("generateinitialsalarycaps/{leagueId}")]
        public async Task<bool> GenerateInitialSalaryCaps(int leagueId)
        {
            var result = await _repo.GenerateInitialSalaryCaps(leagueId);
            return result;
        }

        [HttpPost("createnewleague")]
        public async Task<IActionResult> CreateNeweague(League league)
        {
            var result = await _repo.CreateNewLeague(league);
            return Ok(result);
        }
    }
}