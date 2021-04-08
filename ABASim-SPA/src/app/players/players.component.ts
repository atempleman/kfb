import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { PlayerService } from '../_services/player.service';
import { Player } from '../_models/player';
import { TransferService } from '../_services/transfer.service';
import { AuthService } from '../_services/auth.service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NgxSpinnerService } from 'ngx-spinner';
import { League } from '../_models/league';
import { LeagueService } from '../_services/league.service';
import { TeamService } from '../_services/team.service';
import { Team } from '../_models/team';
import { GetPlayerIdLeague } from '../_models/getPlayerIdLeague';

@Component({
  selector: 'app-players',
  templateUrl: './players.component.html',
  styleUrls: ['./players.component.css']
})
export class PlayersComponent implements OnInit {
  allPlayers: Player[] = [];
  searchForm: FormGroup;
  positionFilter = 0;

  league: League;
  team: Team;

  constructor(private router: Router, private alertify: AlertifyService, private authService: AuthService,
              private transferService: TransferService, private playerService: PlayerService,
              private fb: FormBuilder, private spinner: NgxSpinnerService, private leagueService: LeagueService,
              private teamService: TeamService) { }

  ngOnInit() {
    this.spinner.show();

    this.teamService.getTeamForUserId(this.authService.decodedToken.nameid).subscribe(result => {
      this.team = result;
      // Need to persist the team to cookie
      localStorage.setItem('teamId', this.team.id.toString());
    }, error => {
      this.alertify.error('Error getting your Team');
    }, () => {
      this.setupLeague();
    });

    this.searchForm = this.fb.group({
      filter: ['']
    });
  }

  setupLeague() {
    this.leagueService.getLeagueForUserId(this.authService.decodedToken.nameid).subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting League Details');
    }, () => {
      this.getPlayers();
    });
  }

  getPlayers() {
    this.playerService.getAllPlayers(this.league.id).subscribe(result => {
      this.allPlayers = result;
    }, error => {
      this.alertify.error('Error getting players');
    }, () => {
      this.spinner.hide();
    });
  }

  viewPlayer(player: Player) {
    this.transferService.setData(player.id);
    this.router.navigate(['/view-player']);
  }

  filterTable() {
    this.spinner.show();
    // this.displayPaging = 1;
    const filter = this.searchForm.value.filter;
    console.log(filter);
    // Need to call service
    this.playerService.filterPlayers(filter).subscribe(result => {
      this.allPlayers = result;
    }, error => {
      this.alertify.error('Error getting filtered players');
      console.log(error);
      this.spinner.hide();
    }, () => {
      this.spinner.hide();
    });
  }

  resetFilter() {
    this.spinner.show();
    // this.displayPaging = 0;
    this.getPlayers();

    this.searchForm = this.fb.group({
      filter: ['']
    });
  }

  filterByPos(pos: number) {
    this.spinner.show();
    this.positionFilter = pos;

    if (pos === 0) {
    //   this.displayPaging = 0;
      this.getPlayers();
    } else {
    //   this.displayPaging = 1;
    // Now we need to update the listing appropriately

    const summary: GetPlayerIdLeague = {
      playerId: this.positionFilter,
      leagueId: this.league.id
    };

    this.playerService.getPlayerByPos(summary).subscribe(result => {
        this.allPlayers = result;
      }, error => {
        this.alertify.error('Error getting filtered players');
        this.spinner.hide();
      }, () => {
        this.spinner.hide();
      });
    }
  }
}
