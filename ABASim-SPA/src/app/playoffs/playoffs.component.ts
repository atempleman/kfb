import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LeagueService } from '../_services/league.service';
import { AlertifyService } from '../_services/alertify.service';
import { League } from '../_models/league';
import { AuthService } from '../_services/auth.service';
import { PlayoffSummary } from '../_models/playoffSummary';
import { TeamService } from '../_services/team.service';
import { Team } from '../_models/team';
import { GetPlayoffSummary } from '../_models/getPlayoffSummary';

@Component({
  selector: 'app-playoffs',
  templateUrl: './playoffs.component.html',
  styleUrls: ['./playoffs.component.css']
})
export class PlayoffsComponent implements OnInit {
  league: League;
  isAdmin = 0;
  playoffRoundSelection = 0;
  playoffSummaries: PlayoffSummary[] = [];

  ppSelected = 1;
  statsSelected = 0;
  scheduleSelected = 0;

  team: Team;

  constructor(private router: Router, private leagueService: LeagueService, private alertify: AlertifyService,
              private authService: AuthService, private teamService: TeamService) { }

  ngOnInit() {
    // Check to see if the user is an admin user
    this.isAdmin = this.authService.isAdmin();

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
    this.leagueService.getLeagueForUserId(this.team.id).subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting League Details');
    }, () => {
    });
  }

  playoffRoundSelected() {
    if (+this.playoffRoundSelection === 1) {
      const summary: GetPlayoffSummary = {
        round: 1,
        leagueId: this.league.id
      };
      this.leagueService.getFirstRoundSummaries(summary).subscribe(result => {
        this.playoffSummaries = result;
      }, error => {
        this.alertify.error('Error getting first round summaries');
      });
    } else if (+this.playoffRoundSelection === 2) {
      const summary: GetPlayoffSummary = {
        round: 2,
        leagueId: this.league.id
      };
        this.leagueService.getFirstRoundSummaries(summary).subscribe(result => {
          this.playoffSummaries = result;
        }, error => {
          this.alertify.error('Error getting first round summaries');
        });
    } else if (+this.playoffRoundSelection === 3) {
      const summary: GetPlayoffSummary = {
        round: 3,
        leagueId: this.league.id
      };
      this.leagueService.getFirstRoundSummaries(summary).subscribe(result => {
        this.playoffSummaries = result;
      }, error => {
        this.alertify.error('Error getting first round summaries');
      });
    } else if (+this.playoffRoundSelection === 4) {
      const summary: GetPlayoffSummary = {
        round: 4,
        leagueId: this.league.id
      };
      this.leagueService.getFirstRoundSummaries(summary).subscribe(result => {
        this.playoffSummaries = result;
      }, error => {
        this.alertify.error('Error getting first round summaries');
      });
    }
  }

  goToStats() {
    this.router.navigate(['/playoffs-stats']);
  }

  goToResults() {
    this.router.navigate(['/playoffs-results']);
  }

  playoffPicture() {
    this.statsSelected = 0;
    this.scheduleSelected = 0;
    this.ppSelected = 1;
  }

  statisticsSelection() {
    this.ppSelected = 0;
    this.scheduleSelected = 0;
    this.statsSelected = 1;
  }

  scheduleSelection() {
    this.ppSelected = 0;
    this.statsSelected = 0;
    this.scheduleSelected = 1;
  }

}
