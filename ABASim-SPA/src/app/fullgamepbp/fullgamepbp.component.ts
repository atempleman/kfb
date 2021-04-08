import { Component, OnInit } from '@angular/core';
import { League } from '../_models/league';
import { GameDetails } from '../_models/gameDetails';
import { PlayByPlay } from '../_models/playByPlay';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { Router } from '@angular/router';
import { TransferService } from '../_services/transfer.service';
import { LeagueService } from '../_services/league.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { TeamService } from '../_services/team.service';
import { Team } from '../_models/team';
import { GetGameLeague } from '../_models/getGameLeague';

@Component({
  selector: 'app-fullgamepbp',
  templateUrl: './fullgamepbp.component.html',
  styleUrls: ['./fullgamepbp.component.css']
})
export class FullgamepbpComponent implements OnInit {
  league: League;
  gameId: number;
  state: number;
  gameDetails: GameDetails;
  gameBegun = 0;
  playByPlays: PlayByPlay[] = [];
  playNumber = 0;
  numberOfPlays = 0;
  playNo = 0;
  displayBoxScoresButtons = 0;
  team: Team;

  constructor(private alertify: AlertifyService, private authService: AuthService, private leagueService: LeagueService,
              private transferService: TransferService, private router: Router, private spinner: NgxSpinnerService,
              private teamService: TeamService) { }

  ngOnInit() {
    this.gameId = this.transferService.getData();
    this.state = this.transferService.getState();
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
      this.getGameDetails();
    });
  }

  getGameDetails() {
    const gameLeague: GetGameLeague = {
      gameId: this.gameId,
      leagueId: this.league.id
    };

    if (this.state === 0) {
      this.leagueService.getGameDetailsPreseason(gameLeague).subscribe(result => {
        this.gameDetails = result;
      }, error => {
        this.alertify.error('Error getting game details');
      }, () => {
        this.leagueService.getPlayByPlaysForId(gameLeague).subscribe(result => {
          this.playByPlays = result;
          const element = this.playByPlays[this.playByPlays.length - 1];
          this.numberOfPlays = element.ordering;
          this.playByPlays.sort((n1, n2) => {
            if (n1.ordering < n2.ordering) {
                return -1;
            }
            if (n1.ordering > n2.ordering) {
                return 1;
            }
            return 0;
          });
        }, error => {
          this.alertify.error('Error getting Play by Play');
        }, () => {
          this.spinner.hide();
        });
      });
    } else if (this.state === 1) {
      this.leagueService.getGameDetailsSeason(gameLeague).subscribe(result => {
        this.gameDetails = result;
      }, error => {
        this.alertify.error('Error getting game details');
      }, () => {
        this.leagueService.getPlayByPlaysForId(gameLeague).subscribe(result => {
          this.playByPlays = result;
          const element = this.playByPlays[this.playByPlays.length - 1];
          this.numberOfPlays = element.ordering;
          this.playByPlays.sort((n1, n2) => {
            if (n1.ordering < n2.ordering) {
                return -1;
            }
            if (n1.ordering > n2.ordering) {
                return 1;
            }
            return 0;
          });
        }, error => {
          this.alertify.error('Error getting Play by Play');
        }, () => {
          this.spinner.hide();
        });
      });
    } else if (this.state === 2) {
      this.leagueService.getGameDetailsPlayoffs(gameLeague).subscribe(result => {
        this.gameDetails = result;
      }, error => {
        this.alertify.error('Error getting game details');
      }, () => {
        this.leagueService.getPlayoffsPlayByPlaysForId(gameLeague).subscribe(result => {
          this.playByPlays = result;
          const element = this.playByPlays[this.playByPlays.length - 1];
          this.numberOfPlays = element.ordering;
          this.playByPlays.sort((n1, n2) => {
            if (n1.ordering < n2.ordering) {
                return -1;
            }
            if (n1.ordering > n2.ordering) {
                return 1;
            }
            return 0;
          });
        }, error => {
          this.alertify.error('Error getting Play by Play');
        }, () => {
          this.spinner.hide();
        });
      });
    }
  }

  viewBoxScore(gameId: number) {
    this.transferService.setData(gameId);
    this.router.navigate(['/box-score']);
  }
}
