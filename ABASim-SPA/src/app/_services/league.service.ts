import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { League } from '../_models/league';
import { LeagueState } from '../_models/leagueState';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { GameDisplay } from '../_models/gameDisplay';
import { GameDisplayCurrent } from '../_models/gameDisplayCurrent';
import { Standing } from '../_models/standing';
import { Schedule } from '../_models/schedule';
import { Transaction } from '../_models/transaction';
import { PlayByPlay } from '../_models/playByPlay';
import { GameDetails } from '../_models/gameDetails';
import { LeagueScoring } from '../_models/leagueScoring';
import { LeagueRebounding } from '../_models/leagueRebounding';
import { LeagueOther } from '../_models/leagueOther';
import { LeagueDefence } from '../_models/leagueDefence';
import { LeagueLeadersPoints } from '../_models/leagueLeadersPoints';
import { LeagueLeadersAssists } from '../_models/leagueLeadersAssists';
import { LeagueLeadersRebounds } from '../_models/leagueLeadersRebounds';
import { LeagueLeadersBlocks } from '../_models/leagueLeadersBlocks';
import { LeagueLeadersSteals } from '../_models/leagueLeadersSteals';
import { LeagueLeadersTurnover } from '../_models/leagueLeadersTurnovers';
import { LeagueLeadersFouls } from '../_models/leagueLeadersFouls';
import { LeagueLeadersMinutes } from '../_models/leagueLeadersMinutes';
import { PlayoffSummary } from '../_models/playoffSummary';
import { PlayoffResultsComponent } from '../playoff-results/playoff-results.component';
import { PlayoffResult } from '../_models/playoffResult';
import { Team } from '../_models/team';
import { LeaguePlayerInjury } from '../_models/leaguePlayerInjury';
import { Votes } from '../_models/votes';
import { GetPlayoffSummary } from '../_models/getPlayoffSummary';
import { GetGameLeague } from '../_models/getGameLeague';
import { GetStatsLeague } from '../_models/getStatsLeague';
import { GetScheduleLeague } from '../_models/getScheduleLeague';
import { GetStandingLeague } from '../_models/getStandingLeague';

@Injectable({
  providedIn: 'root'
})
export class LeagueService {
  baseUrl = environment.apiUrl + '/league/';

  constructor(private http: HttpClient) { }

  getLeague(leagueId: number) {
    return this.http.get<League>(this.baseUrl + 'getleague');
  }

  getLeagueForUserId(userId: number) {
    return this.http.get<League>(this.baseUrl + 'getleagueforuser/' + userId);
  }

  getLeagueStatuses(): Observable<LeagueState[]> {
    return this.http.get<LeagueState[]>(this.baseUrl + 'getleaguestatus');
  }

  getLeagueStatusForId(stateId: number) {
    console.log('state id: ' + stateId);
    return this.http.get<LeagueState>(this.baseUrl + 'getleaguestateforid/' + stateId);
  }

  getPreseasonGamesForTomorrow(leagueId: number): Observable<GameDisplay[]> {
    return this.http.get<GameDisplay[]>(this.baseUrl + 'getgamesfortomorrow/' + leagueId);
  }

  getPreseasonGamesForToday(leagueId: number): Observable<GameDisplayCurrent[]> {
    return this.http.get<GameDisplayCurrent[]>(this.baseUrl + 'getgamesfortoday/' + leagueId);
  }

  getConferenceStandings(conference: GetStandingLeague): Observable<Standing[]> {
    return this.http.get<Standing[]>(this.baseUrl + 'getstandingsforconference/' + conference);
  }

  getDivisionStandings(division: GetStandingLeague): Observable<Standing[]> {
    return this.http.get<Standing[]>(this.baseUrl + 'getstandingsfordivision/' + division);
  }

  getLeagueStandings(leagueId: number): Observable<Standing[]> {
    return this.http.get<Standing[]>(this.baseUrl + 'getstandingsforleague/' + leagueId);
  }

  getScheduleGames(day: GetScheduleLeague): Observable<Schedule[]> {
    return this.http.get<Schedule[]>(this.baseUrl + 'getscheduledisplay/' + day);
  }

  getPlayoffGames(day: GetScheduleLeague): Observable<PlayoffResult[]> {
    return this.http.get<PlayoffResult[]>(this.baseUrl + 'getplayoffdisplay/' + day);
  }

  getTransactions(leagueId: number): Observable<Transaction[]> {
    return this.http.get<Transaction[]>(this.baseUrl + 'gettransactions/' + leagueId);
  }

  getPlayByPlaysForId(gammeleague: GetGameLeague): Observable<PlayByPlay[]> {
    return this.http.get<PlayByPlay[]>(this.baseUrl + 'getgameplaybyplay/' + gammeleague);
  }

