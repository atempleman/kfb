import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Player } from '../_models/player';
import { Observable } from 'rxjs';
import { Team } from '../_models/team';
import { DepthChart } from '../_models/depthChart';
import { environment } from 'src/environments/environment';
import { ExtendedPlayer } from '../_models/extendedPlayer';
import { WaivedPlayer } from '../_models/waivedPlayer';
import { CoachSetting } from '../_models/coachSetting';
import { SignedPlayer } from '../_models/signedPlayer';
import { Trade } from '../_models/trade';
import { TradeMessage } from '../_models/tradeMessage';
import { TeamDraftPick } from '../_models/teamDraftPick';
import { PlayerInjury } from '../_models/playerInjury';
import { CompletePlayer } from '../_models/completePlayer';
import { TeamSalaryCapInfo } from '../_models/teamSalaryCapInfo';
import { PlayerContractDetailed } from '../_models/playerContractDetailed';
import { OffensiveStrategy } from '../_models/offensiveStrategy';
import { DefensiveStrategy } from '../_models/defensiveStrategyId';
import { Strategy } from '../_models/strategy';
import { ContractOffer } from '../_models/contractOffer';
import { WaivedContract } from '../_models/waivedContract';
import { TradePlayerView } from '../_models/tradePlayerView';
import { Standing } from '../_models/standing';
import { QuickViewPlayer } from '../_models/QuickViewPlayer';
import { LeaguePlayerInjury } from '../_models/leaguePlayerInjury';
import { LeagueService } from './league.service';
import { GetRosterQuickView } from '../_models/getRosterQuickView';
import { GetTeamLeague } from '../_models/getTeamLeague';
import { GetPlayerIdLeague } from '../_models/getPlayerIdLeague';

@Injectable({
  providedIn: 'root'
})
export class TeamService {
  baseUrl = environment.apiUrl + '/team/';

  constructor(private http: HttpClient) { }

  checkAvailableTeams() {
    return this.http.get<boolean>(this.baseUrl + 'checkavailableteams');
  }

  getAvailableTeams(): Observable<Team[]> {
    return this.http.get<Team[]>(this.baseUrl + 'getavailableteams');
  }

  getAvailableTeamsForPrivate(leaguecode: string): Observable<Team[]> {
    return this.http.get<Team[]>(this.baseUrl + 'getavailableteamsprivate/' + leaguecode);
  }

  getRosterForTeam(quickview: GetRosterQuickView): Observable<Player[]> {
    const params = new HttpParams()
      .set('teamId', quickview.teamId.toString())
      .set('leagueId', quickview.leagueId.toString());

    return this.http.get<Player[]>(this.baseUrl + 'getrosterforteam', {params});
  }

  getExtendedRosterForTeam(quickview: GetRosterQuickView): Observable<CompletePlayer[]> {
    const params = new HttpParams()
      .set('teamId', quickview.teamId.toString())
      .set('leagueId', quickview.leagueId.toString());

    return this.http.get<CompletePlayer[]>(this.baseUrl + 'getextendedroster', {params});
  }

  getQuickViewRosterForTeam(quickview: GetRosterQuickView): Observable<QuickViewPlayer[]> {
    const params = new HttpParams()
      .set('teamId', quickview.teamId.toString())
      .set('leagueId', quickview.leagueId.toString());

    return this.http.get<QuickViewPlayer[]>(this.baseUrl + 'getquickviewroster', { params });
  }

  getInjuriesForTeam(quickview: GetRosterQuickView): Observable<LeaguePlayerInjury[]> {
    const params = new HttpParams()
      .set('teamId', quickview.teamId.toString())
      .set('leagueId', quickview.leagueId.toString());

    return this.http.get<LeaguePlayerInjury[]>(this.baseUrl + 'getteaminjuries', { params });
  }

  getTeamForUserId(userId: number) {
    return this.http.get<Team>(this.baseUrl + 'getteamforuserid/' + userId);
  }

  getAllTeams(leagueId: number): Observable<Team[]> {
    return this.http.get<Team[]>(this.baseUrl + 'getallteams/' + leagueId);
  }

  getTeamForTeamId(qv: GetRosterQuickView) {
    const params = new HttpParams()
      .set('teamId', qv.teamId.toString())
      .set('leagueId', qv.leagueId.toString());

    return this.http.get<Team>(this.baseUrl + 'getteamforteamid', {params});
  }

