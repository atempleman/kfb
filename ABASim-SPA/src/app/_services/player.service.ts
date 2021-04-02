import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
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

@Injectable({
  providedIn: 'root'
})
export class PlayerService {
  baseUrl = environment.apiUrl + '/player/';

  constructor(private http: HttpClient) { }

  getInitialDraftPlayers(page: number): Observable<DraftPlayer[]> {
    return this.http.get<DraftPlayer[]>(this.baseUrl + 'getinitialdraftplayers/' + page);
  }

  getAllInitialDraftPlayers(): Observable<DraftPlayer[]> {
    return this.http.get<DraftPlayer[]>(this.baseUrl + 'getinitialdraftplayers');
  }

  getCountOfAvailableDraftPlayers() {
    return this.http.get<number>(this.baseUrl + 'getcountofdraftplayers');
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
    return this.http.get<Player[]>(this.baseUrl + 'getfreeagentsbypos/' + pos);
  }

  filterFreeAgents(value: GetPlayerLeague): Observable<Player[]> {
    return this.http.get<Player[]>(this.baseUrl + 'getfilteredfreeagents/' + value);
  }

  playerForPlayerProfileById(playerIdLeague: GetPlayerIdLeague) {
    return this.http.get<CompletePlayer>(this.baseUrl + 'getcompleteplayer/' + playerIdLeague);
  }

  filterDraftPlayerPool(value: string): Observable<DraftPlayer[]> {
    return this.http.get<DraftPlayer[]>(this.baseUrl + 'filterdraftplayers/' + value);
  }

  getDraftPlayerPoolByPos(pos: number): Observable<DraftPlayer[]> {
    return this.http.get<DraftPlayer[]>(this.baseUrl + 'draftpoolfilterbyposition/' + pos);
  }

  getPlayerByPos(pos: GetPlayerIdLeague): Observable<Player[]> {
    console.log('ash2');
    return this.http.get<Player[]>(this.baseUrl + 'filterbyposition/' + pos);
  }

  getCareerStats(playerId: GetPlayerIdLeague): Observable<CareerStats[]> {
    return this.http.get<CareerStats[]>(this.baseUrl + 'getcareerstats/' + playerId);
  }

  getPlayerForName(playerleague: GetPlayerLeague): Observable<Player> {
    return this.http.get<Player>(this.baseUrl + 'getplayerforname/' + playerleague);
  }

  getContractForPlayer(playerId: GetPlayerIdLeague): Observable<PlayerContractQuickView> {
    return this.http.get<PlayerContractQuickView>(this.baseUrl + 'getcontractforplayer/' + playerId);
  }

  getPlayerContractForPlayer(playerId: GetPlayerIdLeague): Observable<PlayerContract> {
    return this.http.get<PlayerContract>(this.baseUrl + 'getfullcontractforplayer/' + playerId);
  }

  getRetiredPlayers(): Observable<RetiredPlayer[]> {
    return this.http.get<RetiredPlayer[]>(this.baseUrl + 'getretiredplayers/');
  }

  getDetailedRetiredPlayer(playerId: number): Observable<DetailedRetiredPlayer> {
    return this.http.get<DetailedRetiredPlayer>(this.baseUrl + 'getdetailedretiredplayer/' + playerId);
  }
}
