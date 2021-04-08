import { Component, OnInit } from '@angular/core';
import { League } from '../_models/league';
import { Router } from '@angular/router';
import { LeagueService } from '../_services/league.service';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { TransferService } from '../_services/transfer.service';
import { PlayoffResult } from '../_models/playoffResult';
import { TeamService } from '../_services/team.service';
import { Team } from '../_models/team';
import { GetScheduleLeague } from '../_models/getScheduleLeague';

@Component({
  selector: 'app-playoff-results',
  templateUrl: './playoff-results.component.html',
  styleUrls: ['./playoff-results.component.css']
})
export class PlayoffResultsComponent implements OnInit {
  league: League;
  isAdmin = 0;
  schedules: PlayoffResult[] = [];
  gameDayViewing = 0;
  team: Team;

  constructor(private router: Router, private leagueService: LeagueService, private alertify: AlertifyService,
              private authService: AuthService, private transferService: TransferService,
              private teamService: TeamService) { }

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
      this.getScheduleForDay(this.league.day);
    });
  }

  getScheduleForDay(day: number) {
    this.gameDayViewing = day;

    if (this.gameDayViewing - 2 <= 218) {
      this.gameDayViewing = 218;
    } else if (this.gameDayViewing + 2 >= 282) {
      this.gameDayViewing = 282;
    }

    const summary: GetScheduleLeague = {
      day: this.gameDayViewing,
      leagueId: this.league.id
    };

    this.leagueService.getPlayoffGames(summary).subscribe(result => {
      this.schedules = result;
    }, error => {
      this.alertify.error('Error getting schedule games');
    });
  }

  viewBoxScore(game: PlayoffResult) {
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
    if (this.gameDayViewing - 2 < 218) {
      startNumber = 218;
    } else if (this.gameDayViewing + 2 >= 282) {
      startNumber = 280;
    } else {
      startNumber = this.gameDayViewing - 2;
    }

    if (this.gameDayViewing + 2 > 282) {
      endNumber = 282;
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

    this.leagueService.getPlayoffGames(summary).subscribe(result => {
      this.schedules = result;
      console.log(this.schedules);
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

    this.leagueService.getPlayoffGames(summary).subscribe(result => {
      this.schedules = result;
      console.log(this.schedules);
    }, error => {
      this.alertify.error('Error getting schedule games');
    });
  }

}