  getPlayoffsPlayByPlaysForId(gameleague: GetGameLeague): Observable<PlayByPlay[]> {
    return this.http.get<PlayByPlay[]>(this.baseUrl + 'getgameplaybyplayplayoffs/' + gameleague);
  }

  getGameDetailsPreseason(gameLeague: GetGameLeague) {
    return this.http.get<GameDetails>(this.baseUrl + 'getpreseasongamedetails/ ' + gameLeague);
  }

  getGameDetailsSeason(gameLeague: GetGameLeague) {
    return this.http.get<GameDetails>(this.baseUrl + 'getseasongamedetails/ ' + gameLeague);
  }

  getGameDetailsPlayoffs(gameLeague: GetGameLeague) {
    return this.http.get<GameDetails>(this.baseUrl + 'getplayoffgamedetails/ ' + gameLeague);
  }

  getSeasonGamesForTomorrow(leagueId: number): Observable<GameDisplay[]> {
    return this.http.get<GameDisplay[]>(this.baseUrl + 'getgamesfortomorrowseason/' + leagueId);
  }

  getSeasonGamesForToday(leagueId: number): Observable<GameDisplayCurrent[]> {
    return this.http.get<GameDisplayCurrent[]>(this.baseUrl + 'getgamesfortodayseason/' + leagueId);
  }

  getFirstRoundGamesForToday(leagueId: number): Observable<GameDisplayCurrent[]> {
    return this.http.get<GameDisplayCurrent[]>(this.baseUrl + 'getfirstroundgamesfortoday/' + leagueId);
  }

  getFirstRoundSummaries(summary: GetPlayoffSummary): Observable<PlayoffSummary[]> {
    return this.http.get<PlayoffSummary[]>(this.baseUrl + 'getplayoffsummariesforround/' + summary);
  }

  getPointsLeagueLeadersForPage(page: GetStatsLeague): Observable<LeagueLeadersPoints[]> {
    return this.http.get<LeagueLeadersPoints[]>(this.baseUrl + 'leagueleaderspoints/' + page);
  }

  getCountOfPointsLeagueLeaders(leagueId: number) {
    return this.http.get<number>(this.baseUrl + 'getcountofpointsleaders/' + leagueId);
  }

  getAssistsLeagueLeadersForPage(page: GetStatsLeague): Observable<LeagueLeadersAssists[]> {
    return this.http.get<LeagueLeadersAssists[]>(this.baseUrl + 'leagueleadersassists/' + page);
  }

  getReboundsLeagueLeadersForPage(page: GetStatsLeague): Observable<LeagueLeadersRebounds[]> {
    return this.http.get<LeagueLeadersRebounds[]>(this.baseUrl + 'leagueleadersrebounds/' + page);
  }

  getBlocksLeagueLeadersForPage(page: GetStatsLeague): Observable<LeagueLeadersBlocks[]> {
    return this.http.get<LeagueLeadersBlocks[]>(this.baseUrl + 'leagueleadersblocks/' + page);
  }

  getStealsLeagueLeadersForPage(page: GetStatsLeague): Observable<LeagueLeadersSteals[]> {
    return this.http.get<LeagueLeadersSteals[]>(this.baseUrl + 'leagueleaderssteals/' + page);
  }

  getTurnoversLeagueLeadersForPage(page: GetStatsLeague): Observable<LeagueLeadersTurnover[]> {
    return this.http.get<LeagueLeadersTurnover[]>(this.baseUrl + 'leagueleadersturnovers/' + page);
  }

  getFoulsLeagueLeadersForPage(page: GetStatsLeague): Observable<LeagueLeadersFouls[]> {
    return this.http.get<LeagueLeadersFouls[]>(this.baseUrl + 'leagueleadersfouls/' + page);
  }

  getMinutesLeagueLeadersForPage(page: GetStatsLeague): Observable<LeagueLeadersMinutes[]> {
    return this.http.get<LeagueLeadersMinutes[]>(this.baseUrl + 'leagueleadersminutes/' + page);
  }

  getTopFivePoints(leagueId: number): Observable<LeagueLeadersPoints[]> {
    return this.http.get<LeagueLeadersPoints[]>(this.baseUrl + 'gettopfivepoints/' + leagueId);
  }

  getTopFiveAssists(leagueId: number): Observable<LeagueLeadersAssists[]> {
    return this.http.get<LeagueLeadersAssists[]>(this.baseUrl + 'gettopfiveassists/' + leagueId);
  }