  getDepthChartForTeamId(qv: GetRosterQuickView): Observable<DepthChart[]> {
    const params = new HttpParams()
      .set('teamId', qv.teamId.toString())
      .set('leagueId', qv.leagueId.toString());
    return this.http.get<DepthChart[]>(this.baseUrl + 'getteamdepthchart', {params});
  }

  saveDepthCharts(depthCharts: DepthChart[]) {
    return this.http.post(this.baseUrl + 'savedepthchart', depthCharts);
  }

  getTeamForTeamName(tl: GetTeamLeague): Observable<Team> {
    const params = new HttpParams()
      .set('teamname', tl.teamname.toString())
      .set('leagueId', tl.leagueId.toString());

    return this.http.get<Team>(this.baseUrl + 'getteamforteamname', {params});
  }

  getTeamForTeamMascot(tl: GetTeamLeague): Observable<Team> {
    const params = new HttpParams()
      .set('teamname', tl.teamname.toString())
      .set('leagueId', tl.leagueId.toString());
    return this.http.get<Team>(this.baseUrl + 'getteamformascot', {params});
  }

  rosterSpotCheck(qv: GetRosterQuickView) {
    const params = new HttpParams()
      .set('teamId', qv.teamId.toString())
      .set('leagueId', qv.leagueId.toString());

    return this.http.get<boolean>(this.baseUrl + 'rosterSpotCheck', {params});
  }

  getTeamInitialLotteryOrder(leagueId: number): Observable<Team[]> {
    return this.http.get<Team[]>(this.baseUrl + 'getTeamInitialLotteryOrder/' + leagueId);
  }

  getTeamSeasonLotteryOrder(leagueId: number): Observable<Team[]> {
    return this.http.get<Team[]>(this.baseUrl + 'getTeamSeasonLotteryOrder/' + leagueId);
  }

  waivePlayer(waivedPlayer: WaivedPlayer) {
    return this.http.post(this.baseUrl + 'waiveplayer', waivedPlayer);
  }

  signPlayer(signedPlayer: SignedPlayer) {
    return this.http.post(this.baseUrl + 'signplayer', signedPlayer);
  }

  getCoachingSettings(qv: GetRosterQuickView) {
    const params = new HttpParams()
      .set('teamId', qv.teamId.toString())
      .set('leagueId', qv.leagueId.toString());
    return this.http.get<CoachSetting>(this.baseUrl + 'getcoachsettings', {params});
  }

  saveCoachingSettings(setting: CoachSetting) {
    return this.http.post(this.baseUrl + 'savecoachsetting', setting);
  }

  getAllTeamsExceptUsers(qv: GetRosterQuickView): Observable<Team[]> {
    const params = new HttpParams()
      .set('teamId', qv.teamId.toString())
      .set('leagueId', qv.leagueId.toString());

    return this.http.get<Team[]>(this.baseUrl + 'getallteamsexceptusers', {params});
  }

  getTradeOffers(qv: GetRosterQuickView): Observable<Trade[]> {
    const params = new HttpParams()
      .set('teamId', qv.teamId.toString())
      .set('leagueId', qv.leagueId.toString());

    return this.http.get<Trade[]>(this.baseUrl + 'gettradeoffers', {params});
  }

  saveTradeProposal(trade: Trade[]) {
    return this.http.post(this.baseUrl + 'savetradeproposal', trade);
  }

  acceptTradeProposal(tradeId: number) {
    return this.http.get<boolean>(this.baseUrl + 'acceptradeproposal/' + tradeId);
  }

  pullTradeProposal(tradeId: number) {
    return this.http.get<boolean>(this.baseUrl + 'pullradeproposal/' + tradeId);
  }

  rejectTradeProposal(trade: TradeMessage) {
    return this.http.post(this.baseUrl + 'rejecttradeproposal', trade);
  }

  getTradeMessageForTradeId(tradeId: number) {
    return this.http.get<TradeMessage>(this.baseUrl + 'gettrademessage/' + tradeId);
  }

  getTeamDraftPicks(qv: GetRosterQuickView): Observable<TeamDraftPick[]> {
    const params = new HttpParams()
      .set('teamId', qv.teamId.toString())
      .set('leagueId', qv.leagueId.toString());

    return this.http.get<TeamDraftPick[]>(this.baseUrl + 'getteamsdraftpicks', {params});
  }

