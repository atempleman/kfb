import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { LeagueService } from '../_services/league.service';
import { AlertifyService } from '../_services/alertify.service';
import { Standing } from '../_models/standing';
import { Router } from '@angular/router';
import { TeamService } from '../_services/team.service';
import { Team } from '../_models/team';
import { TransferService } from '../_services/transfer.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { League } from '../_models/league';
import { GetTeamLeague } from '../_models/getTeamLeague';
import { GetStandingLeague } from '../_models/getStandingLeague';

@Component({
  selector: 'app-standings',
  templateUrl: './standings.component.html',
  styleUrls: ['./standings.component.css']
})
export class StandingsComponent implements OnInit {
  statusConference = true;
  statusDivision = false;
  statusLeague = false;

  eastStandings: Standing[] = [];
  westStandings: Standing[] = [];

  allStandings: Standing[] = [];

  atlanticStandings: Standing[] = [];
  centralstandings: Standing[] = [];
  southeastStandings: Standing[] = [];
  northwestStandings: Standing[] = [];
  pacificStandings: Standing[] = [];
  southwestStandings: Standing[] = [];

  team: Team;
  league: League;

  constructor(private leagueService: LeagueService, private alertify: AlertifyService, private transferService: TransferService,
              private authService: AuthService, private router: Router, private teamService: TeamService,
              private spinner: NgxSpinnerService) { }

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
      this.setupPage();
    });
  }

  setupPage() {
    const summary: GetStandingLeague = {
      value: 1,
      leagueId: this.league.id
    };

    this.leagueService.getConferenceStandings(summary).subscribe(result => {
      this.spinner.show();
      // tslint:disable-next-line: max-line-length
      this.eastStandings = result.sort((a, b) => (a.wins / a.gamesPlayed) < (b.wins / b.gamesPlayed) ? 1 : (a.wins / a.gamesPlayed) > (b.wins / b.gamesPlayed) ? -1 : 0);
      // this.eastStandings = result;
    }, error => {
      this.alertify.success('Error getting eastern conference standings');
    }, () => {
      this.spinner.hide();
    });

    const westSummary: GetStandingLeague = {
      value: 1,
      leagueId: this.league.id
    };

    this.leagueService.getConferenceStandings(westSummary).subscribe(result => {
      this.spinner.show();
      // tslint:disable-next-line: max-line-length
      this.westStandings = result.sort((a, b) => (a.wins / a.gamesPlayed) < (b.wins / b.gamesPlayed) ? 1 : (a.wins / a.gamesPlayed) > (b.wins / b.gamesPlayed) ? -1 : 0);
      // this.westStandings = result;
    }, error => {
      this.alertify.success('Error getting western conference standings');
    }, () => {
      this.spinner.hide();
    });
  }

  conferenceClick() {
    const conf1Summary: GetStandingLeague = {
      value: 1,
      leagueId: this.league.id
    };

    this.leagueService.getConferenceStandings(conf1Summary).subscribe(result => {
      // this.eastStandings = result;
      // tslint:disable-next-line: max-line-length
      this.eastStandings = result.sort((a, b) => (a.wins / a.gamesPlayed) < (b.wins / b.gamesPlayed) ? 1 : (a.wins / a.gamesPlayed) > (b.wins / b.gamesPlayed) ? -1 : 0);
    }, error => {
      this.alertify.error('Error getting eastern conference standings');
    });

    const conf2Summary: GetStandingLeague = {
      value: 2,
      leagueId: this.league.id
    };

    this.leagueService.getConferenceStandings(conf2Summary).subscribe(result => {
      // this.westStandings = result;
      // tslint:disable-next-line: max-line-length
      this.westStandings = result.sort((a, b) => (a.wins / a.gamesPlayed) < (b.wins / b.gamesPlayed) ? 1 : (a.wins / a.gamesPlayed) > (b.wins / b.gamesPlayed) ? -1 : 0);
    }, error => {
      this.alertify.error('Error getting western conference standings');
    });

    this.statusLeague = false;
    this.statusDivision = false;
    this.statusConference = true;
  }

  divisionClick() {
    const div1Summary: GetStandingLeague = {
      value: 1,
      leagueId: this.league.id
    };

    this.leagueService.getDivisionStandings(div1Summary).subscribe(result => {
      // this.atlanticStandings = result;
      // tslint:disable-next-line: max-line-length
      this.atlanticStandings = result.sort((a, b) => (a.wins / a.gamesPlayed) < (b.wins / b.gamesPlayed) ? 1 : (a.wins / a.gamesPlayed) > (b.wins / b.gamesPlayed) ? -1 : 0);
    }, error => {
      this.alertify.error('Error getting division standings');
    });

    const div2Summary: GetStandingLeague = {
      value: 2,
      leagueId: this.league.id
    };

    this.leagueService.getDivisionStandings(div2Summary).subscribe(result => {
      // this.centralstandings = result;
      // tslint:disable-next-line: max-line-length
      this.centralstandings = result.sort((a, b) => (a.wins / a.gamesPlayed) < (b.wins / b.gamesPlayed) ? 1 : (a.wins / a.gamesPlayed) > (b.wins / b.gamesPlayed) ? -1 : 0);
    }, error => {
      this.alertify.error('Error getting division standings');
    });

    const div3Summary: GetStandingLeague = {
      value: 3,
      leagueId: this.league.id
    };

    this.leagueService.getDivisionStandings(div3Summary).subscribe(result => {
      // this.southeastStandings = result;
      // tslint:disable-next-line: max-line-length
      this.southeastStandings = result.sort((a, b) => (a.wins / a.gamesPlayed) < (b.wins / b.gamesPlayed) ? 1 : (a.wins / a.gamesPlayed) > (b.wins / b.gamesPlayed) ? -1 : 0);
    }, error => {
      this.alertify.error('Error getting division standings');
    });

    const div4Summary: GetStandingLeague = {
      value: 4,
      leagueId: this.league.id
    };

    this.leagueService.getDivisionStandings(div4Summary).subscribe(result => {
      // this.northwestStandings = result;
      // tslint:disable-next-line: max-line-length
      this.northwestStandings = result.sort((a, b) => (a.wins / a.gamesPlayed) < (b.wins / b.gamesPlayed) ? 1 : (a.wins / a.gamesPlayed) > (b.wins / b.gamesPlayed) ? -1 : 0);
    }, error => {
      this.alertify.error('Error getting division standings');
    });

    const div5Summary: GetStandingLeague = {
      value: 5,
      leagueId: this.league.id
    };

    this.leagueService.getDivisionStandings(div5Summary).subscribe(result => {
      // this.pacificStandings = result;
      // tslint:disable-next-line: max-line-length
      this.pacificStandings = result.sort((a, b) => (a.wins / a.gamesPlayed) < (b.wins / b.gamesPlayed) ? 1 : (a.wins / a.gamesPlayed) > (b.wins / b.gamesPlayed) ? -1 : 0);
    }, error => {
      this.alertify.error('Error getting division standings');
    });

    const div6Summary: GetStandingLeague = {
      value: 6,
      leagueId: this.league.id
    };

    this.leagueService.getDivisionStandings(div6Summary).subscribe(result => {
      // this.southwestStandings = result;
      // tslint:disable-next-line: max-line-length
      this.southwestStandings = result.sort((a, b) => (a.wins / a.gamesPlayed) < (b.wins / b.gamesPlayed) ? 1 : (a.wins / a.gamesPlayed) > (b.wins / b.gamesPlayed) ? -1 : 0);
    }, error => {
      this.alertify.error('Error getting division standings');
    });

    this.statusConference = false;
    this.statusLeague = false;
    this.statusDivision = true;
  }

  leagueClick() {
    this.leagueService.getLeagueStandings(this.league.id).subscribe(result => {
      // this.allStandings = result;
      // tslint:disable-next-line: max-line-length
      this.allStandings = result.sort((a, b) => (a.wins / a.gamesPlayed) < (b.wins / b.gamesPlayed) ? 1 : (a.wins / a.gamesPlayed) > (b.wins / b.gamesPlayed) ? -1 : 0);
    }, error => {
      this.alertify.error('Error getting league standings');
    });

    this.statusConference = false;
    this.statusDivision = false;
    this.statusLeague = true;
  }

  getWinPercantage(wins: number, played: number) {
    return (wins / played).toFixed(3);
  }

  goToStats() {
    this.router.navigate(['/stats']);
  }

  goToLeague() {
    this.router.navigate(['/league']);
  }

  goToSchedule() {
    this.router.navigate(['/schedule']);
  }

  goToTransactions() {
    this.router.navigate(['/transactions']);
  }

  viewTeam(name: string) {
    // Need to go a call to get the team id
    let team: Team;

    // GetTeamLeague
    const summary: GetTeamLeague = {
      teamname: name,
      leagueId: this.league.id
    };

    this.teamService.getTeamForTeamName(summary).subscribe(result => {
      team = result;
    }, error => {
      this.alertify.error('Error getting players team');
    }, () => {
      this.transferService.setData(team.id);
      this.router.navigate(['/view-team']);
    });
  }

}
