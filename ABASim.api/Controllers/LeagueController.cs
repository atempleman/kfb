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

        [HttpGet("getleagueforuser")]
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

        [HttpGet("getstandingsforleague")]
        public async Task<IActionResult> GetStandingsForLeague(int leagueId)
        {
            var standings = await _repo.GetStandingsForLeague(leagueId);
            return Ok(standings);
        }

        [HttpGet("getstandingsforconference/{conference}")]
        public async Task<IActionResult> GetStandingsForConference(GetStandingLeagueDto conference)
        {
            var standings = await _repo.GetStandingsForConference(conference);
            return Ok(standings);
        }

        [HttpGet("getstandingsfordivision/{division}")]
        public async Task<IActionResult> GetStandingsForDivision(GetStandingLeagueDto division)
        {
            var standings = await _repo.GetStandingsForDivision(division);
            return Ok(standings);
        }

        [HttpGet("getscheduledisplay/{day}")]
        public async Task<IActionResult> GetScheduleForDisplay(GetScheduleLeagueDto day)
        {
            var schedules = await _repo.GetScheduleForDisplay(day);
            return Ok(schedules);
        }

        [HttpGet("getplayoffdisplay/{day}")]
        public async Task<IActionResult> GetPlayoffScheduleForDisplay(GetScheduleLeagueDto day)
        {
            var schedules = await _repo.GetPlayoffScheduleForDisplay(day);
            return Ok(schedules);
        }

        [HttpGet("gettransactions/{leagueid}")]
        public async Task<IActionResult> GetTransactions(int leagueId)
        {
            var transactions = await _repo.GetTransactions(leagueId);
            return Ok(transactions);
        }

        [HttpGet("getgameplaybyplay/{gameleague}")]
        public async Task<IActionResult> GetGamePlayByPlay(GameLeagueDto dto)
        {
            var playByPlay = await _repo.GetGamePlayByPlay(dto);
            return Ok(playByPlay);
        }

        [HttpGet("getgameplaybyplayplayoffs/{gameleague}")]
        public async Task<IActionResult> GetGamePlayByPlayPlayoff(GameLeagueDto dto)
        {
            var playByPlay = await _repo.GetGamePlayByPlayPlayoffs(dto);
            return Ok(playByPlay);
        }

        [HttpGet("getpreseasongamedetails/{gameleague}")]
        public async Task<IActionResult> GetPreseasonGameDetails(GameLeagueDto dto)
        {
            var details = await _repo.GetPreseasonGameDetails(dto);
            return Ok(details);
        }

        [HttpGet("getseasongamedetails/{gameleague}")]
        public async Task<IActionResult> GetSeasonGameDetails(GameLeagueDto dto)
        {
            var details = await _repo.GetSeasonGameDetails(dto);
            return Ok(details);
        }

        [HttpGet("getplayoffgamedetails/{gameleague}")]
        public async Task<IActionResult> GetPlayoffGameDetails(GameLeagueDto dto)
        {
            var details = await _repo.GetPlayoffGameDetails(dto);
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

        [HttpGet("leagueleaderspoints/{page}")]
        public async Task<IActionResult> GetPointLeagueLeaders(GetStatLeagueDto page)
        {
            var points = await _repo.GetPointsLeagueLeaders(page);
            return Ok(points);
        }

        [HttpGet("getcountofpointsleaders/{leagueid}")]
        public async Task<IActionResult> GetCountOfLeagueLeaders(int leagueId)
        {
            // var count = await Task.Run(_repo.GetCountOfPointsLeagueLeaders());
            var count = _repo.GetCountOfPointsLeagueLeaders(leagueId);
            return Ok(count);
        }

        [HttpGet("leagueleadersassists/{page}")]
        public async Task<IActionResult> GetAssistLeagueLeaders(GetStatLeagueDto page)
        {
            var assists = await _repo.GetAssistsLeagueLeaders(page);
            return Ok(assists);
        }

        [HttpGet("leagueleadersrebounds/{page}")]
        public async Task<IActionResult> GetReboundLeagueLeaders(GetStatLeagueDto page)
        {
            var rebounds = await _repo.GetReboundsLeagueLeaders(page);
            return Ok(rebounds);
        }

        [HttpGet("leagueleadersblocks/{page}")]
        public async Task<IActionResult> GetBlockLeagueLeaders(GetStatLeagueDto page)
        {
            var blocks = await _repo.GetBlocksLeagueLeaders(page);
            return Ok(blocks);
        }

        [HttpGet("leagueleaderssteals/{page}")]
        public async Task<IActionResult> GetStealLeagueLeaders(GetStatLeagueDto page)
        {
            var steals = await _repo.GetStealsLeagueLeaders(page);
            return Ok(steals);
        }

        [HttpGet("leagueleadersfouls/{page}")]
        public async Task<IActionResult> GetFoulLeagueLeaders(GetStatLeagueDto page)
        {
            var fouls = await _repo.GetFoulsLeagueLeaders(page);
            return Ok(fouls);
        }

        [HttpGet("leagueleadersminutes/{page}")]
        public async Task<IActionResult> GetMinutesLeagueLeaders(GetStatLeagueDto page)
        {
            var minutes = await _repo.GetMinutesLeagueLeaders(page);
            return Ok(minutes);
        }

        [HttpGet("leagueleadersturnovers/{page}")]
        public async Task<IActionResult> GetTurnoversLeagueLeaders(GetStatLeagueDto page)
        {
            var tos = await _repo.GetTurnoversLeagueLeaders(page);
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

        [HttpGet("gettopfivesteals/{leagueId")]
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

        [HttpGet("getplayoffsummariesforround/{summary}")]
        public async Task<IActionResult> GetPlayoffSummariesForRound(GetPlayoffSummaryDto round)
        {
            var results = await _repo.GetPlayoffSummariesForRound(round);
            return Ok(results);
        }

         [HttpGet("playoffleagueleaderspoints/{page}")]
        public async Task<IActionResult> GetPointLeagueLeadersPlayoffs(GetStatLeagueDto page)
        {
            var points = await _repo.GetPlayoffsPointsLeagueLeaders(page);
            return Ok(points);
        }

        [HttpGet("getcountofpointsleadersplayoffs/{leagueId}")]
        public async Task<IActionResult> GetCountOfLeagueLeadersPlayoffs(int leagueId)
        {
            var count = _repo.GetCountOfPointsLeagueLeadersPlayoffs(leagueId);
            return Ok(count);
        }

        [HttpGet("leagueleadersassistsplayoffs/{page}")]
        public async Task<IActionResult> GetAssistLeagueLeadersPlayoffs(GetStatLeagueDto page)
        {
            var assists = await _repo.GetPlayoffAssistsLeagueLeaders(page);
            return Ok(assists);
        }

        [HttpGet("leagueleadersreboundsplayoffs/{page}")]
        public async Task<IActionResult> GetReboundLeagueLeadersPlayoffs(GetStatLeagueDto page)
        {
            var rebounds = await _repo.GetPlayoffReboundsLeagueLeaders(page);
            return Ok(rebounds);
        }

        [HttpGet("leagueleadersblocksplayoffs/{page}")]
        public async Task<IActionResult> GetBlockLeagueLeadersPlayoffs(GetStatLeagueDto page)
        {
            var blocks = await _repo.GetPlayoffBlocksLeagueLeaders(page);
            return Ok(blocks);
        }

        [HttpGet("leagueleadersstealsplayoffs/{page}")]
        public async Task<IActionResult> GetStealLeagueLeadersPlayoffs(GetStatLeagueDto page)
        {
            var steals = await _repo.GetPlayoffStealsLeagueLeaders(page);
            return Ok(steals);
        }

        [HttpGet("leagueleadersminutesplayoffs/{page}")]
        public async Task<IActionResult> GetMinutesLeagueLeadersPlayoffs(GetStatLeagueDto page)
        {
            var minutes = await _repo.GetPlayoffMinutesLeagueLeaders(page);
            return Ok(minutes);
        }

        [HttpGet("leagueleadersturnoversplayoffs/{page}")]
        public async Task<IActionResult> GetTurnoversLeagueLeadersPlayoffs(GetStatLeagueDto page)
        {
            var tos = await _repo.GetPlayoffTurnoversLeagueLeaders(page);
            return Ok(tos);
        }

        [HttpGet("leagueleadersfoulsplayoffs/{page}")]
        public async Task<IActionResult> GetFoulLeagueLeadersPlayoffs(GetStatLeagueDto page)
        {
            var fouls = await _repo.GetPlayoffFoulsLeagueLeaders(page);
            return Ok(fouls);
        }

        [HttpGet("getchampion")]
        public async Task<IActionResult> GetChampion()
        {
            var team = await _repo.GetChampion();
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