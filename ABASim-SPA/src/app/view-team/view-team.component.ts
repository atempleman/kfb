import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { CompletePlayer } from '../_models/completePlayer';
import { ExtendedPlayer } from '../_models/extendedPlayer';
import { GetPlayerLeague } from '../_models/getPlayerLeague';
import { GetRosterQuickView } from '../_models/getRosterQuickView';
import { League } from '../_models/league';
import { PlayerContractDetailed } from '../_models/playerContractDetailed';
import { PlayerInjury } from '../_models/playerInjury';
import { Standing } from '../_models/standing';
import { Team } from '../_models/team';
import { TeamSalaryCapInfo } from '../_models/teamSalaryCapInfo';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { LeagueService } from '../_services/league.service';
import { PlayerService } from '../_services/player.service';
import { TeamService } from '../_services/team.service';
import { TransferService } from '../_services/transfer.service';

@Component({
  selector: 'app-view-team',
  templateUrl: './view-team.component.html',
  styleUrls: ['./view-team.component.css']
})
export class ViewTeamComponent implements OnInit {
  team: Team;
  teamId: number;
  teamsInjuries: PlayerInjury[] = [];
  playingRoster: CompletePlayer[] = [];
  playerCount = 0;
  statusGrades = 0;
  statusStats = 1;
  statusContracts = 0;
  teamCap: TeamSalaryCapInfo;
  remainingCapSpace = 0;
  teamContracts: PlayerContractDetailed[] = [];

  teamRecord: Standing;
  league: League;

  constructor(private alertify: AlertifyService, private transferService: TransferService, private teamService: TeamService,
              private router: Router, private playerService: PlayerService, private spinner: NgxSpinnerService,
              private leagueService: LeagueService, private authService: AuthService) { }

