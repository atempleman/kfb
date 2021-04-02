import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { LeagueState } from '../_models/leagueState';
import { environment } from 'src/environments/environment';
import { CheckGame } from '../_models/checkGame';
import { Observable } from 'rxjs';
import { GameDisplayCurrent } from '../_models/gameDisplayCurrent';
import { LeagueStatusUpdate } from '../_models/leagueStatusUpdate';
import { GetRosterQuickView } from '../_models/getRosterQuickView';
import { GetScheduleLeague } from '../_models/getScheduleLeague';
import { GetGameLeague } from '../_models/getGameLeague';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl + '/admin/';

  constructor(private http: HttpClient) { }

  updateLeagueStatus(newStatus: LeagueStatusUpdate) {
    return this.http.get<boolean>(this.baseUrl + 'updateleaguestatus/' + newStatus);
  }

  removeTeamRegistration(teamId: GetRosterQuickView) {
    return this.http.get<boolean>(this.baseUrl + 'removeteamrego/' + teamId);
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
    return this.http.get<boolean>(this.baseUrl + 'changeday/' + day);
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

  resetGame(gameId: GetGameLeague) {
    return this.http.get<boolean>(this.baseUrl + 'resetgame/' + gameId);
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
}
