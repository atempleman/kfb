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
    public class TeamController : ControllerBase
    {
        private readonly ITeamRepository _repo;

        public TeamController(ITeamRepository repo)
        {
            _repo = repo;
        }
        
        [HttpGet("checkavailableteams")]
        public async Task<IActionResult> CheckAvailableTeams()
        {
            var exists = await _repo.CheckForAvailableTeams();
            return Ok(exists);
        }

        [HttpGet("getavailableteams")]
        public async Task<IActionResult> GetAvailableTeams()
        {
            var teams = await _repo.GetAvailableTeams();
            return Ok(teams);
        }

        [HttpGet("getavailableteamsprivate/{leaguecode}")]
        public async Task<IActionResult> GetAvailableTeamsForPrivate(string leagueCode)
        {
            var teams = await _repo.GetAvailableTeamsForPrivate(leagueCode);
            return Ok(teams);
        }

        [HttpGet("getteamforuserid/{userId}")]
        public async Task<IActionResult> GetTeamForUserId(int userId)
        {
            var team = await _repo.GetTeamForUserId(userId);
            return Ok(team);
        }

        [HttpGet("getteamforteamid")]
        public async Task<IActionResult> GetTeamForTeamId(string teamId, string leagueId)
        {
            var team = await _repo.GetTeamForTeamId(Int32.Parse(teamId), Int32.Parse(leagueId));
            return Ok(team);
        }

        [HttpGet("getextendedroster")]
        public async Task<IActionResult> GetExtendedRoster(string teamId, string leagueId)
        {
            var team = await _repo.GetExtendPlayersForTeam(Int32.Parse(teamId), Int32.Parse(leagueId));
            return Ok(team);
        }

        [HttpGet("getquickviewroster")]
        public async Task<IActionResult> GetQuickViewRoster(string teamId, string leagueId)
        {
            var team = await _repo.GetQuickViewRoster(Int32.Parse(teamId), Int32.Parse(leagueId));
            return Ok(team);
        }

        [HttpGet("getteaminjuries")]
        public async Task<IActionResult> GetTeamInjuries(string teamId, string leagueId)
        {
            var team = await _repo.GetTeamInjuries(Int32.Parse(teamId), Int32.Parse(leagueId));
            return Ok(team);
        }

        [HttpGet("getrosterforteam")]
        public async Task<IActionResult> GetRosterForTeam(string teamId, string leagueId)
        {
            var players = await _repo.GetRosterForTeam(Int32.Parse(teamId), Int32.Parse(leagueId));
            return Ok(players);
        }

        [HttpGet("getallteams/{leagueId}")]
        public async Task<IActionResult> GetAllTeams(int leagueId)
        {
            var teams = await _repo.GetAllTeams(leagueId);
            return Ok(teams);
        }

        [HttpGet("getTeamInitialLotteryOrder/{leagueId}")]
        public async Task<IActionResult> GetTeamInitialLotteryOrder(int leagueId)
        {
            var teams = await _repo.GetTeamInitialLotteryOrder(leagueId);
            return Ok(teams);
        }

        [HttpGet("getteamdepthchart")]
        public async Task<IActionResult> GetDepthChartForTeam(string teamId, string leagueId)
        {
            var depthCharts = await _repo.GetDepthChartForTeam(Int32.Parse(teamId), Int32.Parse(leagueId));
            return Ok(depthCharts);
        }

        [HttpGet("getteamforteamname")]
        public async Task<IActionResult> GetTeamForTeamName(string teamname, string leagueId)
        {
            var team = await _repo.GetTeamForTeamName(teamname, Int32.Parse(leagueId));
            return Ok(team);
        }

        [HttpGet("getteamformascot")]
        public async Task<IActionResult> GetTeamForTeamMascot(string teamname, string leagueId)
        {
            var team = await _repo.GetTeamForTeamMascot(teamname, Int32.Parse(leagueId));
            return Ok(team);
        }

        [HttpPost("savedepthchart")]
        public async Task<IActionResult> SaveDepthChart(DepthChart[] depthCharts)
        {
            var result = await _repo.SaveDepthChartForTeam(depthCharts);
            return Ok(result);
        }

        [HttpGet("rosterSpotCheck")]
        public async Task<IActionResult> RosterSpotCheck(string teamId, string leagueId)
        {
            var result = await _repo.RosterSpotCheck(Int32.Parse(teamId), Int32.Parse(leagueId));
            return Ok(result);
        }

        [HttpPost("waiveplayer")]
        public async Task<IActionResult> WaivePlayer(WaivePlayerDto waived)
        {
            var added = await _repo.WaivePlayer(waived);
            return Ok(added);
        }

        [HttpPost("signplayer")]
        public async Task<IActionResult> SignPlayer(SignedPlayerDto signed)
        {
            var added = await _repo.SignPlayer(signed);
            return Ok(added);
        }

        [HttpGet("getcoachsettings")]
        public async Task<IActionResult> GetCoachSettingsFormTeamId(string teamId, string leagueId)
        {
            var coachSetting = await _repo.GetCoachSettingForTeamId(Int32.Parse(teamId), Int32.Parse(leagueId));
            return Ok(coachSetting);
        }

        [HttpGet("getallteamsexceptusers")]
        public async Task<IActionResult> GetAllTeamsExceptUsers(string teamId, string leagueId)
        {
            var result = await _repo.GetAllTeamsExceptUsers(Int32.Parse(teamId), Int32.Parse(leagueId));
            return Ok(result);
        }

        [HttpGet("gettradeoffers")]
        public async Task<IActionResult> GetTradeOffers(string teamId, string leagueId)
        {
            var trades = await _repo.GetTradeOffers(Int32.Parse(teamId), Int32.Parse(leagueId));
            return Ok(trades);
        }

        [HttpPost("savetradeproposal")]
        public async Task<IActionResult> SaveTradeProposal(TradeDto[] trade)
        {
            var result = await _repo.SaveTradeProposal(trade);
            return Ok(result);
        }

        [HttpGet("acceptradeproposal/{tradeId}")]
        public async Task<IActionResult> AcceptTradeProposal(int tradeId)
        {
            var result = await _repo.AcceptTradeProposal(tradeId);
            return Ok(result);
        }

        [HttpGet("pullradeproposal/{tradeId}")]
        public async Task<IActionResult> PullTradeProposal(int tradeId)
        {
            var result = await _repo.PullTradeProposal(tradeId);
            return Ok(result);
        }

        [HttpPost("rejecttradeproposal")]
        public async Task<IActionResult> RejectTradeProposal(TradeMessageDto trade)
        {
            var result = await _repo.RejectTradeProposal(trade);
            return Ok(result);
        }
        [HttpGet("gettrademessage/{tradeId}")]
        public async Task<IActionResult> GetTradeMessageForTradeId(int tradeId)
        {
            var result = await _repo.GetTradeMessage(tradeId);
            return Ok(result);
        }

        [HttpGet("getteamsdraftpicks")]
        public async Task<IActionResult> GetTeamsDraftPicks(string teamId, string leagueId)
        {
            var draftPicks = await _repo.GetTeamsDraftPicks(Int32.Parse(teamId), Int32.Parse(leagueId));
            return Ok(draftPicks);
        }

        [HttpGet("getinjuriesforteam")]
        public async Task<IActionResult> GetInjuriesForTeam(string teamId, string leagueId)
        {
            var playerInjuries = await _repo.GetPlayerInjuriesForTeam(Int32.Parse(teamId), Int32.Parse(leagueId));
            return Ok(playerInjuries);
        }

        [HttpGet("getinjuriesforfreeagents/{leagueId}")]
        public async Task<IActionResult> GetInjuriesForFreeAgents(int leagueId)
        {
            var playerInjuries = await _repo.GetInjuriesForFreeAgents(leagueId);
            return Ok(playerInjuries);
        }

        [HttpGet("getinjuryforplayer")]
        public async Task<IActionResult> GetInjuryForPlayer(string playerId, string leagueId)
        {
            var injury = await _repo.GetInjuryForPlayer(Int32.Parse(playerId), Int32.Parse(leagueId));
            return Ok(injury);
        }

        [HttpGet("getteamsalarycapdetails")]
        public async Task<IActionResult> GetTeamSalaryCapDetails(string teamId, string leagueId)
        {
            var cap = await _repo.GetTeamSalaryCapDetails(Int32.Parse(teamId), Int32.Parse(leagueId));
            return Ok(cap);
        }

        [HttpGet("getteamcontracts")]
        public async Task<IActionResult> GetTeamContracts(string teamId, string leagueId)
        {
            var contracts = await _repo.GetTeamContracts(Int32.Parse(teamId), Int32.Parse(leagueId));
            return Ok(contracts);
        }

        [HttpGet("getoffensivestrategies")]
        public async Task<IActionResult> GetOffensiveStrategies()
        {
            var strategies = await _repo.GetOffensiveStrategies();
            return Ok(strategies);
        }

        [HttpGet("getdefensivestrategies")]
        public async Task<IActionResult> GetDefensiveStrategies()
        {
            var strategies = await _repo.GetDefensiveStrategies();
            return Ok(strategies);
        }

        [HttpGet("getstrategyforteam")]
        public async Task<IActionResult> GetStrategyForTeam(string teamId, string leagueId)
        {
            var strategy = await _repo.GetStrategyForTeam(Int32.Parse(teamId), Int32.Parse(leagueId));
            return Ok(strategy);
        }

        [HttpPost("savestrategy")]
        public async Task<IActionResult> SaveStrategy(TeamStrategyDto strategy)
        {
            var result = await _repo.SaveStrategy(strategy);
            return Ok(result);
        }

        
        [HttpPost("savecoachsetting")]
        public async Task<IActionResult> SaveCoachSetting(CoachSetting setting)
        {
            var result = await _repo.SaveCoachingSetting(setting);
            return Ok(result);
        }

        [HttpPost("savecontractoffer")]
        public async Task<IActionResult> SaveContractOffer(ContractOfferDto offer)
        {
            var result = await _repo.SaveContractOffer(offer);
            return Ok(true);
        }

        [HttpGet("getcontractoffersforteam")]
        public async Task<IActionResult> GetContractOffersForTeam(string teamId, string leagueId)
        {
            var offers = await _repo.GetContractOffersForTeam(Int32.Parse(teamId), Int32.Parse(leagueId));
            return Ok(offers);
        }

        [HttpGet("deletefreeagentoffer/{contractId}")]
        public async Task<IActionResult> DeleteFreeAgentOffer(int contractId)
        {
            var result = await _repo.DeleteFreeAgentOffer(contractId);
            return Ok(result);
        }

        [HttpGet("getwaivedcontracts")]
        public async Task<IActionResult> GetWaivedContracts(string teamId, string leagueId)
        {
            var result = await _repo.GetWaivedContracts(Int32.Parse(teamId), Int32.Parse(leagueId));
            return Ok(result);
        }

        // 
        [HttpGet("gettradeplayerviews")]
        public async Task<IActionResult> GetTradePlayerViews(string teamId, string leagueId)
        {
            var result = await _repo.GetTradePlayerViews(Int32.Parse(teamId), Int32.Parse(leagueId));
            return Ok(result);
        }

        [HttpGet("getteamrecord")]
        public async Task<IActionResult> GetTeamRecord(string teamId, string leagueId)
        {
            var result = await _repo.GetTeamRecord(Int32.Parse(teamId), Int32.Parse(leagueId));
            return Ok(result);
        }
    }
}