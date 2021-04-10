import { Component, OnInit } from '@angular/core';
import { GetRosterQuickView } from '../_models/getRosterQuickView';
import { League } from '../_models/league';
import { Team } from '../_models/team';
import { TeamSalaryCapInfo } from '../_models/teamSalaryCapInfo';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { LeagueService } from '../_services/league.service';
import { TeamService } from '../_services/team.service';

@Component({
  selector: 'app-finances',
  templateUrl: './finances.component.html',
  styleUrls: ['./finances.component.css']
})
export class FinancesComponent implements OnInit {
  team: Team;
  teamCap: TeamSalaryCapInfo;
  remainingCapSpace = 0;
  league: League;

  constructor(private teamService: TeamService, private alertify: AlertifyService, private authService: AuthService,
              private leagueService: LeagueService) { }

  ngOnInit() {
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
      this.getSalaryCapDetails();
    });
  }

  getSalaryCapDetails() {
    const summary: GetRosterQuickView = {
      teamId: this.team.teamId,
      leagueId: this.league.id
    };

    this.teamService.getTeamSalaryCapDetails(summary).subscribe(result => {
      this.teamCap = result;
    }, error => {
      this.alertify.error('Error getting salary cap details');
    }, () => {
      this.remainingCapSpace = this.teamCap.salaryCapAmount - this.teamCap.currentSalaryAmount;
    });
  }

}
