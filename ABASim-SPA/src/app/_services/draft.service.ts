import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { AddDraftRank } from '../_models/addDraftRank';
import { DraftPlayer } from '../_models/draftPlayer';
import { DraftTracker } from '../_models/draftTracker';
import { Observable } from 'rxjs';
import { InitialDraftPicks } from '../_models/initialDraftPicks';
import { Team } from '../_models/team';
import { DraftSelection } from '../_models/draftSelection';
import { DraftPick } from '../_models/draftPick';
import { environment } from 'src/environments/environment';
import { DashboardDraftPick } from '../_models/dashboardDraftPick';
import { InitialPickSalary } from '../_models/initialPickSalary';
import { GetDashboardPicks } from '../_models/getDashboardPicks';
import { GetRosterQuickView } from '../_models/getRosterQuickView';
import { GetPlayoffSummary } from '../_models/getPlayoffSummary';
import { RegularDraftContract } from '../_models/regularDraftContract';

@Injectable({
  providedIn: 'root'
})
export class DraftService {
  baseUrl = environment.apiUrl + '/draft/';

  constructor(private http: HttpClient) { }

  addDraftPlayerRanking(newRanking: AddDraftRank) {
    return this.http.post(this.baseUrl + 'adddraftrank', newRanking);
  }

  removeDraftPlayerRanking(ranking: AddDraftRank) {
    return this.http.post(this.baseUrl + 'removedraftrank', ranking);
  }

  getDraftBoardForTeam(qv: GetRosterQuickView): Observable<DraftPlayer[]> {
    const params = new HttpParams()
      .set('teamId', qv.teamId.toString())
      .set('leagueId', qv.leagueId.toString());
    return this.http.get<DraftPlayer[]>(this.baseUrl + 'getdraftboard', {params});
  }

  moveRankingUp(player: AddDraftRank) {
    return this.http.post(this.baseUrl + 'moveup', player);
  }

  moveRankingDown(player: AddDraftRank) {
    return this.http.post(this.baseUrl + 'movedown', player);
  }

  beginInitialDraft(leagueId: number) {
    return this.http.get<boolean>(this.baseUrl + 'beginInitialDraft/' + leagueId);
  }

  getDraftTracker(leagueId: number) {
    return this.http.get<DraftTracker>(this.baseUrl + 'getdrafttracker/' + leagueId);
  }

  getInitialDraftPicks(leagueId: number): Observable<InitialDraftPicks[]> {
    return this.http.get<InitialDraftPicks[]>(this.baseUrl + 'getinitialdraftpicks/' + leagueId);
  }

  getCurrentInitialDraftPick(leagueId: number): Observable<InitialDraftPicks> {
    return this.http.get<InitialDraftPicks>(this.baseUrl + 'getcurrentinitialdraftpick/' + leagueId);
  }

  makeDraftPick(draftPick: DraftSelection) {
    return this.http.post(this.baseUrl + 'initialdraftselection', draftPick);
  }

  makeAutoPick(draftPick: DraftSelection) {
    return this.http.post(this.baseUrl + 'makeautopick', draftPick);
  }

  getDraftPicksForRound(page: GetPlayoffSummary): Observable<DraftPick[]> {
    const params = new HttpParams()
      .set('round', page.round.toString())
      .set('leagueId', page.leagueId.toString());
    return this.http.get<DraftPick[]>(this.baseUrl + 'getinitialdraftpicksforround', {params});
  }

  getDashboardPicks(pick: GetDashboardPicks): Observable<DashboardDraftPick> {
    const params = new HttpParams()
      .set('pick', pick.pick.toString())
      .set('leagueId', pick.leagueId.toString());
    return this.http.get<DashboardDraftPick>(this.baseUrl + 'getdashboardcurrentpick', {params});
  }

  getInitialDraftSalaryDetails(): Observable<InitialPickSalary[]> {
    return this.http.get<InitialPickSalary[]>(this.baseUrl + 'getinitialdraftsalarydetails');
  }

  getRegularSeasonDraftSalaryDetails(): Observable<RegularDraftContract[]> {
    return this.http.get<RegularDraftContract[]>(this.baseUrl + 'getregulardraftsalarydetails');
  }
}
