import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { LeagueState } from '../_models/leagueState';
import { environment } from 'src/environments/environment';
import { CheckGame } from '../_models/checkGame';
import { Observable } from 'rxjs';
import { GameDisplayCurrent } from '../_models/gameDisplayCurrent';
import { LeagueStatusUpdate } from '../_models/leagueStatusUpdate';
import { GetRosterQuickView } from '../_models/getRosterQuickView';
import { GetScheduleLeague } from '../_models/getScheduleLeague';
import { GetGameLeague } from '../_models/getGameLeague';
import { League } from '../_models/league';
import { RolloverStatus } from '../_models/rolloverStatus';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl + '/admin/';

  constructor(private http: HttpClient) { }

  updateLeagueStatus(newStatus: LeagueStatusUpdate) {
    const params = new HttpParams()
      .set('status', newStatus.status.toString())
      .set('leagueId', newStatus.leagueId.toString());
    return this.http.get<boolean>(this.baseUrl + 'updateleaguestatus', {params});
  }

  removeTeamRegistration(qv: GetRosterQuickView) {
    const params = new HttpParams()
      .set('teamId', qv.teamId.toString())
      .set('leagueId', qv.leagueId.toString());

    return this.http.get<boolean>(this.baseUrl + 'removeteamrego', {params});
  }

  runInitialDraftLottery(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'runinitialdraftlottery/' + leagueId);
  }

  checkAllGamesRun(leagueId: number): Observable<boolean> {
    return this.http.get<boolean>(this.baseUrl + 'checkgamesrun/' + leagueId);
  }

  rolloverDay(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'rolloverday/' + leagueId);
  }

  changeDay(day: GetScheduleLeague) {
    const params = new HttpParams()
      .set('day', day.day.toString())
      .set('leagueId', day.leagueId.toString());
    return this.http.get<boolean>(this.baseUrl + 'changeday', {params});
  }

  beginPlayoffs(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'beginplayoffs/' + leagueId);
  }

  beginConfSemis(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'beginconfsemis/' + leagueId);
  }

  beginConfFinals(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'beginconffinals/' + leagueId);
  }

  beginFinals(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'beginfinals/' + leagueId);
  }

  endSeason(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'endseason/' + leagueId);
  }

  runTeamDraftPicksSetup(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'runteamdraftpicks/' + leagueId);
  }

  generateInitalContracts(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'generateinitialcontracts/' + leagueId);
  }

  generateInitialSalaryCaps(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'generateinitialsalarycaps/' + leagueId);
  }

  generateAutoPicks(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'testautopickordering/' + leagueId);
  }

  getGamesForReset(leagueId: number): Observable<GameDisplayCurrent[]> {
    return this.http.get<GameDisplayCurrent[]>(this.baseUrl + 'getgamesforreset/' + leagueId);
  }

  resetGame(gl: GetGameLeague) {
    const params = new HttpParams()
      .set('gameId', gl.gameId.toString())
      .set('leagueId', gl.leagueId.toString());
    return this.http.get<boolean>(this.baseUrl + 'resetgame', {params});
  }

  rolloverSeasonStats(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'rolloverseasonstats/' + leagueId);
  }

  rolloverAwardWinners(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'rolloverawards/' + leagueId);
  }

  rolloverContractUpdates(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'rollovercontractupdates/' + leagueId);
  }

  generateDraft(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'generatedraft/' + leagueId);
  }

  deletePreseasonAndPlayoffsData(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'deletepreseasonplayoffs/' + leagueId);
  }

  deleteTeamSettingsData(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'deleteteamsettings/' + leagueId);
  }

  deleteAwardsData(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'deleteawards/' + leagueId);
  }

  deleteOtherData(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'deleteother/' + leagueId);
  }

  deleteSeasonData(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'deleteseason/' + leagueId);
  }

  resetStandings(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'resetstandings/' + leagueId);
  }

  rolloverLeague(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'rolloverleague/' + leagueId);
  }

  resetLeague(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'resetleague/' + leagueId);
  }
  
  createNewLeague(newLeague: League) {
    // return this.http.post(this.baseUrl + 'createnewleague', newLeague);
    console.log(newLeague);
    const params = new HttpParams()
      .set('leagueName', newLeague.leagueName)
      .set('leagueCode', newLeague.leagueCode)
      .set('year', newLeague.year.toString());
    return this.http.get<boolean>(this.baseUrl + 'createnewleague', {params});
  }

  getLeagueRolloverStatus(leagueId: number) {
    return this.http.get<RolloverStatus>(this.baseUrl + 'getrolloverstatus/' + leagueId);
  }

  saveSeasonResults(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'saveseasonresults/' + leagueId);
  }

  rolloverDeletePlayerData(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'rolloverdeleteplayerdata/' + leagueId);
  }

  rolloverDeletePlayByPlaySeason(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'rolloverdeleteplaybyplayseason/' + leagueId);
  }

  rolloverDeletePlayByPlayPlayoffs(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'rolloverdeleteplaybyplayplayoffs/' + leagueId);
  }

  rolloverDeletePlayoffSerieses(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'rolloverdeleteplayoffserieses/' + leagueId);
  }

  rolloverDeleteGameResults(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'rolloverdeletegameresults/' + leagueId);
  }

  rolloverDeleteBoxScores(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'rolloverdeleteboxscores/' + leagueId);
  }

  rolloverDeleteAwards(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'rolloverdeleteawards/' + leagueId);
  }

  rolloverLoadNextDraftPicks(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'rolloverloadnextdraftpicks/' + leagueId);
  }

  rolloverDeleteTeamSettings(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'rolloverteamsettings/' + leagueId);
  }

  rolloverDeleteMessagesAndOffers(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'rolloverdeletemessagesandoffers/' + leagueId);
  }

  rolloverRetiredPlayers(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'rolloverretiredplayers/' + leagueId);
  }

  rolloverUpdateContractsAndPlayers(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'rolloverupdatecontractsandplayers/' + leagueId);
  }

  rolloverDeletePlayerDetailsData(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'rolloverdeleteplayerdetailsdata/' + leagueId);
  }

  rolloverUpdateSeason(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'rolloverupdateseason/' + leagueId);
  }

  rolloverAddPlayerData(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'rolloveraddplayerdata/' + leagueId);
  }

  rolloverSetPlayerTeams(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'rolloversetplayerteams/' + leagueId);
  }

  rolloverFinishUp(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'rolloverfinishup/' + leagueId);
  }

  runSeasonDraftLottery(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'runseasondraftlottery/' + leagueId);
  }
}
