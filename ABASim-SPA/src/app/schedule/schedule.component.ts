import { Component, OnInit } from '@angular/core';
import { League } from '../_models/league';
import { Router } from '@angular/router';
import { LeagueService } from '../_services/league.service';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Schedule } from '../_models/schedule';
import { TransferService } from '../_services/transfer.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { TeamService } from '../_services/team.service';
import { Team } from '../_models/team';
import { GetScheduleLeague } from '../_models/getScheduleLeague';

@Component({
  selector: 'app-schedule',
  templateUrl: './schedule.component.html',
  styleUrls: ['./schedule.component.css']
})
export class ScheduleComponent implements OnInit {
  league: League;
  isAdmin = 0;
  schedules: Schedule[] = [];
  gameDayViewing = 0;
  team: Team;

  constructor(private router: Router, private leagueService: LeagueService, private alertify: AlertifyService,
              private authService: AuthService, private transferService: TransferService, private spinner: NgxSpinnerService,
              private teamService: TeamService) { }

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
      this.getScheduleForDay(this.league.day);
    });
  }

  getScheduleForDay(day: number) {
    this.gameDayViewing = day;

    if (this.gameDayViewing > 150) {
      this.gameDayViewing = 150;
    }

    const summary: GetScheduleLeague = {
      day: this.gameDayViewing,
      leagueId: this.league.id
    };

    this.leagueService.getScheduleGames(summary).subscribe(result => {
      this.schedules = result;
    }, error => {
      this.alertify.error('Error getting schedule games');
    }, () => {
      this.spinner.hide();
    });
  }

  viewBoxScore(game: Schedule) {
    if (game.awayScore === 0 || game.homeScore === 0) {
      this.alertify.error('Game has not been played yet');
    } else {
      this.transferService.setData(game.gameId);
      this.router.navigate(['/box-score']);
    }
  }

  getDaysViewing() {
    let startNumber = 0;
    let endNumber = 0;
    if (this.gameDayViewing - 2 < 0) {
      startNumber = 1;
    } else if (this.gameDayViewing + 2 >= 150) {
      startNumber = 148;
    } else {
      startNumber = this.gameDayViewing - 2;
    }

    if (this.gameDayViewing + 2 > 150) {
      endNumber = 150;
    } else {
      endNumber = this.gameDayViewing + 2;
    }
    return 'Days ' + (startNumber.toString() + ' to ' + endNumber.toString());
  }

  getNextDays() {
    if (this.gameDayViewing - 3 > 150) {
      this.gameDayViewing = 150;
    } else {
      this.gameDayViewing = this.gameDayViewing + 3;
    }

    const summary: GetScheduleLeague = {
      day: this.gameDayViewing,
      leagueId: this.league.id
    };

    this.leagueService.getScheduleGames(summary).subscribe(result => {
      this.schedules = result;
    }, error => {
      this.alertify.error('Error getting schedule games');
    });
  }

  getPrevDays() {
    if (this.gameDayViewing - 3 < 1) {
      this.gameDayViewing = 1;
    } else {
      this.gameDayViewing = this.gameDayViewing - 3;
    }

    const summary: GetScheduleLeague = {
      day: this.gameDayViewing,
      leagueId: this.league.id
    };

    this.leagueService.getScheduleGames(summary).subscribe(result => {
      this.schedules = result;
    }, error => {
      this.alertify.error('Error getting schedule games');
    });
  }

  goToStandings() {
    this.router.navigate(['/standings']);
  }

  goToStats() {
    this.router.navigate(['/stats']);
  }

  goToLeague() {
    this.router.navigate(['/league']);
  }

  goToTransactions() {
    this.router.navigate(['/transactions']);
  }

}