  ngOnInit() {
    this.teamId = this.transferService.getData();

    this.leagueService.getLeagueForUserId(this.authService.decodedToken.nameid).subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting League Details');
    }, () => {
      this.setupTeam();
    });
  }

  setupTeam() {
    const teamleague: GetRosterQuickView = {
      teamId: this.teamId,
      leagueId: this.league.id
    };

    this.teamService.getTeamForTeamId(teamleague).subscribe(result => {
      this.spinner.show();
      this.team = result;
    }, error => {
      this.alertify.error('Error getting team');
    }, () => {
      this.getTeamStandings();
      this.getPlayerInjuries();
      this.getRosterForTeam();
      this.getSalaryCapDetails();
      this.getTeamContracts();
    });
  }

  getTeamStandings() {
    const teamleague: GetRosterQuickView = {
      teamId: this.teamId,
      leagueId: this.league.id
    };

    this.teamService.getTeamRecord(teamleague).subscribe(result => {
      this.teamRecord = result;
    }, error => {
      this.alertify.error('Error getting team record');
    });
  }

  getTeamContracts() {
    const teamleague: GetRosterQuickView = {
      teamId: this.teamId,
      leagueId: this.league.id
    };

    this.teamService.getTeamContracts(teamleague).subscribe(result => {
      this.teamContracts = result;
    }, error => {
      this.alertify.error('Error getting team contracts');
    });
  }

  getSalaryCapDetails() {
    const teamleague: GetRosterQuickView = {
      teamId: this.teamId,
      leagueId: this.league.id
    };

    this.teamService.getTeamSalaryCapDetails(teamleague).subscribe(result => {
      this.teamCap = result;
    }, error => {
      this.alertify.error('Error getting salary cap details');
    }, () => {
      this.remainingCapSpace = this.teamCap.salaryCapAmount - this.teamCap.currentSalaryAmount;
    });
  }

  getPlayerInjuries() {
    const teamleague: GetRosterQuickView = {
      teamId: this.teamId,
      leagueId: this.league.id
    };

    this.teamService.getPlayerInjuriesForTeam(teamleague).subscribe(result => {
      this.teamsInjuries = result;
    }, error => {
      this.alertify.error('Error getting teams injuries');
    });
  }

  checkIfInjured(playerId: number) {
    const injured = this.teamsInjuries.find(x => x.playerId === playerId);

    if (injured) {
      return 1;
    } else {
      return 0;
    }
  }

  getRosterForTeam() {
    const teamleague: GetRosterQuickView = {
      teamId: this.teamId,
      leagueId: this.league.id
    };

    this.teamService.getExtendedRosterForTeam(teamleague).subscribe(result => {
      this.playingRoster = result;
      this.playerCount = this.playingRoster.length;
    }, error => {
      this.alertify.error('Error getting your roster');
    }, () => {
      this.spinner.hide();
    });
  }

  viewPlayer(player: CompletePlayer) {
    this.transferService.setData(player.playerId);
    this.router.navigate(['/view-player']);
  }

  viewPlayerFromContract(name: string) {
    // Need to get player id for name
    let playerId = 0;

    const playerleague: GetPlayerLeague = {
      playerName: name,
      leagueId: this.league.id
    };

    this.playerService.getPlayerForName(playerleague).subscribe(result => {
      playerId = result.playerId;
    }, error => {
      this.alertify.error('Error getting player name');
    }, () => {
      this.transferService.setData(playerId);
      this.router.navigate(['/view-player']);
    });
  }

  gradesClick() {
    this.statusStats = 0;
    this.statusContracts = 0;
    this.statusGrades = 1;
  }

  statisticsClick() {
    this.statusGrades = 0;
    this.statusContracts = 0;
    this.statusStats = 1;
  }

  contractsClick() {
    this.statusGrades = 0;
    this.statusStats = 0;
    this.statusContracts = 1;
  }

  getMinutesAverage(detailedPlayer: CompletePlayer) {
    const value = ((detailedPlayer.minutesStats / detailedPlayer.gamesStats) / 60);
    const display = value.toFixed(1);
    return display;
  }

  getFGAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.fgmStats / detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getFGAAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.fgaStats / detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getFgPercentage(detailedPlayer: CompletePlayer) {
    const value = ((detailedPlayer.fgmStats) / (detailedPlayer.fgaStats));
    const display = value.toFixed(3);
    return display;
  }

  getThreeFGAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.threeFgmStats / detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getThreeFGAAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.threeFgaStats / detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getThreeFgPercentage(detailedPlayer: CompletePlayer) {
    const value = ((detailedPlayer.threeFgmStats) / (detailedPlayer.threeFgaStats));
    const display = value.toFixed(3);
    return display;
  }

  getFTAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.ftmStats / detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getFTAAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.ftaStats / detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getFTPercentage(detailedPlayer: CompletePlayer) {
    const value = ((detailedPlayer.ftmStats) / (detailedPlayer.ftaStats));
    const display = value.toFixed(3);
    return display;
  }

  getOrebAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.orebsStats / detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getDrebverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.drebsStats / detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalRebAverage(detailedPlayer: CompletePlayer) {
    const totalRebs = detailedPlayer.orebsStats + detailedPlayer.drebsStats;
    const value = (totalRebs / detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalAstAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.astStats / detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalStlAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.stlStats / detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalBlkAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.blkStats / detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalTovAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.toStats / detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalFoulsAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.flsStats / detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalPointsAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.ptsStats / detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffMinutesAverage(detailedPlayer: CompletePlayer) {
    const value = ((detailedPlayer.playoffMinutesStats / detailedPlayer.playoffGamesStats) / 60);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffFGAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.playoffFgmStats / detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffFGAAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.playoffFgaStats / detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffFgPercentage(detailedPlayer: CompletePlayer) {
    const value = ((detailedPlayer.playoffFgmStats) / (detailedPlayer.playoffFgaStats));
    const display = value.toFixed(3);
    return display;
  }

  getPlayoffThreeFGAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.playoffThreeFgmStats / detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffThreeFGAAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.playoffThreeFgaStats / detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffThreeFgPercentage(detailedPlayer: CompletePlayer) {
    const value = ((detailedPlayer.playoffThreeFgmStats) / (detailedPlayer.playoffThreeFgaStats));
    const display = value.toFixed(3);
    return display;
  }

  getPlayoffFTAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.playoffFtmStats / detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffFTAAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.playoffFtaStats / detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffFTPercentage(detailedPlayer: CompletePlayer) {
    const value = ((detailedPlayer.playoffFtmStats) / (detailedPlayer.playoffFtaStats));
    const display = value.toFixed(3);
    return display;
  }

  getPlayoffOrebAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.playoffOrebsStats / detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffDrebverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.playoffDrebsStats / detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalRebAverage(detailedPlayer: CompletePlayer) {
    const totalRebs = detailedPlayer.playoffOrebsStats + detailedPlayer.playoffDrebsStats;
    // console.log('total rebs = ' + totalRebs);
    const value = (totalRebs / detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalAstAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.playoffAstStats / detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalStlAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.playoffStlStats / detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalBlkAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.playoffBlkStats / detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalTovAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.playoffToStats / detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalFoulsAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.playoffFlsStats / detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalPointsAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.playoffPtsStats / detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }
}
