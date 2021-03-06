using System;
using System.Threading.Tasks;
using ABASim.api.Data;
using ABASim.api.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ABASim.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerRepository _repo;
        public PlayerController(IPlayerRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("getinitialdraftplayerspage")]
        public async Task<IActionResult> GetInitialDraftPlayerPoolPage(string page, string leagueId)
        {
            var players = await _repo.GetInitialDraftPlayerPoolPage(Int32.Parse(page), Int32.Parse(leagueId));
            return Ok(players);
        }

        [HttpGet("getinitialdraftplayers/{leagueId}")]
        public async Task<IActionResult> GetInitialDraftPlayerPool(int leagueId)
        {
            var players = await _repo.GetInitialDraftPlayerPool(leagueId);
            return Ok(players);
        }

        [HttpGet("getinitialdraftselectionplayers/{leagueId}")]
        public async Task<IActionResult> GetInitialDraftSelectionPlayerPool(int leagueId)
        {
            var players = await _repo.GetInitialDraftSelectionPlayerPool(leagueId);
            return Ok(players);
        }

        [HttpGet("getplayerforid/{playerId}")]
        public async Task<IActionResult> GetPlayerForId(int playerId)
        {
            var player = await _repo.GetPlayerForId(playerId);
            return Ok(player);
        }

        [HttpGet("getallplayers/{leagueId}")]
        public async Task<IActionResult> GetAllPlayers(int leagueId)
        {
            var players = await _repo.GetAllPlayers(leagueId);
            return Ok(players);
        }

        [HttpGet("getfreeagents/{leagueId}")]
        public async Task<IActionResult> GetFreeAgents(int leagueId)
        {
            var players = await _repo.GetFreeAgents(leagueId);
            return Ok(players);
        }

        [HttpGet("getcompleteplayer")]
        public async Task<IActionResult> GetCompletePlayer(string playerId, string leagueId)
        {
            var player = await _repo.GetCompletePlayer(Int32.Parse(playerId), Int32.Parse(leagueId));
            return Ok(player);
        }

        [HttpGet("getcareerstats")]
        public async Task<IActionResult> GetCareerStats(string playerId, string leagueId)
        {
            var player = await _repo.GetCareerStats(Int32.Parse(playerId), Int32.Parse(leagueId));
            return Ok(player);
        }

        
        [HttpGet("getcountofdraftplayersupcoming/{leagueId}")]
        public async Task<IActionResult> GetCountOfDraftPlayersUpcoming(int leagueId)
        {
            var count = await _repo.GetCountOfDraftPlayersUpcoming(leagueId);
            return Ok(count);
        }

        [HttpGet("getcountofdraftplayers/{leagueId}")]
        public async Task<IActionResult> GetCountOfDraftPlayers(int leagueId)
        {
            var count = await _repo.GetCountOfDraftPlayers(leagueId);
            return Ok(count);
        }

        [HttpGet("getallupcomingplayers/{leagueId}")]
        public async Task<IActionResult> GetAllUpcomingPlayers(int leagueId)
        {
            var players = await _repo.GetAllUpcomingPlayers(leagueId);
            return Ok(players);
        }
        
        [HttpGet("getdraftplayerpoolseason/{leagueId}")]
        public async Task<IActionResult> GetDraftPlayerPoolSeason(string leagueId)
        {
            var players = await _repo.GetDraftPlayerPoolSeason(Int32.Parse(leagueId));
            return Ok(players);
        }

        [HttpGet("filterdraftplayers")]
        public async Task<IActionResult> FilterInitialDraftPlayers(string filter, string leagueId)
        {
            var players = await _repo.FilterInitialDraftPlayerPool(filter, Int32.Parse(leagueId));
            return Ok(players);
        }

        [HttpGet("filterplayers/{value}")]
        public async Task<IActionResult> FilterPlayers(string value)
        {
            var players = await _repo.FilterPlayers(value);
            return Ok(players);
        }

        [HttpGet("draftpoolfilterbyposition")]
        public async Task<IActionResult> DraftPoolFilterByPosition(string filter, string leagueId)
        {
            var players = await _repo.DraftPoolFilterByPosition(Int32.Parse(filter), Int32.Parse(leagueId));
            return Ok(players);
        }

        [HttpGet("filterbyposition")]
        public async Task<IActionResult> FilterByPosition(string filter, string leagueId)
        {
            var players = await _repo.FilterByPosition(Int32.Parse(filter), Int32.Parse(leagueId));
            return Ok(players);
        }

        [HttpGet("getfreeagentsbypos")]
        public async Task<IActionResult> GetFreeAgentsByPos(string playerId, string leagueId)
        {
            var players = await _repo.GetFreeAgentsByPos(Int32.Parse(playerId), Int32.Parse(leagueId));
            return Ok(players);
        }

        [HttpGet("getfilteredfreeagents")]
        public async Task<IActionResult> GetFilteredFreeAgents(string filter, string leagueId)
        {
            var players = await _repo.GetFilteredFreeAgents(filter, Int32.Parse(leagueId));
            return Ok(players);
        }

        [HttpGet("getplayerforname")]
        public async Task<IActionResult> GetPlayerForName(string playername, string leagueId)
        {
            var players = await _repo.GetPlayerForName(playername, Int32.Parse(leagueId));
            return Ok(players);
        }

        [HttpGet("getcontractforplayer")]
        public async Task<IActionResult> GetContractForPlayer(string playerId, string leagueId)
        {
            var player = await _repo.GetContractForPlayer(Int32.Parse(playerId), Int32.Parse(leagueId));
            return Ok(player);
        }

        [HttpGet("getfullcontractforplayer")]
        public async Task<IActionResult> GetFullContractForPlayer(string playerId, string leagueId)
        {
            var player = await _repo.GetFullContractForPlayer(Int32.Parse(playerId), Int32.Parse(leagueId));
            return Ok(player);
        }

        [HttpGet("getretiredplayers")]
        public async Task<IActionResult> GetRetiredPlayers()
        {
            var players = await _repo.GetRetiredPlayers();
            return Ok(players);
        }
        
        [HttpGet("getdetailedretiredplayer/{playerId}")]
        public async Task<IActionResult> GetDetailedRetiredPlayer(int playerId)
        {
            var player = await _repo.GetDetailRetiredPlayer(playerId);
            return Ok(player);
        }
    }
}