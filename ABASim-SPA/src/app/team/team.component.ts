import { Component, OnInit, TemplateRef, ElementRef } from '@angular/core';
import { Router } from '@angular/router';
import { LeagueService } from '../_services/league.service';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { TeamService } from '../_services/team.service';
import { League } from '../_models/league';
import { Team } from '../_models/team';
import { ExtendedPlayer } from '../_models/extendedPlayer';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { WaivedPlayer } from '../_models/waivedPlayer';
import { TransferService } from '../_services/transfer.service';
import { PlayerInjury } from '../_models/playerInjury';
import { CompletePlayer } from '../_models/completePlayer';
import { TeamSalaryCapInfo } from '../_models/teamSalaryCapInfo';
import { PlayerContractDetailed } from '../_models/playerContractDetailed';
import { WaivedContract } from '../_models/waivedContract';
import { Standing } from '../_models/standing';

@Component({
  selector: 'app-team',
  templateUrl: './team.component.html',
  styleUrls: ['./team.component.css']
})
export class TeamComponent implements OnInit {
  league: League;
  team: Team;

  rosterSelected = 1;
  coachingSelected = 0;
  finacnesSelected = 0;
  tradesSelected = 0;
  freeagentsSelected = 0;

  teamRecord: Standing;

  isAdmin = 0;
  message: number;

  teamCap: TeamSalaryCapInfo;
  remainingCapSpace = 0;

  primaryColor: string = '22, 24, 100';
  secondaryColor: string = '12,126,120';

  constructor(private router: Router, private leagueService: LeagueService, private alertify: AlertifyService,
              private authService: AuthService, private teamService: TeamService, private modalService: BsModalService,
              private transferService: TransferService) { }

