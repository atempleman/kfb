import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { SimGame } from '../_models/simGame';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { BoxScore } from '../_models/boxScore';
import { GetGameLeague } from '../_models/getGameLeague';

@Injectable({
  providedIn: 'root'
})
export class GameEngineService {

  baseUrl = environment.apiUrl + '/gameEngine/';

  constructor(private http: HttpClient) { }

  startTestGame(game: SimGame) {
    return this.http.post(this.baseUrl + 'startGame', game);
  }

  startPreseasonGame(game: SimGame) {
    return this.http.post(this.baseUrl + 'startPreseasonGame', game);
  }

  startSeasonGame(game: SimGame) {
    return this.http.post(this.baseUrl + 'startSeasonGame', game);
  }

  startPlayoffGame(game: SimGame) {
    return this.http.post(this.baseUrl + 'startPlayoffGame', game);
  }

  getBoxScoreForGameId(gl: GetGameLeague): Observable<BoxScore[]> {
    const params = new HttpParams()
      .set('gameId', gl.gameId.toString())
      .set('leagueId', gl.leagueId.toString());

    return this.http.get<BoxScore[]>(this.baseUrl + 'getboxscoresforgameid', {params});
  }

  getBoxScoreForGameIdPlayoffs(gl: GetGameLeague): Observable<BoxScore[]> {
    const params = new HttpParams()
      .set('gameId', gl.gameId.toString())
      .set('leagueId', gl.leagueId.toString());

    return this.http.get<BoxScore[]>(this.baseUrl + 'getboxscoresforgameidplayoffs', {params});
  }
}
