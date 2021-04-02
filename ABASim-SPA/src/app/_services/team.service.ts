import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
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

  getRosterForTeam(teamId: GetRosterQuickView): Observable<Player[]> {
    return this.http.get<Player[]>(this.baseUrl + 'getrosterforteam/' + teamId);
  }

  getExtendedRosterForTeam(teamId: GetRosterQuickView): Observable<CompletePlayer[]> {
    return this.http.get<CompletePlayer[]>(this.baseUrl + 'getextendedroster/' + teamId);
  }

  getQuickViewRosterForTeam(quickview: GetRosterQuickView): Observable<QuickViewPlayer[]> {
    return this.http.get<QuickViewPlayer[]>(this.baseUrl + 'getquickviewroster/' + quickview);
  }

  getInjuriesForTeam(quickview: GetRosterQuickView): Observable<LeaguePlayerInjury[]> {
    return this.http.get<LeaguePlayerInjury[]>(this.baseUrl + 'getteaminjuries/' + quickview);
  }

  getTeamForUserId(userId: number) {
    return this.http.get<Team>(this.baseUrl + 'getteamforuserid/' + userId);
  }

  getAllTeams(leagueId: number): Observable<Team[]> {
    return this.http.get<Team[]>(this.baseUrl + 'getallteams/' + leagueId);
  }

  getTeamForTeamId(teamId: GetRosterQuickView) {
    return this.http.get<Team>(this.baseUrl + 'getteamforteamid/' + teamId);
  }

  getDepthChartForTeamId(teamId: number): Observable<DepthChart[]> {
    return this.http.get<DepthChart[]>(this.baseUrl + 'getteamdepthchart/' + teamId);
  }

  saveDepthCharts(depthCharts: DepthChart[]) {
    return this.http.post(this.baseUrl + 'savedepthchart', depthCharts);
  }

  getTeamForTeamName(teamleague: GetTeamLeague): Observable<Team> {
    return this.http.get<Team>(this.baseUrl + 'getteamforteamname/' + teamleague);
  }

  getTeamForTeamMascot(name: GetTeamLeague): Observable<Team> {
    return this.http.get<Team>(this.baseUrl + 'getteamformascot/' + name);
  }

  rosterSpotCheck(teamId: GetRosterQuickView) {
    return this.http.get<boolean>(this.baseUrl + 'rosterSpotCheck/' + teamId);
  }

  getTeamInitialLotteryOrder(leagueId: number): Observable<Team[]> {
    return this.http.get<Team[]>(this.baseUrl + 'getTeamInitialLotteryOrder/' + leagueId);
  }

  waivePlayer(waivedPlayer: WaivedPlayer) {
    return this.http.post(this.baseUrl + 'waiveplayer', waivedPlayer);
  }

  signPlayer(signedPlayer: SignedPlayer) {
    return this.http.post(this.baseUrl + 'signplayer', signedPlayer);
  }

  getCoachingSettings(teamId: GetRosterQuickView) {
    return this.http.get<CoachSetting>(this.baseUrl + 'getcoachsettings/' + teamId);
  }

  saveCoachingSettings(setting: CoachSetting) {
    return this.http.post(this.baseUrl + 'savecoachsetting', setting);
  }

  getAllTeamsExceptUsers(teamId: GetRosterQuickView): Observable<Team[]> {
    return this.http.get<Team[]>(this.baseUrl + 'getallteamsexceptusers/' + teamId);
  }

  getTradeOffers(teamId: GetRosterQuickView): Observable<Trade[]> {
    return this.http.get<Trade[]>(this.baseUrl + 'gettradeoffers/' + teamId);
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

  getTeamDraftPicks(teamId: GetRosterQuickView): Observable<TeamDraftPick[]> {
    return this.http.get<TeamDraftPick[]>(this.baseUrl + 'getteamsdraftpicks/' + teamId);
  }

  getPlayerInjuriesForTeam(teamId: GetRosterQuickView): Observable<PlayerInjury[]> {
    return this.http.get<PlayerInjury[]>(this.baseUrl + 'getinjuriesforteam/' + teamId);
  }

  getInjruiesForFreeAgents(leagueId: number): Observable<PlayerInjury[]> {
    return this.http.get<PlayerInjury[]>(this.baseUrl + 'getinjuriesforfreeagents/' + leagueId);
  }

  getInjuryForPlayer(playeridleague: GetPlayerIdLeague): Observable<PlayerInjury> {
    return this.http.get<PlayerInjury>(this.baseUrl + 'getinjuryforplayer/' + playeridleague);
  }

  getTeamSalaryCapDetails(teamId: GetRosterQuickView): Observable<TeamSalaryCapInfo> {
    return this.http.get<TeamSalaryCapInfo>(this.baseUrl + 'getteamsalarycapdetails/' + teamId);
  }

  getTeamContracts(teamId: GetRosterQuickView): Observable<PlayerContractDetailed[]> {
    return this.http.get<PlayerContractDetailed[]>(this.baseUrl + 'getteamcontracts/' + teamId);
  }

  getOffensiveStrategies(): Observable<OffensiveStrategy[]> {
    return this.http.get<OffensiveStrategy[]>(this.baseUrl + 'getoffensivestrategies');
  }

  getDefensiveStrategies(): Observable<DefensiveStrategy[]> {
    return this.http.get<DefensiveStrategy[]>(this.baseUrl + 'getdefensivestrategies');
  }

  getStrategyForTeam(teamId: GetRosterQuickView): Observable<Strategy> {
    return this.http.get<Strategy>(this.baseUrl + 'getstrategyforteam/' + teamId);
  }

  saveStrategy(strategy: Strategy) {
    return this.http.post(this.baseUrl + 'savestrategy', strategy);
  }

  getContractOffersForTeam(teamId: GetRosterQuickView): Observable<ContractOffer[]> {
    return this.http.get<ContractOffer[]>(this.baseUrl + 'getcontractoffersforteam/' + teamId);
  }

  saveContractOffer(offer: ContractOffer) {
    return this.http.post(this.baseUrl + 'savecontractoffer', offer);
  }

  deleteFreeAgentOffer(contractId: number) {
    return this.http.get<boolean>(this.baseUrl + 'deletefreeagentoffer/' + contractId);
  }

  getWaivedContracts(teamId: GetRosterQuickView): Observable<WaivedContract[]> {
    return this.http.get<WaivedContract[]>(this.baseUrl + 'getwaivedcontracts/' + teamId);
  }

  getTradePlayerView(teamId: GetRosterQuickView): Observable<TradePlayerView[]> {
    return this.http.get<TradePlayerView[]>(this.baseUrl + 'gettradeplayerviews/' + teamId);
  }

  getTeamRecord(teamId: GetRosterQuickView): Observable<Standing> {
    return this.http.get<Standing>(this.baseUrl + 'getteamrecord/' + teamId);
  }
}