  getTopFiveSteals(leagueId: number): Observable<LeagueLeadersSteals[]> {
    return this.http.get<LeagueLeadersSteals[]>(this.baseUrl + 'gettopfivesteals/' + leagueId);
  }

  getTopFiveRebounds(leagueId: number): Observable<LeagueLeadersRebounds[]> {
    return this.http.get<LeagueLeadersRebounds[]>(this.baseUrl + 'gettopfiverebounds/' + leagueId);
  }

  getTopFiveBlocks(leagueId: number): Observable<LeagueLeadersBlocks[]> {
    return this.http.get<LeagueLeadersBlocks[]>(this.baseUrl + 'gettopfiveblocks/' + leagueId);
  }

  getPlayoffsPointsLeagueLeadersForPage(page: GetStatsLeague): Observable<LeagueLeadersPoints[]> {
    return this.http.get<LeagueLeadersPoints[]>(this.baseUrl + 'playoffleagueleaderspoints/' + page);
  }

  getCountOfPointsLeagueLeadersPlayoffs(leagueId: number) {
    return this.http.get<number>(this.baseUrl + 'getcountofpointsleadersplayoffs/' + leagueId);
  }

  getChampion() {
    return this.http.get<Team>(this.baseUrl + 'getchampion');
  }

  getPlayoffsAssistsLeagueLeadersForPage(page: GetStatsLeague): Observable<LeagueLeadersAssists[]> {
    return this.http.get<LeagueLeadersAssists[]>(this.baseUrl + 'leagueleadersassistsplayoffs/' + page);
  }

  getPlayoffsReboundsLeagueLeadersForPage(page: GetStatsLeague): Observable<LeagueLeadersRebounds[]> {
    return this.http.get<LeagueLeadersRebounds[]>(this.baseUrl + 'leagueleadersreboundsplayoffs/' + page);
  }

  getPlayoffsBlocksLeagueLeadersForPage(page: GetStatsLeague): Observable<LeagueLeadersBlocks[]> {
    return this.http.get<LeagueLeadersBlocks[]>(this.baseUrl + 'leagueleadersblocksplayoffs/' + page);
  }

  getPlayoffsStealsLeagueLeadersForPage(page: GetStatsLeague): Observable<LeagueLeadersSteals[]> {
    return this.http.get<LeagueLeadersSteals[]>(this.baseUrl + 'leagueleadersstealsplayoffs/' + page);
  }

  getPlayoffsTurnoversLeagueLeadersForPage(page: GetStatsLeague): Observable<LeagueLeadersTurnover[]> {
    return this.http.get<LeagueLeadersTurnover[]>(this.baseUrl + 'leagueleadersturnoversplayoffs/' + page);
  }

  getPlayoffsFoulsLeagueLeadersForPage(page: GetStatsLeague): Observable<LeagueLeadersFouls[]> {
    return this.http.get<LeagueLeadersFouls[]>(this.baseUrl + 'leagueleadersfoulsplayoffs/' + page);
  }

  getPlayoffsMinutesLeagueLeadersForPage(page: GetStatsLeague): Observable<LeagueLeadersMinutes[]> {
    return this.http.get<LeagueLeadersMinutes[]>(this.baseUrl + 'leagueleadersminutesplayoffs/' + page);
  }

  getYesterdaysTransactins(leagueId: number): Observable<Transaction[]> {
    return this.http.get<Transaction[]>(this.baseUrl + 'getyesterdaystransactions/' + leagueId);
  }

  getLeagueInjuries(leagueId: number): Observable<LeaguePlayerInjury[]> {
    return this.http.get<LeaguePlayerInjury[]>(this.baseUrl + 'getleagueplayerinjuries/' + leagueId);
  }

  getMvpTopFive(leagueId: number): Observable<Votes[]> {
    return this.http.get<Votes[]>(this.baseUrl + 'getmvptopfive/' + leagueId);
  }

  getSixthManTopFive(leagueId: number): Observable<Votes[]> {
    return this.http.get<Votes[]>(this.baseUrl + 'getsixthmantopfive/' + leagueId);
  }

  getDpoyTopFive(leagueId: number): Observable<Votes[]> {
    return this.http.get<Votes[]>(this.baseUrl + 'getdpoytopfive/' + leagueId);
  }

  getAllNBATeams(leagueId: number): Observable<Votes[]> {
    return this.http.get<Votes[]>(this.baseUrl + 'getallnbateams/' + leagueId);
  }

  checkPrivateLeagueTeams() {
    return this.http.get<boolean>(this.baseUrl + 'checkavailableteamsprivate');
  }

  checkLeagueCode(leaguecode: string) {
    console.log(leaguecode);
    return this.http.get<boolean>(this.baseUrl + 'checkleaguecode/' + leaguecode);
  }

}
