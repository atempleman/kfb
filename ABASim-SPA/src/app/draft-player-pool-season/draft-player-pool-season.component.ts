import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { DraftPlayer } from '../_models/draftPlayer';
import { League } from '../_models/league';
import { Player } from '../_models/player';
import { Team } from '../_models/team';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
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
  
  constructor(private spinner: NgxSpinnerService, private alertify: AlertifyService, private teamService: TeamService,
              private authService: AuthService, private leagueService: LeagueService, private playerService: PlayerService,
              private transferService: TransferService, private router: Router) { }

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
}
