import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LeagueService } from '../_services/league.service';
import { AlertifyService } from '../_services/alertify.service';
import { League } from '../_models/league';
import { AuthService } from '../_services/auth.service';
import { TeamService } from '../_services/team.service';
import { Team } from '../_models/team';
import { GameDisplay } from '../_models/gameDisplay';
import { GameDisplayCurrent } from '../_models/gameDisplayCurrent';
import { AdminService } from '../_services/admin.service';
import { SimGame } from '../_models/simGame';
import { GameEngineService } from '../_services/game-engine.service';
import { TransferService } from '../_services/transfer.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { LeagueLeadersPoints } from '../_models/leagueLeadersPoints';
import { LeagueLeadersRebounds } from '../_models/leagueLeadersRebounds';
import { LeagueLeadersAssists } from '../_models/leagueLeadersAssists';
import { LeagueLeadersSteals } from '../_models/leagueLeadersSteals';
import { LeagueLeadersBlocks } from '../_models/leagueLeadersBlocks';
import { PlayoffSummary } from '../_models/playoffSummary';
import { GlobalChat } from '../_models/globalChat';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { User } from '../_models/user';
import { formatDate } from '@angular/common';
import { ContactService } from '../_services/contact.service';
import { DraftTracker } from '../_models/draftTracker';
import { DraftService } from '../_services/draft.service';
import { DashboardDraftPick } from '../_models/dashboardDraftPick';
import { Transaction } from '../_models/transaction';
import { Votes } from '../_models/votes';
import { QuickViewPlayer } from '../_models/QuickViewPlayer';
import { LeaguePlayerInjury } from '../_models/leaguePlayerInjury';
import { GetPlayoffSummary } from '../_models/getPlayoffSummary';
import { GetRosterQuickView } from '../_models/getRosterQuickView';
import { GetDashboardPicks } from '../_models/getDashboardPicks';
import { GetTeamLeague } from '../_models/getTeamLeague';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  league: League;
  team: Team;
  champion: Team;
  isAdmin = 0;
  upcomingGames: GameDisplay[] = [];
  todaysGames: GameDisplayCurrent[] = [];
  playoffSummaries: PlayoffSummary[] = [];
  noRun = 0;

  teamToggle = 0;
  leagueToggle = 0;
  chatToggle = 1;

  quickTeamRoster: QuickViewPlayer[] = [];
  quickRostCount = 0;
  quickTeamInjuries: LeaguePlayerInjury[] = [];

  topFivePoints: LeagueLeadersPoints[] = [];
  topFiveRebounds: LeagueLeadersRebounds[] = [];
  topFiveAssists: LeagueLeadersAssists[] = [];
  topFiveSteals: LeagueLeadersSteals[] = [];
  topFiveBlocks: LeagueLeadersBlocks[] = [];

  chatRecords: GlobalChat[] = [];
  chatForm: FormGroup;
  newChatForm: FormGroup;
  user: User;
  interval;
  draftInterval;

  tracker: DraftTracker;
  currentPick: DashboardDraftPick;
  previousPick: DashboardDraftPick;
  nextPick: DashboardDraftPick;

  yesterdaysTransactions: Transaction[] = [];

  mvp: Votes[] = [];
  sixth: Votes[] = [];
  dpoy: Votes[] = [];

  primaryColor: string = '22, 24, 100';
  secondaryColor: string = '12,126,120';
  primaryTextColor: string = '220, 220, 220';
  secondaryTextColor: string = '220, 220, 220';

  secondaryLinkColor: string = '3, 252, 161';


  constructor(private router: Router, private leagueService: LeagueService, private alertify: AlertifyService,
    private authService: AuthService, private teamService: TeamService, private adminService: AdminService,
    private gameEngine: GameEngineService, private transferService: TransferService, private spinner: NgxSpinnerService,
    private fb: FormBuilder, private contactService: ContactService, private draftService: DraftService) { }

  ngOnInit() {
    this.spinner.show();
    this.createChatForm();

    // Check to see if the user is an admin user
    this.isAdmin = this.authService.isAdmin();
    localStorage.setItem('isAdmin', this.isAdmin.toString());

    this.teamService.getTeamForUserId(this.authService.decodedToken.nameid).subscribe(result => {
      this.team = result;
      // Need to persist the team to cookie
      localStorage.setItem('teamId', this.team.teamId.toString());
    }, error => {
      this.alertify.error('Error getting your Team');
    }, () => {
      this.backgroundStyle();
      this.setupLeague();
    });
  }

  setupLeague() {
    this.leagueService.getLeagueForUserId(this.authService.decodedToken.nameid).subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting League Details');
    }, () => {
      this.setupDashboard();
      this.refreshChat();
      this.interval = setInterval(() => {
        this.refreshChat();
      }, 600000);
    });
  }

  setupDashboard() {
    this.getTeamRoster();
    this.getTeamInjuries();

    if (this.league.stateId === 4) {
      this.getDraftTracker();
    } else if (this.league.stateId === 7) {
      this.getLeagueLeaders();
    } else if (this.league.stateId === 8) {
      // get the playoff series
      this.getRoundOneSummaries();
    } else if (this.league.stateId === 9) {
      // get playoff series
      this.getConfSemiSummaries();
    } else if (this.league.stateId === 10) {
      this.getConfFinalsSummaries();
    } else if (this.league.stateId === 11) {
      this.getFinalsSummaries();
    }

    if (this.league.stateId === 3 || this.league.stateId === 4) {
      this.interval = setInterval(() => {
        this.getPicksToDisplay();
      }, 210000);
    }

    this.getTodaysEvents();
    this.getYesterdaysTransactions();

    if (this.league.stateId === 11 && this.league.day > 28) {
      this.leagueService.getChampion(this.league.id).subscribe(result => {
        this.champion = result;
      }, error => {
        this.alertify.error('Error getting the champion');
      }, () => {
        this.spinner.hide();
      });
    } else {
      this.spinner.hide();
    }

    if (this.league.stateId > 7) {
      this.leagueService.getMvpTopFive(this.league.id).subscribe(result => {
        this.mvp = result;
      }, error => {
        this.alertify.error('Error getting MVP');
      });

      this.leagueService.getSixthManTopFive(this.league.id).subscribe(result => {
        this.sixth = result;
      }, error => {
        this.alertify.error('Error getting Sixth Man');
      });

      this.leagueService.getDpoyTopFive(this.league.id).subscribe(result => {
        this.dpoy = result;
      }, error => {
        this.alertify.error('Error getting DPOY');
      });
    }
  }

  viewPlayer(player: number) {
    this.transferService.setData(player);
    this.router.navigate(['/view-player']);
  }

  getRoundOneSummaries() {
    const summary: GetPlayoffSummary = {
      round: 1,
      leagueId: this.league.id
    };
    this.leagueService.getFirstRoundSummaries(summary).subscribe(result => {
      this.playoffSummaries = result;
    }, error => {
      this.alertify.error('Error getting playoff summaries');
    });
  }

  getConfSemiSummaries() {
    const summary: GetPlayoffSummary = {
      round: 2,
      leagueId: this.league.id
    };
    this.leagueService.getFirstRoundSummaries(summary).subscribe(result => {
      this.playoffSummaries = result;
    }, error => {
      this.alertify.error('Error getting playoff summaries');
    });
  }

  getConfFinalsSummaries() {
    const summary: GetPlayoffSummary = {
      round: 3,
      leagueId: this.league.id
    };
    this.leagueService.getFirstRoundSummaries(summary).subscribe(result => {
      this.playoffSummaries = result;
    }, error => {
      this.alertify.error('Error getting playoff summaries');
    });
  }

  getFinalsSummaries() {
    const summary: GetPlayoffSummary = {
      round: 4,
      leagueId: this.league.id
    };
    this.leagueService.getFirstRoundSummaries(summary).subscribe(result => {
      this.playoffSummaries = result;
    }, error => {
      this.alertify.error('Error getting playoff summaries');
    });
  }

  getLeagueLeaders() {
    console.log('ash');
    this.leagueService.getTopFivePoints(this.league.id).subscribe(result => {
      this.topFivePoints = result;
      console.log(result);
    }, error => {
      this.alertify.error('Error getting points leaders');
    });

    this.leagueService.getTopFiveAssists(this.league.id).subscribe(result => {
      this.topFiveAssists = result;
    }, error => {
      this.alertify.error('Error getting assists leaders');
    });

    this.leagueService.getTopFiveBlocks(this.league.id).subscribe(result => {
      this.topFiveBlocks = result;
    }, error => {
      this.alertify.error('Error getting blocks leaders');
    });

    this.leagueService.getTopFiveRebounds(this.league.id).subscribe(result => {
      this.topFiveRebounds = result;
    }, error => {
      this.alertify.error('Error getting rebounds leaders');
    });

    this.leagueService.getTopFiveSteals(this.league.id).subscribe(result => {
      this.topFiveSteals = result;
    }, error => {
      this.alertify.error('Error getting steals leaders');
    });
  }

  getTodaysEvents() {
    if (this.league.stateId === 6 && this.league.day !== 0) {
      this.leagueService.getPreseasonGamesForToday(this.league.id).subscribe(result => {
        this.todaysGames = result;
      }, error => {
        this.alertify.error('Error getting todays events');
      }, () => {
        this.spinner.hide();
      });
    } else if (this.league.stateId === 7 && this.league.day !== 0) {
      this.leagueService.getSeasonGamesForToday(this.league.id).subscribe(result => {
        this.todaysGames = result;
      }, error => {
        this.alertify.error('Error getting todays events');
      }, () => {
        this.spinner.hide();
      });
    } else if (this.league.stateId === 8 && this.league.day !== 0) {
      this.leagueService.getFirstRoundGamesForToday(this.league.id).subscribe(result => {
        this.todaysGames = result;
      }, error => {
        this.alertify.error('Error gettings todays events');
      }, () => {
        this.spinner.hide();
      });
    } else if (this.league.stateId === 9 && this.league.day !== 0) {
      this.leagueService.getFirstRoundGamesForToday(this.league.id).subscribe(result => {
        this.todaysGames = result;
      }, error => {
        this.alertify.error('Error gettings todays events');
      }, () => {
        this.spinner.hide();
      });
    } else if (this.league.stateId === 10 && this.league.day !== 0) {
      this.leagueService.getFirstRoundGamesForToday(this.league.id).subscribe(result => {
        this.todaysGames = result;
      }, error => {
        this.alertify.error('Error gettings todays events');
      }, () => {
        this.spinner.hide();
      });
    } else if (this.league.stateId === 11 && this.league.day !== 0) {
      this.leagueService.getFirstRoundGamesForToday(this.league.id).subscribe(result => {
        this.todaysGames = result;
      }, error => {
        this.alertify.error('Error gettings todays events');
      }, () => {
        this.spinner.hide();
      });
    } else {
      console.log(this.todaysGames.length);
      console.log(this.league.stateId);
    }
  }

  // getUpcomingEvents() {
  //   // Preseason
  //   if (this.league.stateId === 6) {
  //     // Need to get the games for the day
  //     this.leagueService.getPreseasonGamesForTomorrow(this.league.id).subscribe(result => {
  //       this.upcomingGames = result;
  //     }, error => {
  //       this.alertify.error('Error getting upcoming games');
  //     });
  //   } else if (this.league.stateId === 7) {
  //     this.leagueService.getSeasonGamesForTomorrow(this.league.id).subscribe(result => {
  //       this.upcomingGames = result;
  //     }, error => {
  //       this.alertify.error('Error getting upcoming games');
  //     });
  //   }
  // }

  getTeamRoster() {
    const quickView: GetRosterQuickView = {
      teamId: this.team.teamId,
      leagueId: this.league.id
    };

    if (this.league.stateId > 7) {
      this.teamService.getQuickViewRosterPlayoffsForTeam(quickView).subscribe(result => {
        this.quickTeamRoster = result;
        this.quickRostCount = this.quickTeamRoster.length;
      }, error => {
        this.alertify.error('Error getting your roster');
      });
    } else {
      this.teamService.getQuickViewRosterForTeam(quickView).subscribe(result => {
        this.quickTeamRoster = result;
        this.quickRostCount = this.quickTeamRoster.length;
      }, error => {
        this.alertify.error('Error getting your roster');
      });
    }
  }

  getTeamInjuries() {
    const quickView: GetRosterQuickView = {
      teamId: this.team.teamId,
      leagueId: this.league.id
    };
    this.teamService.getInjuriesForTeam(quickView).subscribe(result => {
      this.quickTeamInjuries = result;
    }, error => {
      this.alertify.error('Error getting injuries');
    });
  }

  goToRoster() {
    this.router.navigate(['/team']);
  }

  goToDraft() {
    this.router.navigate(['/draft']);
  }

  goToAdmin() {
    this.router.navigate(['/admin']);
  }

  goToLeague() {
    this.router.navigate(['/league']);
  }

  goToPlayers() {
    this.router.navigate(['/players']);
  }

  runGame(game: GameDisplayCurrent) {
    this.noRun = 1;
    const simGame: SimGame = {
      awayId: game.awayTeamId,
      homeId: game.homeTeamId,
      gameId: game.id,
      leagueId: this.league.id
    };

    this.gameEngine.startPreseasonGame(simGame).subscribe(result => {
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

  runGamePlayoffs(game: GameDisplayCurrent) {
    this.noRun = 1;
    const simGame: SimGame = {
      awayId: game.awayTeamId,
      homeId: game.homeTeamId,
      gameId: game.id,
      leagueId: this.league.id
    };

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

  runGameSeason(game: GameDisplayCurrent) {
    this.noRun = 1;
    const simGame: SimGame = {
      awayId: game.awayTeamId,
      homeId: game.homeTeamId,
      gameId: game.id,
      leagueId: this.league.id
    };

    console.log(simGame.leagueId);

    this.gameEngine.startSeasonGame(simGame).subscribe(result => {
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

  watchGame(gameId: number, stateId: number) {
    this.transferService.setData(gameId);
    this.transferService.setState(stateId);
    this.router.navigate(['/watch-game']);
  }

  fullGame(gameId: number, stateId: number) {
    this.transferService.setData(gameId);
    this.transferService.setState(stateId);
    this.router.navigate(['/full-game-comm']);
  }

  viewBoxScore(gameId: number) {
    this.transferService.setData(gameId);
    this.router.navigate(['/box-score']);
  }

  goToStats(value: number) {
    this.transferService.setData(value);
    this.router.navigate(['/stats']);
  }

  goToPlayoffs() {
    this.router.navigate(['/playoffs']);
  }

  refreshChat() {
    this.contactService.getChatRecords(this.league.id).subscribe(result => {
      this.chatRecords = result;
    }, error => {
      this.alertify.error('Error getting chat messages');
    });
  }

  createChatForm() {
    this.chatForm = this.fb.group({
      message: ['']
    });
  }

  sendChat() {
    if (this.chatForm.valid) {
      const result = this.chatForm.controls['message'].value;
      const dt = formatDate(new Date(), 'dd/MM/yyyy', 'en');
      const chatRecord: GlobalChat = {
        chatText: result,
        username: this.authService.decodedToken.nameid,
        chatTime: dt.toString(),
        leagueId: this.league.id
      };

      this.contactService.sendChat(chatRecord).subscribe(rst => {
      }, error => {
        this.alertify.error('Error sending message');
      }, () => {
        this.alertify.success('Message posted');
        this.chatForm.reset();
        // Need to get the chat messages again
        this.contactService.getChatRecords(this.league.id).subscribe(r => {
          this.chatRecords = r;
        }, error => {
          this.alertify.error('Error getting chat messages');
        });
      });
    } else {
      this.alertify.error('Please populate your chat message');
    }
  }

  getDraftTracker() {
    this.draftService.getDraftTracker(this.league.id).subscribe(result => {
      this.tracker = result;
    }, error => {
      this.alertify.error('Error getting draft tracker');
    }, () => {
      // Now need to get the Previous, Current and Next picks
      this.getPicksToDisplay();
    });
  }

  getPicksToDisplay() {
    const pickleague: GetDashboardPicks = {
      pick: -1,
      leagueId: this.league.id
    };
    // Get the previous pick
    this.draftService.getDashboardPicks(pickleague).subscribe(result => {
      this.previousPick = result;
    }, error => {
      this.alertify.error('Error getting last pick');
    });

    const pickleagueTwo: GetDashboardPicks = {
      pick: 0,
      leagueId: this.league.id
    };
    // Get the Current pick
    this.draftService.getDashboardPicks(pickleagueTwo).subscribe(result => {
      this.currentPick = result;
    }, error => {
      this.alertify.error('Error getting current pick');
    });

    const pickleagueThree: GetDashboardPicks = {
      pick: 1,
      leagueId: this.league.id
    };

    // Get Next Pick
    this.draftService.getDashboardPicks(pickleagueThree).subscribe(result => {
      this.nextPick = result;
      console.log(result);
    }, error => {
      this.alertify.error('Error getting next pick');
    });
  }

  viewTeam(teamMascot: string) {
    let team: Team;

    const teamleague: GetTeamLeague = {
      teamname: teamMascot,
      leagueId: this.league.id
    };

    // Need to go a call to get the team id
    this.teamService.getTeamForTeamMascot(teamleague).subscribe(result => {
      team = result;
    }, error => {
      this.alertify.error('Error getting players team');
    }, () => {
      this.transferService.setData(team.teamId);
      this.router.navigate(['/view-team']);
    });
  }

  getYesterdaysTransactions() {
    this.leagueService.getYesterdaysTransactins(this.league.id).subscribe(result => {
      this.yesterdaysTransactions = result;
    }, error => {
      this.alertify.error('Error getting transactions');
    });
  }

  backgroundStyle() {
    switch (this.team.teamId) {
      case 2:
        // Toronto
        this.primaryColor = '206,17,65';
        this.secondaryColor = '6,25,34';
        this.secondaryTextColor = '6,25,34'
        this.secondaryLinkColor = '6,25,34'
        this.primaryTextColor = '206,17,65';
        break;
      case 3:
        // Milwaukee
        this.primaryColor = '0,71,27';
        this.secondaryColor = '240,235,210';
        this.secondaryTextColor = '240,235,210';
        this.secondaryLinkColor = '240,235,210';
        this.primaryTextColor = '0,71,27';
        break;
      case 4:
        // Miami
        this.primaryColor = '152,0,46';
        this.secondaryColor = '249,160,27';
        this.secondaryTextColor = '249,160,27';
        this.secondaryLinkColor = '249,160,27';
        this.primaryTextColor = '152,0,46';
        break;
      case 5:
        // Denver
        this.primaryColor = '13,34,64';
        this.secondaryColor = '255,198,39';
        this.secondaryTextColor = '255,198,39';
        this.secondaryLinkColor = '255,198,39';
        this.primaryTextColor = '13,34,64';
        break;
      case 6:
        // Lakers
        this.primaryColor = '85,37,130';
        this.secondaryColor = '253,185,39';
        this.secondaryTextColor = '253,185,39';
        this.secondaryLinkColor = '253,185,39';
        this.primaryTextColor = '85,37,130';
        break;
      case 7:
        // Rockets
        this.primaryColor = '206,17,65';
        this.secondaryColor = '6,25,34';
        this.secondaryTextColor = '6,25,34';
        this.secondaryLinkColor = '6,25,34';
        this.primaryTextColor = '206,17,65';
        break;
      case 8:
        // Boston
        this.primaryColor = '0, 122, 51';
        this.secondaryColor = '139,111,78';
        this.secondaryTextColor = '220, 220, 220';
        this.primaryTextColor = '139,111,78';
        this.primaryTextColor = '220, 220, 220';
        break;
      case 9:
        // Indiana
        this.primaryColor = '0,45,98';
        this.secondaryColor = '253,187,48';
        this.secondaryTextColor = '253,187,48';
        this.secondaryLinkColor = '253,187,48';
        this.primaryTextColor = '0,45,98';
        break;
      case 10:
        // Orlando
        this.primaryColor = '0,125,197';
        this.secondaryColor = '196,206,211';
        this.secondaryTextColor = '196,206,211';
        this.secondaryLinkColor = '196,206,211';
        this.primaryTextColor = '0,125,197';
        break;
      case 11:
        // OKC
        // this.primaryColor = '0,125,195';
        // this.secondaryColor = '239,59,36';
        // this.secondaryTextColor = '239,59,36';
        // this.secondaryLinkColor = '239,59,36';
        // this.primaryTextColor = '0,125,195';
        this.primaryColor = '0,101,58';
        this.secondaryColor = '255,194,32';
        this.secondaryTextColor = '255,194,32';
        this.secondaryLinkColor = '255,194,32';
        this.primaryTextColor = '0,101,58';
        break;
      case 12:
        // Clippers
        this.primaryColor = '200,16,46';
        this.secondaryColor = '29,66,148';
        this.secondaryTextColor = '29,66,148';
        this.secondaryLinkColor = '29,66,148';
        this.primaryTextColor = '200,16,46';
        break;
        break;
      case 13:
        // Dallas
        this.primaryColor = '0,83,188';
        this.secondaryColor = '0,43,92';
        this.secondaryTextColor = '220,220,220';
        this.secondaryLinkColor = '220,220,220';
        this.primaryTextColor = '220,220,220';
        break;
      case 14:
        // 76ers
        this.primaryColor = '0,107,182';
        this.secondaryColor = '237,23,76';
        this.secondaryTextColor = '237,23,76';
        this.secondaryLinkColor = '237,23,76';
        this.primaryTextColor = '0,107,182';
        break;
      case 15:
        // Chicago
        this.primaryColor = '206,17,65';
        this.secondaryColor = '6,25,34';
        this.secondaryTextColor = '220,220,220';
        this.secondaryLinkColor = '6,25,34';
        this.primaryTextColor = '220,220,220';
        break;
      case 16:
        // Charlotte
        this.primaryColor = '29,17,96';
        this.secondaryColor = '0,120,140';
        this.secondaryTextColor = '220,220,220';
        this.secondaryLinkColor = '0,120,140';
        this.primaryTextColor = '220,220,220';
        break;
      case 17:
        // Utah
        this.primaryColor = '0,43,92';
        this.secondaryColor = '0,71,27';
        this.secondaryTextColor = '220,220,220';
        this.secondaryLinkColor = '0,71,27';
        this.primaryTextColor = '220,220,220';
        break;
      case 18:
        // Phoenix
        this.primaryColor = '29,17,96';
        this.secondaryColor = '229,95,32';
        this.secondaryTextColor = '220,220,220';
        this.secondaryLinkColor = '229,95,32';
        this.primaryTextColor = '220,220,220';
        break;
      case 19:
        // Memphis
        this.primaryColor = '93,118,169';
        this.secondaryColor = '18,23,63';
        this.secondaryTextColor = '220,220,220';
        this.secondaryLinkColor = '18,23,63';
        this.primaryTextColor = '220,220,220';
        break;
      case 20:
        // Brooklyn
        this.primaryColor = '0,0,0';
        this.secondaryColor = '119,125,132';
        this.secondaryTextColor = '220,220,220';
        this.secondaryLinkColor = '119,125,132';
        this.primaryTextColor = '220,220,220';
        break;
      case 21:
        // Detroit
        this.primaryColor = '200,16,46';
        this.secondaryColor = '29,66,138';
        this.secondaryTextColor = '220,220,220';
        this.secondaryLinkColor = '29,66,138';
        this.primaryTextColor = '220,220,220';
        break;
      case 22:
        // Washington
        this.primaryColor = '0,43,92';
        this.secondaryColor = '227,24,55';
        this.secondaryTextColor = '220,220,220';
        this.secondaryLinkColor = '227,24,55';
        this.primaryTextColor = '220,220,220';
        break;
      case 23:
        // Portland
        this.primaryColor = '224,58,62';
        this.secondaryColor = '6,25,34';
        this.secondaryTextColor = '220,220,220';
        this.secondaryLinkColor = '6,25,34';
        this.primaryTextColor = '220,220,220';
        break;
      case 24:
        // Sacromento
        this.primaryColor = '91,43,130';
        this.secondaryColor = '99,113,122';
        this.secondaryTextColor = '220,220,220';
        this.secondaryLinkColor = '99,113,122';
        this.primaryTextColor = '220,220,220';
        break;
      case 25:
        // Spurs
        this.primaryColor = '6,25,34';
        this.secondaryColor = '6,25,34';
        // this.secondaryColor = '196,206,211';
        // this.secondaryTextColor = '6,25,34';
        // this.secondaryLinkColor = '6,25,34';
        // this.primaryTextColor = '6,25,34';
        // this.primaryTextColor = '6,25,34';
        // this.secondaryTextColor = '220,220,220';
        // this.secondaryLinkColor = '220,220,220';
        break;
      case 26:
        // Knicks
        this.primaryColor = '0,107,182';
        this.secondaryColor = '245,132,38';
        this.secondaryTextColor = '220,220,220';
        this.secondaryLinkColor = '245,132,38';
        this.primaryTextColor = '220,220,220';
        break;
      case 27:
        // Cavs
        this.primaryColor = '134,0,56';
        this.secondaryColor = '4,30,66';
        this.secondaryTextColor = '220,220,220';
        this.secondaryLinkColor = '4,30,66';
        this.primaryTextColor = '220,220,220';
        break;
      case 28:
        // Atlanta
        this.primaryColor = '225,68,52';
        this.secondaryColor = '196,214,0';
        this.secondaryTextColor = '196,214,0';
        this.secondaryLinkColor = '196,214,0';
        this.primaryTextColor = '225,68,52';
        break;
      case 29:
        // Minnesota
        this.primaryColor = '12,35,64';
        this.secondaryColor = '35,97,146';
        this.secondaryTextColor = '220,220,220';
        this.secondaryLinkColor = '35,97,146';
        this.primaryTextColor = '220,220,220';
        break;
      case 30:
        // GSW
        this.primaryColor = '29,66,138';
        this.secondaryColor = '255,199,44';
        this.secondaryTextColor = '220,220,220';
        this.secondaryLinkColor = '255,199,44';
        this.primaryTextColor = '29,66,138';
        break;
      case 32:
        // New Orleans
        this.primaryColor = '0,22,65';
        this.secondaryColor = '225,58,62';
        this.secondaryTextColor = '220,220,220';
        this.secondaryLinkColor = '225,58,62';
        this.primaryTextColor = '220,220,220';
        break;
      default:
        this.primaryColor = '22, 24, 100';
        this.secondaryColor = '12, 126, 120';
        break;
    }
  }

  // getSecondaryFontColor

  teamSelection() {
    if (this.teamToggle == 0) {
      this.teamToggle = 1;
    } else {
      this.teamToggle = 0;
    }
  }

  leagueSelection() {
    if (this.leagueToggle == 0) {
      this.leagueToggle = 1;
    } else {
      this.leagueToggle = 0;
    }
  }

  chatSelection() {
    if (this.chatToggle == 0) {
      this.chatToggle = 1;
    } else {
      this.chatToggle = 0;
    }
  }

  getTotalPointsAverage(detailedPlayer: QuickViewPlayer) {
    const value = (detailedPlayer.ptsStats / detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalRebAverage(detailedPlayer: QuickViewPlayer) {
    const totalRebs = detailedPlayer.orebsStats + detailedPlayer.drebsStats;
    const value = (totalRebs / detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalAstAverage(detailedPlayer: QuickViewPlayer) {
    const value = (detailedPlayer.astStats / detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

}
