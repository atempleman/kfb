import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { AddDraftRank } from '../_models/addDraftRank';
import { DraftPlayer } from '../_models/draftPlayer';
import { GetRosterQuickView } from '../_models/getRosterQuickView';
import { League } from '../_models/league';
import { Player } from '../_models/player';
import { Team } from '../_models/team';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { DraftService } from '../_services/draft.service';
import { LeagueService } from '../_services/league.service';
import { PlayerService } from '../_services/player.service';
import { TeamService } from '../_services/team.service';
import { TransferService } from '../_services/transfer.service';

@Component({
  selector: 'app-draft-player-pool-season',
  templateUrl: './draft-player-pool-season.component.html',
  styleUrls: ['./draft-player-pool-season.component.css']
})
export class DraftPlayerPoolSeasonComponent implements OnInit {
  team: Team;
  league: League;
  recordTotal = 0;
  pages = 1;
  pager = 1;
  draftPlayers: Player[] = [];
  darftPlayersPool: DraftPlayer[] = [];
  draftboardPlayers: DraftPlayer[] = [];

  constructor(private spinner: NgxSpinnerService, private alertify: AlertifyService, private teamService: TeamService,
    private authService: AuthService, private leagueService: LeagueService, private playerService: PlayerService,
    private transferService: TransferService, private router: Router, private draftService: DraftService) { }

  ngOnInit() {
    this.spinner.show();

    this.teamService.getTeamForUserId(this.authService.decodedToken.nameid).subscribe(result => {
      this.team = result;
      // Need to persist the team to cookie
      localStorage.setItem('teamId', this.team.teamId.toString());
    }, error => {
      this.alertify.error('Error getting your Team');
    }, () => {
      this.setupLeague();
    });
  }

  setupLeague() {
    this.leagueService.getLeagueForUserId(this.authService.decodedToken.nameid).subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting League Details');
    }, () => {
      this.setupPage();
    });
  }

  setupPage() {
    this.getDraftboardPlayers();
    this.getDraftPlayers();
    this.getCountOfAvailablePlayers();
  }

  getCountOfAvailablePlayers() {
    this.playerService.getCountOfAvailableDraftPlayersSeason(this.league.id).subscribe(result => {
      this.recordTotal = result;
    }, error => {
      this.alertify.error('Error getting count of available players');
    }, () => {
      this.pages = +(this.recordTotal / 50).toFixed(0) + 1;
    });
  }

  getDraftPlayers() {
    if (this.league.stateId == 13 || this.league.stateId == 14) {
      console.log('ash');
      // This is where we get the full player data
      this.playerService.getAllDraftPoolPlayersSeason(this.league.id).subscribe(result => {
        this.darftPlayersPool = result;
        console.log(this.darftPlayersPool);
      }, error => {
        this.alertify.error('Error getting player pool available for the draft');
        this.spinner.hide();
      }, () => {
        this.spinner.hide();
      });
    } else {
      // Get all draft players
      this.playerService.getAllUpcomingPlayers(this.league.id).subscribe(result => {
        this.draftPlayers = result;
        // this.masterList = result;
      }, error => {
        this.alertify.error('Error getting players available for the draft');
        this.spinner.hide();
      }, () => {
        this.spinner.hide();
      });
    }
  }

  viewPlayer(player: number) {
    this.transferService.setData(player);
    this.router.navigate(['/view-player']);
  }

  checkPlayer(playerId: number) {
    const db = this.draftboardPlayers.find(x => x.playerId === playerId);
    if (db) {
      return 1;
    } else {
      return 0;
    }
  }

  getDraftboardPlayers() {
    // Need to get the draftboard players
    const summary: GetRosterQuickView = {
      teamId: this.team.teamId,
      leagueId: this.league.id
    };
    this.draftService.getDraftBoardForTeam(summary).subscribe(result => {
      this.draftboardPlayers = result;
    }, error => {
      this.alertify.error('Error getting draftboard');
    }, () => {
      this.getDraftPlayers();
    });
  }

  addPlayerToDraftRank(selectedPlayer: DraftPlayer) {
    const newRanking = {} as AddDraftRank;
    newRanking.playerId = selectedPlayer.playerId;
    newRanking.teamId = this.team.teamId;
    newRanking.leagueId = this.league.id;

    this.draftService.addDraftPlayerRanking(newRanking).subscribe(result => {
    }, error => {
      this.alertify.error('Error adding Player to Draft Board');
    }, () => {
      this.alertify.success('Player added to Draft Board.');
      const record = this.draftPlayers.find(x => x.playerId === selectedPlayer.playerId);

      // Need to convert this to a draft player record
      let dp: DraftPlayer = {
        playerId: record.playerId,
        firstName: record.firstName,
        surname: record.surname,
        pgPosition: record.pgPosition,
        sgPosition: record.sgPosition,
        sfPosition: record.sfPosition,
        pfPosition: record.pfPosition,
        cPosition: record.cPosition,
        age: record.age,
        twoGrade: '',
        threeGrade: '',
        ftGrade: '',
        oRebGrade: '',
        dRebGrade: '',
        handlingGrade: '',
        stealGrade: '',
        blockGrade: '',
        staminaGrade: '',
        passingGrade: '',
        intangiblesGrade: ''
      };

      this.draftboardPlayers.push(dp);
    });
  }

  removePlayerDraftRank(selectedPlayer: DraftPlayer) {
    const newRanking = {} as AddDraftRank;
    newRanking.playerId = selectedPlayer.playerId;
    newRanking.teamId = this.team.teamId;
    newRanking.leagueId = this.league.id;

    this.draftService.removeDraftPlayerRanking(newRanking).subscribe(result => {

    }, error => {
      this.alertify.error('Error removing Player from Draft board');
    }, () => {
      const index = this.draftboardPlayers.findIndex(x => x.playerId === selectedPlayer.playerId);
      this.draftboardPlayers.splice(index, 1);
    });
  }
}
