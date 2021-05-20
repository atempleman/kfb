using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ABASim.api.Data;
using ABASim.api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ABASim.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeagueController : ControllerBase
    {
        // private readonly DataContext _context;
        private readonly ILeagueRepository _repo;
        public LeagueController(ILeagueRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("checkavailableteamsprivate")]
        public async Task<IActionResult> CheckForAvailablePrivateTeams()
        {
            var exists = await _repo.CheckForAvailablePrivateTeams();
            return Ok(exists);
        }

        [HttpGet("checkleaguecode/{leaguecode}")]
        public async Task<IActionResult> CheckLeagueCode(string leagueCode)
        {
            var exists = await _repo.CheckLeagueCode(leagueCode);
            return Ok(exists);
        }

        [HttpGet("getleagueforuser/{userId}")]
        public async Task<IActionResult> GetLeagueForUserId(int userId)
        {
            var league = await _repo.GetLeagueForUserId(userId);
            var leagueState = await _repo.GetLeagueStateForId(league.StateId);

            LeagueDto leagueDto = new LeagueDto
            {
                Id = league.Id,
                StateId = league.StateId,
                Day = league.Day,
                State = leagueState.State,
                Year = league.Year
            };

            return Ok(leagueDto);
        }


        [HttpGet("getleague/{leagueId}")]
        public async Task<IActionResult> GetLeague(int leagueId)
        {
            var league = await _repo.GetLeague(leagueId);
            var leagueState = await _repo.GetLeagueStateForId(league.StateId);

            LeagueDto leagueDto = new LeagueDto
            {
                Id = league.Id,
                StateId = league.StateId,
                Day = league.Day,
                State = leagueState.State,
                Year = league.Year
            };

            return Ok(leagueDto);
        }

        [HttpGet("getleaguestatus")]
        public async Task<IActionResult> GetLeagueStates()
        {
            var leagueStates = await _repo.GetLeagueStates();
            return Ok(leagueStates);
        }

        [HttpGet("getleaguestateforid/{stateId}")]
        public async Task<IActionResult> GetLeagueStateForId(int stateId)
        {
            var leagueState = await _repo.GetLeagueStateForId(stateId);
            return Ok(leagueState);
        }

        [HttpGet("getgamesfortomorrow/{leagueId}")]
        public async Task<IActionResult> GetGamesForTomrrowPreseason(int leagueId)
        {
            var nextGames = await _repo.GetNextDaysGamesForPreseason(leagueId);
            return Ok(nextGames);
        }

        [HttpGet("getgamesfortoday/{leagueId}")]
        public async Task<IActionResult> GetGamesForTodayPreseason(int leagueId)
        {
            var nextGames = await _repo.GetTodaysGamesForPreason(leagueId);
            return Ok(nextGames);
        }

        [HttpGet("getstandingsforleague/{leagueId}")]
        public async Task<IActionResult> GetStandingsForLeague(int leagueId)
        {
            var standings = await _repo.GetStandingsForLeague(leagueId);
            return Ok(standings);
        }

        [HttpGet("getstandingsforconference")]
        public async Task<IActionResult> GetStandingsForConference(string value, string leagueId)
        {
            var standings = await _repo.GetStandingsForConference(Int32.Parse(value), Int32.Parse(leagueId));
            return Ok(standings);
        }

        [HttpGet("getstandingsfordivision")]
        public async Task<IActionResult> GetStandingsForDivision(string value, string leagueId)
        {
            var standings = await _repo.GetStandingsForDivision(Int32.Parse(value), Int32.Parse(leagueId));
            return Ok(standings);
        }

        [HttpGet("getscheduledisplay")]
        public async Task<IActionResult> GetScheduleForDisplay(string day, string leagueId)
        {
            var schedules = await _repo.GetScheduleForDisplay(Int32.Parse(day), Int32.Parse(leagueId));
            return Ok(schedules);
        }

        [HttpGet("getplayoffdisplay")]
        public async Task<IActionResult> GetPlayoffScheduleForDisplay(string day, string leagueId)
        {
            var schedules = await _repo.GetPlayoffScheduleForDisplay(Int32.Parse(day), Int32.Parse(leagueId));
            return Ok(schedules);
        }

        [HttpGet("gettransactions/{leagueid}")]
        public async Task<IActionResult> GetTransactions(int leagueId)
        {
            var transactions = await _repo.GetTransactions(leagueId);
            return Ok(transactions);
        }

        [HttpGet("getgameplaybyplay")]
        public async Task<IActionResult> GetGamePlayByPlay(string gameId, string leagueId)
        {
            var playByPlay = await _repo.GetGamePlayByPlay(Int32.Parse(gameId), Int32.Parse(leagueId));
            return Ok(playByPlay);
        }

        [HttpGet("getgameplaybyplayplayoffs")]
        public async Task<IActionResult> GetGamePlayByPlayPlayoff(string gameId, string leagueId)
        {
            var playByPlay = await _repo.GetGamePlayByPlayPlayoffs(Int32.Parse(gameId), Int32.Parse(leagueId));
            return Ok(playByPlay);
        }

        [HttpGet("getpreseasongamedetails")]
        public async Task<IActionResult> GetPreseasonGameDetails(string gameId, string leagueId)
        {
            var details = await _repo.GetPreseasonGameDetails(Int32.Parse(gameId), Int32.Parse(leagueId));
            return Ok(details);
        }

        [HttpGet("getseasongamedetails")]
        public async Task<IActionResult> GetSeasonGameDetails(string gameId, string leagueId)
        {
            var details = await _repo.GetSeasonGameDetails(Int32.Parse(gameId), Int32.Parse(leagueId));
            return Ok(details);
        }

        [HttpGet("getplayoffgamedetails")]
        public async Task<IActionResult> GetPlayoffGameDetails(string gameId, string leagueId)
        {
            var details = await _repo.GetPlayoffGameDetails(Int32.Parse(gameId), Int32.Parse(leagueId));
            return Ok(details);
        }

        [HttpGet("getgamesfortomorrowseason/{leagueId}")]
        public async Task<IActionResult> GetGamesForTomrrowSeason(int leagueId)
        {
            var nextGames = await _repo.GetNextDaysGamesForSeason(leagueId);
            return Ok(nextGames);
        }

        [HttpGet("getgamesfortodayseason/{leagueId}")]
        public async Task<IActionResult> GetGamesForTodaySeason(int leagueId)
        {
            var nextGames = await _repo.GetTodaysGamesForSeason(leagueId);
            return Ok(nextGames);
        }

        [HttpGet("getfirstroundgamesfortoday/{leagueId}")]
        public async Task<IActionResult> GetFirstRoundGamesForToday(int leagueId)
        {
            var nextGames = await _repo.GetFirstRoundGamesForToday(leagueId);
            return Ok(nextGames);
        }

        [HttpGet("getleaguescoring")]
        public async Task<IActionResult> GetLeagueScoring()
        {
            var scoring = await _repo.GetLeagueScoring();
            return Ok(scoring);
        }

        [HttpGet("getleaguedefence")]
        public async Task<IActionResult> GetLeagueDefence()
        {
            var defence = await _repo.GetLeagueDefence();
            return Ok(defence);
        }

        [HttpGet("getleagueother")]
        public async Task<IActionResult> GetLeagueOther()
        {
            var other = await _repo.GetLeagueOther();
            return Ok(other);
        }

        [HttpGet("getleaguerebounding")]
        public async Task<IActionResult> GetLeagueRebounding()
        {
            var rebounding = await _repo.GetLeagueRebounding();
            return Ok(rebounding);
        }

        [HttpGet("leagueleaderspoints")]
        public async Task<IActionResult> GetPointLeagueLeaders(string page, string leagueId)
        {
            var points = await _repo.GetPointsLeagueLeaders(Int32.Parse(page), Int32.Parse(leagueId));
            return Ok(points);
        }

        [HttpGet("getcountofpointsleaders/{leagueid}")]
        public async Task<IActionResult> GetCountOfLeagueLeaders(int leagueId)
        {
            var count = await _repo.GetCountOfPointsLeagueLeaders(leagueId);
            return Ok(count);
        }

        [HttpGet("leagueleadersassists")]
        public async Task<IActionResult> GetAssistLeagueLeaders(string page, string leagueId)
        {
            var assists = await _repo.GetAssistsLeagueLeaders(Int32.Parse(page), Int32.Parse(leagueId));
            return Ok(assists);
        }

        [HttpGet("leagueleadersrebounds")]
        public async Task<IActionResult> GetReboundLeagueLeaders(string page, string leagueId)
        {
            var rebounds = await _repo.GetReboundsLeagueLeaders(Int32.Parse(page), Int32.Parse(leagueId));
            return Ok(rebounds);
        }

        [HttpGet("leagueleadersblocks")]
        public async Task<IActionResult> GetBlockLeagueLeaders(string page, string leagueId)
        {
            var blocks = await _repo.GetBlocksLeagueLeaders(Int32.Parse(page), Int32.Parse(leagueId));
            return Ok(blocks);
        }

        [HttpGet("leagueleaderssteals")]
        public async Task<IActionResult> GetStealLeagueLeaders(string page, string leagueId)
        {
            var steals = await _repo.GetStealsLeagueLeaders(Int32.Parse(page), Int32.Parse(leagueId));
            return Ok(steals);
        }

        [HttpGet("leagueleadersfouls")]
        public async Task<IActionResult> GetFoulLeagueLeaders(string page, string leagueId)
        {
            var fouls = await _repo.GetFoulsLeagueLeaders(Int32.Parse(page), Int32.Parse(leagueId));
            return Ok(fouls);
        }

        [HttpGet("leagueleadersminutes")]
        public async Task<IActionResult> GetMinutesLeagueLeaders(string page, string leagueId)
        {
            var minutes = await _repo.GetMinutesLeagueLeaders(Int32.Parse(page), Int32.Parse(leagueId));
            return Ok(minutes);
        }

        [HttpGet("leagueleadersturnovers")]
        public async Task<IActionResult> GetTurnoversLeagueLeaders(string page, string leagueId)
        {
            var tos = await _repo.GetTurnoversLeagueLeaders(Int32.Parse(page), Int32.Parse(leagueId));
            return Ok(tos);
        }

        [HttpGet("gettopfivepoints/{leagueId}")]
        public async Task<IActionResult> GetTopFivePoints(int leagueId)
        {
            var results = await _repo.GetTopFivePoints(leagueId);
            return Ok(results);
        }

        [HttpGet("gettopfiveassists/{leagueId}")]
        public async Task<IActionResult> GetTopFiveAssists(int leagueId)
        {
            var results = await _repo.GetTopFiveAssists(leagueId);
            return Ok(results);
        }

        [HttpGet("gettopfiverebounds/{leagueId}")]
        public async Task<IActionResult> GetTopFiveRebounds(int leagueId)
        {
            var results = await _repo.GetTopFiveRebounds(leagueId);
            return Ok(results);
        }

        [HttpGet("gettopfivesteals/{leagueId}")]
        public async Task<IActionResult> GetTopFiveSteals(int leagueId)
        {
            var results = await _repo.GetTopFiveSteals(leagueId);
            return Ok(results);
        }

        [HttpGet("gettopfiveblocks/{leagueId}")]
        public async Task<IActionResult> GetTopFiveBlocks(int leagueId)
        {
            var results = await _repo.GetTopFiveBlocks(leagueId);
            return Ok(results);
        }

        [HttpGet("getplayoffsummariesforround")]
        public async Task<IActionResult> GetPlayoffSummariesForRound(string round, string leagueId)
        {
            var results = await _repo.GetPlayoffSummariesForRound(Int32.Parse(round), Int32.Parse(leagueId));
            return Ok(results);
        }

        [HttpGet("playoffleagueleaderspoints")]
        public async Task<IActionResult> GetPointLeagueLeadersPlayoffs(string page, string leagueId)
        {
            var points = await _repo.GetPlayoffsPointsLeagueLeaders(Int32.Parse(page), Int32.Parse(leagueId));
            return Ok(points);
        }

        [HttpGet("getcountofpointsleadersplayoffs/{leagueId}")]
        public async Task<IActionResult> GetCountOfLeagueLeadersPlayoffs(int leagueId)
        {
            var count = await _repo.GetCountOfPointsLeagueLeadersPlayoffs(leagueId);
            return Ok(count);
        }

        [HttpGet("leagueleadersassistsplayoffs")]
        public async Task<IActionResult> GetAssistLeagueLeadersPlayoffs(string page, string leagueId)
        {
            var assists = await _repo.GetPlayoffAssistsLeagueLeaders(Int32.Parse(page), Int32.Parse(leagueId));
            return Ok(assists);
        }

        [HttpGet("leagueleadersreboundsplayoffs")]
        public async Task<IActionResult> GetReboundLeagueLeadersPlayoffs(string page, string leagueId)
        {
            var rebounds = await _repo.GetPlayoffReboundsLeagueLeaders(Int32.Parse(page), Int32.Parse(leagueId));
            return Ok(rebounds);
        }

        [HttpGet("leagueleadersblocksplayoffs")]
        public async Task<IActionResult> GetBlockLeagueLeadersPlayoffs(string page, string leagueId)
        {
            var blocks = await _repo.GetPlayoffBlocksLeagueLeaders(Int32.Parse(page), Int32.Parse(leagueId));
            return Ok(blocks);
        }

        [HttpGet("leagueleadersstealsplayoffs")]
        public async Task<IActionResult> GetStealLeagueLeadersPlayoffs(string page, string leagueId)
        {
            var steals = await _repo.GetPlayoffStealsLeagueLeaders(Int32.Parse(page), Int32.Parse(leagueId));
            return Ok(steals);
        }

        [HttpGet("leagueleadersminutesplayoffs")]
        public async Task<IActionResult> GetMinutesLeagueLeadersPlayoffs(string page, string leagueId)
        {
            var minutes = await _repo.GetPlayoffMinutesLeagueLeaders(Int32.Parse(page), Int32.Parse(leagueId));
            return Ok(minutes);
        }

        [HttpGet("leagueleadersturnoversplayoffs")]
        public async Task<IActionResult> GetTurnoversLeagueLeadersPlayoffs(string page, string leagueId)
        {
            var tos = await _repo.GetPlayoffTurnoversLeagueLeaders(Int32.Parse(page), Int32.Parse(leagueId));
            return Ok(tos);
        }

        [HttpGet("leagueleadersfoulsplayoffs")]
        public async Task<IActionResult> GetFoulLeagueLeadersPlayoffs(string page, string leagueId)
        {
            var fouls = await _repo.GetPlayoffFoulsLeagueLeaders(Int32.Parse(page), Int32.Parse(leagueId));
            return Ok(fouls);
        }

        [HttpGet("getchampion/{leagueId}")]
        public async Task<IActionResult> GetChampion(int leagueId)
        {
            var team = await _repo.GetChampion(leagueId);
            return Ok(team);
        }

        [HttpGet("getyesterdaystransactions/{leagueid}")]
        public async Task<IActionResult> GetYesterdaysTransactions(int leagueId)
        {
            var team = await _repo.GetYesterdaysTransactions(leagueId);
            return Ok(team);
        }

        [HttpGet("getleagueplayerinjuries/{leagueId}")]
        public async Task<IActionResult> GetLeaguePlayerInjuries(int leagueId)
        {
            var team = await _repo.GetLeaguePlayerInjuries(leagueId);
            return Ok(team);
        }

        [HttpGet("getmvptopfive/{leagueId}")]
        public async Task<IActionResult> GetMvpTopFive(int leagueId)
        {
            var votes = await _repo.GetMvpTopFive(leagueId);
            return Ok(votes);
        }

        [HttpGet("getsixthmantopfive/{leagueId}")]
        public async Task<IActionResult> GetSixthManTopFive(int leagueId)
        {
            var votes = await _repo.GetSixthManTopFive(leagueId);
            return Ok(votes);
        }

        [HttpGet("getdpoytopfive/{leagueId}")]
        public async Task<IActionResult> GetDpoyTopFive(int leagueId)
        {
            var votes = await _repo.GetDpoyTopFive(leagueId);
            return Ok(votes);
        }

        [HttpGet("getallnbateams/{leagueId}")]
        public async Task<IActionResult> GetAllNBATeams(int leagueId)
        {
            var votes = await _repo.GetAllNBATeams(leagueId);
            return Ok(votes);
        }
    }
}