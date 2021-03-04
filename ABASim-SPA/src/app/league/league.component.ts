import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LeagueService } from '../_services/league.service';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { League } from '../_models/league';
import { GameDisplay } from '../_models/gameDisplay';
import { GameDisplayCurrent } from '../_models/gameDisplayCurrent';
import { AdminService } from '../_services/admin.service';
import { SimGame } from '../_models/simGame';
import { GameEngineService } from '../_services/game-engine.service';
import { TransferService } from '../_services/transfer.service';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-league',
  templateUrl: './league.component.html',
  styleUrls: ['./league.component.css']
})
export class LeagueComponent implements OnInit {
  league: League;
  isAdmin = 0;
  upcomingGames: GameDisplay[] = [];
  todaysGames: GameDisplayCurrent[] = [];
  noRun = 0;

  standingsSelected = 0;
  statsSelected = 0;
  scheduleSelected = 0;
  eventsSelected = 0;
  transactionsSelected = 0;
  injuresSelected = 0;
  awardsSelected = 0;

  constructor(private router: Router, private leagueService: LeagueService, private alertify: AlertifyService,
              private authService: AuthService, private adminService: AdminService, private gameEngine: GameEngineService,
              private transferService: TransferService, private spinner: NgxSpinnerService) { }

  ngOnInit() {
    // Check to see if the user is an admin user
    this.isAdmin = this.authService.isAdmin();

    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting League Details');
    }, () => {
      this.spinner.show();
      this.getTodaysEvents();
      this.getUpcomingEvents();
    });
  }

  eventsSelection() {
    this.transactionsSelected = 0;
    this.awardsSelected = 0;
    this.scheduleSelected = 0;
    this.statsSelected = 0;
    this.standingsSelected = 0;
    this.eventsSelected = 1;
    this.injuresSelected = 0;
  }

  standingsSelection() {
    this.transactionsSelected = 0;
    this.awardsSelected = 0;
    this.scheduleSelected = 0;
    this.statsSelected = 0;
    this.standingsSelected = 1;
    this.eventsSelected = 0;
    this.injuresSelected = 0;
  }

  statisticsSelection() {
    this.transactionsSelected = 0;
    this.awardsSelected = 0;
    this.scheduleSelected = 0;
    this.statsSelected = 1;
    this.standingsSelected = 0;
    this.eventsSelected = 0;
    this.injuresSelected = 0;
  }

  scheduleSelection() {
    this.transactionsSelected = 0;
    this.awardsSelected = 0;
    this.scheduleSelected = 1;
    this.statsSelected = 0;
    this.standingsSelected = 0;
    this.eventsSelected = 0;
    this.injuresSelected = 0;
  }

  transactionsSelection() {
    this.transactionsSelected = 1;
    this.awardsSelected = 0;
    this.scheduleSelected = 0;
    this.statsSelected = 0;
    this.standingsSelected = 0;
    this.eventsSelected = 0;
    this.injuresSelected = 0;
  }

  injuriesSelection() {
    this.transactionsSelected = 0;
    this.awardsSelected = 0;
    this.scheduleSelected = 0;
    this.statsSelected = 0;
    this.standingsSelected = 0;
    this.eventsSelected = 0;
    this.injuresSelected = 1;
  }

  awardsSelection() {
    this.transactionsSelected = 0;
    this.awardsSelected = 1;
    this.scheduleSelected = 0;
    this.statsSelected = 0;
    this.standingsSelected = 0;
    this.eventsSelected = 0;
    this.injuresSelected = 0;
  }

  getTodaysEvents() {
    if (this.league.stateId === 6 && this.league.day !== 0) {
      this.leagueService.getPreseasonGamesForToday().subscribe(result => {
        this.todaysGames = result;
      }, error => {
        this.alertify.error('Error getting todays events');
      }, () => {
        this.spinner.hide();
      });
    } else if (this.league.stateId === 7 && this.league.day !== 0) {
      this.leagueService.getSeasonGamesForToday().subscribe(result => {
        this.todaysGames = result;
      }, error => {
        this.alertify.error('Error getting todays events');
      }, () => {
        this.spinner.hide();
      });
    }
  }

  getUpcomingEvents() {
    // Preseason
    if (this.league.stateId === 6) {
      // Need to get the games for the day
      this.leagueService.getPreseasonGamesForTomorrow().subscribe(result => {
        this.upcomingGames = result;
      }, error => {
        this.alertify.error('Error getting upcoming games');
      });
    } else if (this.league.stateId === 7) {
      this.leagueService.getSeasonGamesForTomorrow().subscribe(result => {
        this.upcomingGames = result;
      }, error => {
        this.alertify.error('Error getting upcoming games');
      });
    }
  }

  runGame(game: GameDisplayCurrent) {
    this.noRun = 1;
    const simGame: SimGame = {
      awayId:  game.awayTeamId,
      homeId:  game.homeTeamId,
      gameId:  game.id,
    };

    this.gameEngine.startPreseasonGame(simGame).subscribe(result => {
    }, error => {
      this.alertify.error(error);
      this.noRun = 0;
    }, () => {
      // Need to pass feedback and re-get the days games
      this.alertify.success('Game run successfully');
      this.getTodaysEvents();
      this.noRun = 0;
    });
  }

  runGameSeason(game: GameDisplayCurrent) {
    this.noRun = 1;
    const simGame: SimGame = {
      awayId:  game.awayTeamId,
      homeId:  game.homeTeamId,
      gameId:  game.id,
    };

    this.gameEngine.startSeasonGame(simGame).subscribe(result => {
    }, error => {
      this.alertify.error(error);
      this.noRun = 0;
    }, () => {
      // Need to pass feedback and re-get the days games
      this.alertify.success('Game run successfully');
      this.noRun = 0;
      this.getTodaysEvents();
    });
  }

  watchGame(gameId: number) {
    console.log(gameId);
    this.transferService.setData(gameId);
    this.router.navigate(['/watch-game']);
  }

  viewBoxScore(gameId: number) {
    this.transferService.setData(gameId);
    this.router.navigate(['/box-score']);
  }

  goToStandings() {
    this.router.navigate(['/standings']);
  }

  goToStats() {
    this.router.navigate(['/stats']);
  }

  goToSchedule() {
    this.router.navigate(['/schedule']);
  }

  goToTransactions() {
    this.router.navigate(['/transactions']);
  }

  goToInjuries() {
    this.router.navigate(['/injuries']);
  }

  goToAwards() {
    this.router.navigate(['/awards']);
  }

  goToRetiredPlayers() {
    this.router.navigate(['/retired']);
  }

  fullGame(gameId: number, stateId: number) {
    this.transferService.setData(gameId);
    this.transferService.setState(stateId);
    this.router.navigate(['/full-game-comm']);
  }

  runGamePlayoffs(game: GameDisplayCurrent) {
    console.log('ashley testing here');
    this.noRun = 1;
    const simGame: SimGame = {
      awayId: game.awayTeamId,
      homeId: game.homeTeamId,
      gameId: game.id,
    };

    console.log(simGame);

    this.gameEngine.startPlayoffGame(simGame).subscribe(result => {
    }, error => {
      this.alertify.error(error);
      this.noRun = 0;
    }, () => {
      // Need to pass feedback and re-get the days games
      this.alertify.success('Game run successfully');
      this.noRun = 0;
      this.getTodaysEvents();
    });
  }

}
