import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DraftPlayer } from '../_models/draftPlayer';
import { Player } from '../_models/player';
import { environment } from 'src/environments/environment';
import { CompletePlayer } from '../_models/completePlayer';
import { CareerStats } from '../_models/careerStats';
import { PlayerContractQuickView } from '../_models/playerContractQuickView';
import { PlayerContract } from '../_models/playerContract';
import { RetiredPlayer } from '../_models/retiredPlayer';
import { DetailedRetiredPlayer } from '../_models/detailedRetiredPlayer';
import { GetPlayerLeague } from '../_models/getPlayerLeague';
import { GetPlayerIdLeague } from '../_models/getPlayerIdLeague';
import { GetPlayoffSummary } from '../_models/getPlayoffSummary';
import { DraftSelectionPlayer } from '../_models/draftSelectionPlayer';

@Injectable({
  providedIn: 'root'
})
export class PlayerService {
  baseUrl = environment.apiUrl + '/player/';

  constructor(private http: HttpClient) { }

  getInitialDraftPlayers(page: GetPlayoffSummary): Observable<DraftPlayer[]> {
    const params = new HttpParams()
      .set('page', page.round.toString())
      .set('leagueId', page.leagueId.toString());
    return this.http.get<DraftPlayer[]>(this.baseUrl + 'getinitialdraftplayerspage', {params});
  }

  getAllInitialDraftPlayers(leagueId: number): Observable<DraftPlayer[]> {
    return this.http.get<DraftPlayer[]>(this.baseUrl + 'getinitialdraftplayers/' + leagueId);
  }

  getAllInitialDraftSelectionPlayers(leagueId: number): Observable<DraftSelectionPlayer[]> {
    return this.http.get<DraftSelectionPlayer[]>(this.baseUrl + 'getinitialdraftselectionplayers/' + leagueId);
  }

  getCountOfAvailableDraftPlayers(leagueId: number) {
    return this.http.get<number>(this.baseUrl + 'getcountofdraftplayers/' + leagueId);
  }

  getPlayerForId(playerId: number) {
    return this.http.get<Player>(this.baseUrl + 'getplayerforid/' + playerId);
  }

  getAllPlayers(leagueId: number): Observable<Player[]> {
    let players = this.http.get<Player[]>(this.baseUrl + 'getallplayers/' + leagueId);
    return players
  }

  filterPlayers(value: string): Observable<Player[]> {
    return this.http.get<Player[]>(this.baseUrl + 'filterplayers/' + value);
  }

  getFreeAgents(leagueId: number): Observable<Player[]> {
    return this.http.get<Player[]>(this.baseUrl + 'getfreeagents/' + leagueId);
  }

  getFreeAgentsByPos(pos: GetPlayerIdLeague): Observable<Player[]> {
    const params = new HttpParams()
      .set('playerId', pos.playerId.toString())
      .set('leagueId', pos.leagueId.toString());

    return this.http.get<Player[]>(this.baseUrl + 'getfreeagentsbypos', {params});
  }

  filterFreeAgents(value: GetPlayerLeague): Observable<Player[]> {
    const params = new HttpParams()
      .set('filter', value.playerName.toString())
      .set('leagueId', value.leagueId.toString());

    return this.http.get<Player[]>(this.baseUrl + 'getfilteredfreeagents', {params});
  }

  playerForPlayerProfileById(pil: GetPlayerIdLeague) {
    const params = new HttpParams()
      .set('playerId', pil.playerId.toString())
      .set('leagueId', pil.leagueId.toString());

    return this.http.get<CompletePlayer>(this.baseUrl + 'getcompleteplayer', {params});
  }

  filterDraftPlayerPool(value: GetPlayerLeague): Observable<DraftPlayer[]> {
    const params = new HttpParams()
      .set('filter', value.playerName.toString())
      .set('leagueId', value.leagueId.toString());
    return this.http.get<DraftPlayer[]>(this.baseUrl + 'filterdraftplayers', {params});
  }

  getDraftPlayerPoolByPos(pos: GetPlayerIdLeague): Observable<DraftPlayer[]> {
    const params = new HttpParams()
      .set('filter', pos.playerId.toString())
      .set('leagueId', pos.leagueId.toString());
    return this.http.get<DraftPlayer[]>(this.baseUrl + 'draftpoolfilterbyposition', {params});
  }

  getPlayerByPos(pos: GetPlayerIdLeague): Observable<Player[]> {
    const params = new HttpParams()
      .set('filter', pos.playerId.toString())
      .set('leagueId', pos.leagueId.toString());

    return this.http.get<Player[]>(this.baseUrl + 'filterbyposition', {params});
  }

  getCareerStats(pil: GetPlayerIdLeague): Observable<CareerStats[]> {
    const params = new HttpParams()
      .set('playerId', pil.playerId.toString())
      .set('leagueId', pil.leagueId.toString());

    return this.http.get<CareerStats[]>(this.baseUrl + 'getcareerstats', {params});
  }

  getPlayerForName(pl: GetPlayerLeague): Observable<Player> {
    const params = new HttpParams()
      .set('playername', pl.playerName.toString())
      .set('leagueId', pl.leagueId.toString());

    return this.http.get<Player>(this.baseUrl + 'getplayerforname', {params});
  }

  getContractForPlayer(pil: GetPlayerIdLeague): Observable<PlayerContractQuickView> {
    const params = new HttpParams()
      .set('playerId', pil.playerId.toString())
      .set('leagueId', pil.leagueId.toString());

    return this.http.get<PlayerContractQuickView>(this.baseUrl + 'getcontractforplayer', {params});
  }

  getPlayerContractForPlayer(pil: GetPlayerIdLeague): Observable<PlayerContract> {
    const params = new HttpParams()
      .set('playerId', pil.playerId.toString())
      .set('leagueId', pil.leagueId.toString());

    return this.http.get<PlayerContract>(this.baseUrl + 'getfullcontractforplayer', {params});
  }

  getRetiredPlayers(): Observable<RetiredPlayer[]> {
    return this.http.get<RetiredPlayer[]>(this.baseUrl + 'getretiredplayers/');
  }

  getDetailedRetiredPlayer(playerId: number): Observable<DetailedRetiredPlayer> {
    return this.http.get<DetailedRetiredPlayer>(this.baseUrl + 'getdetailedretiredplayer/' + playerId);
  }
}
