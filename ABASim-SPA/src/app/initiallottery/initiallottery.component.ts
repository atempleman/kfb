import { Component, OnInit } from '@angular/core';
import { LeagueService } from '../_services/league.service';
import { AlertifyService } from '../_services/alertify.service';
import { League } from '../_models/league';
import { TeamService } from '../_services/team.service';
import { Team } from '../_models/team';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-initiallottery',
  templateUrl: './initiallottery.component.html',
  styleUrls: ['./initiallottery.component.css']
})
export class InitiallotteryComponent implements OnInit {
  league: League;
  teams: Team[] = [];
  team: Team;

  constructor(private leagueService: LeagueService, private alertify: AlertifyService, private teamService: TeamService,
              private authService: AuthService) { }

  ngOnInit() {
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
      this.getTeams();
    });
  }

  getTeams() {
    if (this.league.stateId < 3) {
      this.teamService.getAllTeams(this.league.id).subscribe(result => {
        this.teams = result;
      }, error => {
        this.alertify.error('Error getting all teams');
      });
    } else if (this.league.stateId > 2) {
      this.teamService.getTeamInitialLotteryOrder(this.league.id).subscribe(result => {
        this.teams = result;
      }, error => {
        this.alertify.error('Error getting lottery order');
      });
    }
  }

}