  ngOnInit() {
    // Check to see if the user is an admin user
    this.isAdmin = this.authService.isAdmin();

    // get the league object - TODO - roll the league state into the object as a Dto and pass back
    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting League Details');
    }, () => {
    });

    this.teamService.getTeamForUserId(this.authService.decodedToken.nameid).subscribe(result => {
      this.team = result;
    }, error => {
      this.alertify.error('Error getting your Team');
    }, () => {
      this.getTeamStandings();
      
      // this.backgroundStyle();
      this.getSalaryCapDetails();
    });
  }

  getTeamStandings() {
    this.teamService.getTeamRecord(this.team.id).subscribe(result => {
      this.teamRecord = result;
    }, error => {
      this.alertify.error('Error getting team record');
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

  rosterSelection() {
    this.finacnesSelected = 0;
    this.coachingSelected = 0;
    this.tradesSelected = 0;
    this.freeagentsSelected = 0;
    this.rosterSelected = 1;
  }

  coachingSelection() {
    this.rosterSelected = 0;
    this.finacnesSelected = 0;
    this.tradesSelected = 0;
    this.freeagentsSelected = 0;
    this.coachingSelected = 1;
  }

  finacnesSelection() {
    this.rosterSelected = 0;
    this.tradesSelected = 0;
    this.freeagentsSelected = 0;
    this.coachingSelected = 0;
    this.finacnesSelected = 1;
  }

  freeagentsSelection() {
    this.rosterSelected = 0;
    this.tradesSelected = 0;
    this.coachingSelected = 0;
    this.finacnesSelected = 0;
    this.freeagentsSelected = 1;
  }

  tradesSelection() {
    this.rosterSelected = 0;
    this.coachingSelected = 0;
    this.finacnesSelected = 0;
    this.freeagentsSelected = 0;
    this.tradesSelected = 1;
  }

  goToCoaching() {
    this.router.navigate(['/coaching']);
  }

  goToDepthCharts() {
    this.router.navigate(['/depthchart']);
  }

  goToFreeAgents() {
    this.router.navigate(['/freeagents']);
  }

  goToTrades() {
    this.router.navigate(['/trades']);
  }

  backgroundStyle() {
    switch (this.team.id) {
      case 2:
        // Toronto
        this.primaryColor = '206,17,65';
        this.secondaryColor = '6,25,34';
        break;
      case 3:
        // Milwaukee
        this.primaryColor = '0,71,27';
        this.secondaryColor = '240,235,210';
        break;
      case 4:
        // Miami
        this.primaryColor = '152,0,46';
        this.secondaryColor = '249,160,27';
        break;
      case 5:
        // Denver
        this.primaryColor = '13,34,64';
        this.secondaryColor = '255,198,39';
        break;
      case 6:
        // Lakers
        this.primaryColor = '85,37,130';
        this.secondaryColor = '253,185,39';
        break;
      case 7:
        // Rockets
        this.primaryColor = '206,17,65';
        this.secondaryColor = '6,25,34';
        break;
      case 8:
        // Boston
        this.primaryColor = '0, 122, 51';
        this.secondaryColor = '139,111,78';
        break;
      case 9:
        // Indiana
        this.primaryColor = '0,45,98';
        this.secondaryColor = '253,187,48';
        break;
      case 10:
        // Orlando
        this.primaryColor = '0,125,197';
        this.secondaryColor = '196,206,211';
        break;
      case 11:
        // OKC
        this.primaryColor = '0,125,195';
        this.secondaryColor = '239,59,36';
        break;
      case 12:
        // Clippers
        this.primaryColor = '200,16,46';
        this.secondaryColor = '29,66,148';
        break;
      case 13:
        // Dallas
        this.primaryColor = '0,83,188';
        this.secondaryColor = '0,43,92';
        break;
      case 14:
        // 76ers
        this.primaryColor = '0,107,182';
        this.secondaryColor = '237,23,76';
        break;
      case 15:
        // Chicago
        this.primaryColor = '206,17,65';
        this.secondaryColor = '6,25,34';
        break;
      case 16:
        // Charlotte
        this.primaryColor = '29,17,96';
        this.secondaryColor = '0,120,140';
        break;
      case 17:
        // Utah
        this.primaryColor = '0,43,92';
        this.secondaryColor = '0,71,27';
        break;
      case 18:
        // Phoenix
        this.primaryColor = '29,17,96';
        this.secondaryColor = '229,95,32';
        break;
      case 19:
        // Memphis
        this.primaryColor = '93,118,169';
        this.secondaryColor = '18,23,63';
        break;
      case 20:
        // Brooklyn
        this.primaryColor = '0,0,0';
        this.secondaryColor = '119,125,132';
        break;
      case 21:
        // Detroit
        this.primaryColor = '200,16,46';
        this.secondaryColor = '29,66,138';
        break;
      case 22:
        // Washington
        this.primaryColor = '0,43,92';
        this.secondaryColor = '227,24,55';
        break;
      case 23:
        // Portland
        this.primaryColor = '224,58,62';
        this.secondaryColor = '6,25,34';
        break;
      case 24:
        // Sacromento
        this.primaryColor = '91,43,130';
        this.secondaryColor = '99,113,122';
        break;
      case 25:
        // Spurs
        this.primaryColor = '196,206,211';
        this.secondaryColor = '6,25,34';
        break;
      case 26:
        // Knicks
        this.primaryColor = '0,107,182';
        this.secondaryColor = '245,132,38';
        break;
      case 27:
        // Cavs
        this.primaryColor = '134,0,56';
        this.secondaryColor = '4,30,66';
        break;
      case 28:
        // Atlanta
        this.primaryColor = '225,68,52';
        this.secondaryColor = '196,214,0';
        break;
      case 29:
        // Minnesota
        this.primaryColor = '12,35,64';
        this.secondaryColor = '35,97,146';
        break;
      case 30:
        // GSW
        this.primaryColor = '29,66,138';
        this.secondaryColor = '255,199,44';
        break;
      case 32:
        // New Orleans
        this.primaryColor = '0,22,65';
        this.secondaryColor = '225,58,62';
        break;
      default:
        this.primaryColor = '22, 24, 100';
        this.secondaryColor = '12, 126, 120';
        break;
    }
  }

}