  getPlayerInjuriesForTeam(qv: GetRosterQuickView): Observable<PlayerInjury[]> {
    const params = new HttpParams()
      .set('teamId', qv.teamId.toString())
      .set('leagueId', qv.leagueId.toString());

    return this.http.get<PlayerInjury[]>(this.baseUrl + 'getinjuriesforteam', {params});
  }

  getInjruiesForFreeAgents(leagueId: number): Observable<PlayerInjury[]> {
    return this.http.get<PlayerInjury[]>(this.baseUrl + 'getinjuriesforfreeagents/' + leagueId);
  }

  getInjuryForPlayer(pil: GetPlayerIdLeague): Observable<PlayerInjury> {
    const params = new HttpParams()
      .set('playerId', pil.playerId.toString())
      .set('leagueId', pil.leagueId.toString());

    return this.http.get<PlayerInjury>(this.baseUrl + 'getinjuryforplayer', {params});
  }

  getTeamSalaryCapDetails(qv: GetRosterQuickView): Observable<TeamSalaryCapInfo> {
    const params = new HttpParams()
      .set('teamId', qv.teamId.toString())
      .set('leagueId', qv.leagueId.toString());

    return this.http.get<TeamSalaryCapInfo>(this.baseUrl + 'getteamsalarycapdetails', {params});
  }

  getTeamContracts(qv: GetRosterQuickView): Observable<PlayerContractDetailed[]> {
    const params = new HttpParams()
      .set('teamId', qv.teamId.toString())
      .set('leagueId', qv.leagueId.toString());

    return this.http.get<PlayerContractDetailed[]>(this.baseUrl + 'getteamcontracts', {params});
  }

  getOffensiveStrategies(): Observable<OffensiveStrategy[]> {
    return this.http.get<OffensiveStrategy[]>(this.baseUrl + 'getoffensivestrategies');
  }

  getDefensiveStrategies(): Observable<DefensiveStrategy[]> {
    return this.http.get<DefensiveStrategy[]>(this.baseUrl + 'getdefensivestrategies');
  }

  getStrategyForTeam(qv: GetRosterQuickView): Observable<Strategy> {
    const params = new HttpParams()
      .set('teamId', qv.teamId.toString())
      .set('leagueId', qv.leagueId.toString());

    return this.http.get<Strategy>(this.baseUrl + 'getstrategyforteam', {params});
  }

  saveStrategy(strategy: Strategy) {
    return this.http.post(this.baseUrl + 'savestrategy', strategy);
  }

  getContractOffersForTeam(qv: GetRosterQuickView): Observable<ContractOffer[]> {
    const params = new HttpParams()
      .set('teamId', qv.teamId.toString())
      .set('leagueId', qv.leagueId.toString());

    return this.http.get<ContractOffer[]>(this.baseUrl + 'getcontractoffersforteam', {params});
  }

  saveContractOffer(offer: ContractOffer) {
    return this.http.post(this.baseUrl + 'savecontractoffer', offer);
  }

  deleteFreeAgentOffer(contractId: number) {
    return this.http.get<boolean>(this.baseUrl + 'deletefreeagentoffer/' + contractId);
  }

  getWaivedContracts(qv: GetRosterQuickView): Observable<WaivedContract[]> {
    const params = new HttpParams()
      .set('teamId', qv.teamId.toString())
      .set('leagueId', qv.leagueId.toString());

    return this.http.get<WaivedContract[]>(this.baseUrl + 'getwaivedcontracts', {params});
  }

  getTradePlayerView(qv: GetRosterQuickView): Observable<TradePlayerView[]> {
    const params = new HttpParams()
      .set('teamId', qv.teamId.toString())
      .set('leagueId', qv.leagueId.toString());

    return this.http.get<TradePlayerView[]>(this.baseUrl + 'gettradeplayerviews', {params});
  }

  getTeamRecord(qv: GetRosterQuickView): Observable<Standing> {
    const params = new HttpParams()
      .set('teamId', qv.teamId.toString())
      .set('leagueId', qv.leagueId.toString());

    return this.http.get<Standing>(this.baseUrl + 'getteamrecord/', {params});
  }
}
