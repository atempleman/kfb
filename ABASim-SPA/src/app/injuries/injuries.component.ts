import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { League } from '../_models/league';
import { LeaguePlayerInjury } from '../_models/leaguePlayerInjury';
import { Player } from '../_models/player';
import { Team } from '../_models/team';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { LeagueService } from '../_services/league.service';
import { TeamService } from '../_services/team.service';
import { TransferService } from '../_services/transfer.service';

@Component({
  selector: 'app-injuries',
  templateUrl: './injuries.component.html',
  styleUrls: ['./injuries.component.css']
})
export class InjuriesComponent implements OnInit {
  leagueInjuries: LeaguePlayerInjury[] = [];
  league: League;
  team: Team;

  constructor(private router: Router, private leagueService: LeagueService, private alertify: AlertifyService,
              private transferService: TransferService, private spinner: NgxSpinnerService,
              private teamService: TeamService, private authService: AuthService) { }

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
    this.leagueService.getLeagueInjuries(this.league.id).subscribe(result => {
      this.leagueInjuries = result;
    }, error => {
      this.alertify.error('Error getting league injuries');
      this.spinner.hide();
    }, () => {
      this.spinner.hide();
    });
  }

  viewPlayer(player: LeaguePlayerInjury) {
    this.transferService.setData(player.playerId);
    this.router.navigate(['/view-player']);
  }
}
