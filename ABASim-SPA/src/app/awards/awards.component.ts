import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { League } from '../_models/league';
import { Team } from '../_models/team';
import { Votes } from '../_models/votes';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { LeagueService } from '../_services/league.service';
import { TeamService } from '../_services/team.service';
import { TransferService } from '../_services/transfer.service';

@Component({
  selector: 'app-awards',
  templateUrl: './awards.component.html',
  styleUrls: ['./awards.component.css']
})
export class AwardsComponent implements OnInit {
  league: League;
  mvpList: Votes[] = [];
  dpoyList: Votes[] = [];
  sixthList: Votes[] = [];
  firstTeam: Votes[] = [];
  secondTeam: Votes[] = [];
  thirdTeam: Votes[] = [];
  allnbateams: Votes[] = [];

  team: Team;

  constructor(private router: Router, private leagueService: LeagueService, private alertify: AlertifyService,
              private transferService: TransferService, private spinner: NgxSpinnerService, private teamService: TeamService,
              private authService: AuthService) { }

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
    this.leagueService.getMvpTopFive(this.league.id).subscribe(result => {
      this.mvpList = result;
    }, error => {
      this.alertify.error('Error getting MVP leaders');
    });

    this.leagueService.getDpoyTopFive(this.league.id).subscribe(result => {
      this.dpoyList = result;
    }, error => {
      this.alertify.error('Error getting DPOY leaders');
    });

    this.leagueService.getSixthManTopFive(this.league.id).subscribe(result => {
      this.sixthList = result;
    }, error => {
      this.alertify.error('Error getting 6th man leaders');
    });


    this.leagueService.getAllNBATeams(this.league.id).subscribe(result => {
      this.allnbateams = result;
    }, error => {
      this.alertify.error('Error getting All-ABA Teams');
    }, () => {
      this.firstTeam = this.allnbateams.splice(0, 5);
      this.secondTeam = this.allnbateams.splice(0, 5);
      this.thirdTeam = this.allnbateams.splice(0, 5);

      this.spinner.hide();
    });
  }

  viewPlayer(player: number) {
    this.transferService.setData(player);
    this.router.navigate(['/view-player']);
  }

}
