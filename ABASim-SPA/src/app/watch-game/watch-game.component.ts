import { Component, OnInit } from '@angular/core';
import { LeagueService } from '../_services/league.service';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { TransferService } from '../_services/transfer.service';
import { GameDetails } from '../_models/gameDetails';
import { PlayByPlay } from '../_models/playByPlay';
import { Router } from '@angular/router';
import { League } from '../_models/league';
import { TeamService } from '../_services/team.service';
import { Team } from '../_models/team';
import { GetGameLeague } from '../_models/getGameLeague';

@Component({
  selector: 'app-watch-game',
  templateUrl: './watch-game.component.html',
  styleUrls: ['./watch-game.component.css']
})
export class WatchGameComponent implements OnInit {
  league: League;
  gameId: number;
  state: number;
  gameDetails: GameDetails;
  gameBegun = 0;
  playByPlays: PlayByPlay[] = [];
  playNumber = 0;
  displayedPlayByPlays: PlayByPlay[] = [];
  numberOfPlays = 0;
  playNo = 0;
  displayBoxScoresButtons = 0;

  team: Team;

  constructor(private alertify: AlertifyService, private authService: AuthService, private leagueService: LeagueService,
              private transferService: TransferService, private router: Router, private teamService: TeamService) { }

  ngOnInit() {
    this.gameId = this.transferService.getData();
    this.state = this.transferService.getState();

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
      });
    } else if (this.state === 1) {
      this.leagueService.getGameDetailsSeason(gameLeague).subscribe(result => {
        this.gameDetails = result;
      }, error => {
        this.alertify.error('Error getting game details');
      });
    } else if (this.state === 2) {
      this.leagueService.getGameDetailsPlayoffs(gameLeague).subscribe(result => {
        this.gameDetails = result;
      }, error => {
        this.alertify.error('Error getting game details');
      });
    }
  }

  beginGame() {
    this.gameBegun = 1;

    const gameLeague: GetGameLeague = {
      gameId: this.gameId,
      leagueId: this.league.id
    };

    if (this.state === 1 || this.state === 0) {
      this.leagueService.getPlayByPlaysForId(gameLeague).subscribe(result => {
        this.playByPlays = result;
        const element = this.playByPlays[this.playByPlays.length - 1];
        this.numberOfPlays = element.ordering;
        this.playByPlays.sort((n1, n2) => {
          if (n1.ordering > n2.ordering) {
              return 1;
          }
          if (n1.ordering < n2.ordering) {
              return -1;
          }
          return 0;
        });
      }, error => {
        this.alertify.error('Error getting Play by Play');
      }, () => {
        const refreshId = setInterval(() => {
          this.displayPlays();
          if (this.numberOfPlays === this.playNo) {
            this.displayBoxScoresButtons = 1;
            clearInterval(refreshId);
          }
        }, 3000);
      });
    } else if (this.state === 2) {
      this.leagueService.getPlayoffsPlayByPlaysForId(gameLeague).subscribe(result => {
        this.playByPlays = result;
        const element = this.playByPlays[this.playByPlays.length - 1];
        this.numberOfPlays = element.ordering;
        this.playByPlays.sort((n1, n2) => {
          if (n1.ordering > n2.ordering) {
              return 1;
          }
          if (n1.ordering < n2.ordering) {
              return -1;
          }
          return 0;
        });
      }, error => {
        this.alertify.error('Error getting Play by Play');
      }, () => {
        const refreshId = setInterval(() => {
          this.displayPlays();
          if (this.numberOfPlays === this.playNo) {
            this.displayBoxScoresButtons = 1;
            clearInterval(refreshId);
          }
        }, 3000);
      });
    }
  }

  displayPlays() {
    const filtered = this.playByPlays[this.playNo];
    this.displayedPlayByPlays.push(filtered);
    this.displayedPlayByPlays.sort((n1, n2) => {
      if (n1.ordering > n2.ordering) {
          return -1;
      }
      if (n1.ordering < n2.ordering) {
          return 1;
      }
      return 0;
    });
    this.playNo++;
  }

  displayingPlayByPlays() {
    const filtered = this.playByPlays.filter(x => x.playNumber === this.playNumber);

    filtered.forEach(element => {
      this.displayedPlayByPlays.push(element);
    });

    this.displayedPlayByPlays.sort((n1, n2) => {
      if (n1.ordering > n2.ordering) {
          return -1;
      }
      if (n1.ordering < n2.ordering) {
          return 1;
      }
      return 0;
    });

    this.playNumber++;
  }

  viewBoxScore(gameId: number) {
    this.transferService.setData(gameId);
    this.router.navigate(['/box-score']);
  }

}
