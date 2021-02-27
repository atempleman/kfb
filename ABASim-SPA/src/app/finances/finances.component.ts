import { Component, OnInit } from '@angular/core';
import { Team } from '../_models/team';
import { TeamSalaryCapInfo } from '../_models/teamSalaryCapInfo';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
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

  constructor(private teamService: TeamService, private alertify: AlertifyService, private authService: AuthService) { }

  ngOnInit() {
    this.teamService.getTeamForUserId(this.authService.decodedToken.nameid).subscribe(result => {
      this.team = result;
    }, error => {
      this.alertify.error('Error getting your Team');
    }, () => {
      this.getSalaryCapDetails();
    });
  }

  getSalaryCapDetails() {
    this.teamService.getTeamSalaryCapDetails(this.team.id).subscribe(result => {
      this.teamCap = result;
    }, error => {
      this.alertify.error('Error getting salary cap details');
    }, () => {
      this.remainingCapSpace = this.teamCap.salaryCapAmount - this.teamCap.currentSalaryAmount;
    });
  }

}
