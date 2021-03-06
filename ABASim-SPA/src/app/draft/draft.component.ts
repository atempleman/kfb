import { Component, OnInit, TemplateRef } from '@angular/core';
import { LeagueService } from '../_services/league.service';
import { League } from '../_models/league';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';
import { DraftService } from '../_services/draft.service';
import { InitialDraftPicks } from '../_models/initialDraftPicks';
import { TeamService } from '../_services/team.service';
import { Team } from '../_models/team';
import { DraftPick } from '../_models/draftPick';
import { AuthService } from '../_services/auth.service';
import { DraftTracker } from '../_models/draftTracker';
import { DraftPlayer } from '../_models/draftPlayer';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { PlayerService } from '../_services/player.service';
import { DraftSelection } from '../_models/draftSelection';
import { AdminService } from '../_services/admin.service';
import { TransferService } from '../_services/transfer.service';
import { InitialPickSalary } from '../_models/initialPickSalary';
import { GetPlayoffSummary } from '../_models/getPlayoffSummary';
import { LeagueStatusUpdate } from '../_models/leagueStatusUpdate';
import { GetRosterQuickView } from '../_models/getRosterQuickView';
import { DraftSelectionPlayer } from '../_models/draftSelectionPlayer';
import { RegularDraftContract } from '../_models/regularDraftContract';

@Component({
  selector: 'app-draft',
  templateUrl: './draft.component.html',
  styleUrls: ['./draft.component.css']
})
export class DraftComponent implements OnInit {
  league: League;
  public modalRef: BsModalRef;
  draftPicks: DraftPick[] = [];
  teamOnClock: Team;
  team: Team;

  initialDraftSelected = 1;
  seasonDraftSelected = 1;
  initialDraftLotterySelected = 0;
  seasonDraftLotterySelected = 0;
  initialPlayerPoolSelected = 0;
  seasonPlayerPoolSelected = 0;
  initialDraftboardSelected = 0;
  seasonDraftboardSelected = 0;

  tracker: DraftTracker;

  allDraftPicks: InitialDraftPicks[] = [];
  currentRound = 1;
  roundDraftPicks: InitialDraftPicks[] = [];
  draftablePlayers: DraftPlayer[] = [];
  draftSelectionPlayers: DraftSelectionPlayer[] = [];

  allTeams: Team[] = [];

  loaded = 0;
  isAdmin = 0;
  teamId = 0;
  draftSelection = 0;

  onClockLoaded = 0;

  timeRemaining: number;
  timeDisplay: string;
  interval;
  pageInterval;

  initialPickSalary: InitialPickSalary[] = [];
  regularPickSalary: RegularDraftContract[] = [];

  rounds: number[] = [];

  viewingRound: number;

  constructor(private leagueService: LeagueService, private alertify: AlertifyService, private router: Router,
    private draftService: DraftService, private teamService: TeamService, private authService: AuthService,
    private modalService: BsModalService, private playerService: PlayerService, private adminService: AdminService,
    private transferService: TransferService) { }

  ngOnInit() {
    this.isAdmin = +localStorage.getItem('isAdmin');
    this.teamId = +localStorage.getItem('teamId');

    this.rounds = Array(13).fill(0).map((x, i) => i);

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
      this.loaded = 1;
      if (this.league.stateId == 13) {
        this.seasonDraftSelected = 0;
        this.seasonDraftLotterySelected = 1;
      }

      if (this.league.stateId === 3) {
        this.getDraftDetails();
      } else if (this.league.stateId === 4) {
        this.getDraftTracker();
      }

      if (this.league.stateId === 3 || this.league.stateId === 5 || this.league.stateId === 4) {
        this.playerService.getAllInitialDraftSelectionPlayers(this.league.id).subscribe(result => {
          this.draftSelectionPlayers = result;
        }, error => {
          this.alertify.error('Error getting available players to draft');
        }, () => {
        });

        this.draftService.getInitialDraftSalaryDetails().subscribe(result => {
          this.initialPickSalary = result;
        }, error => {
          this.alertify.error('Error getting salary details');
        });
      } else if (this.league.stateId === 14) {
        this.playerService.getAllInitialDraftSelectionPlayers(this.league.id).subscribe(result => {
          this.draftSelectionPlayers = result;
        }, error => {
          this.alertify.error('Error getting available players to draft');
        }, () => {
        });
        
        this.draftService.getRegularSeasonDraftSalaryDetails().subscribe(result => {
          this.regularPickSalary = result;
        }, error => {
          this.alertify.error('Error getting salary details');
        });
      }

      this.pageInterval = setInterval(() => {
        this.getDraftTracker();
      }, 30000);
    });
  }

  counter(i: number) {
    return new Array(i);
  }

  getTeamOnClock() {
    const pick = this.tracker.pick;
    const round = this.tracker.round;
    const dp = this.draftPicks.find(x => x.pick === pick && x.round === round);
    if (dp) {
      return ' - ' + dp.teamName + ' are on the clock';
    } else {
      return '';
    }
  }

  timerDisplay() {
    if (this.league.stateId < 3) {

    } else {
      this.interval = setInterval(() => {
        const time = this.tracker.dateTimeOfLastPick + ' UTC';
        const dtPick = new Date(time);
        const currentTime = new Date();

        this.timeRemaining = dtPick.getTime() - currentTime.getTime();
        const value = (this.timeRemaining / 1000).toFixed(0);
        this.timeRemaining = +value;

        if (this.timeRemaining > 0) {
          this.timeRemaining--;
          this.timeDisplay = this.transform(this.timeRemaining);
        } else {
          this.timeRemaining = 0;
          this.timeDisplay = 'Time Expired';
        }
      }, 1000);
    }
  }

  transform(value: number): string {
    const minutes: number = Math.floor(value / 60);
    return minutes + ':' + (value - minutes * 60);
  }

  getDraftTracker() {
    // Get the draft tracker
    this.draftService.getDraftTracker(this.league.id).subscribe(result => {
      this.tracker = result;
    }, error => {
      this.alertify.error('Error getting draft tracker');
    }, () => {
      if (this.tracker != null) {
        this.currentRound = this.tracker.round;
      } else {
        this.currentRound = 0;
      }

      this.getDraftDetails();
      this.onClockLoaded++;
      this.timerDisplay();
    });
  }

  getDraftDetails() {
    this.viewingRound = this.currentRound;
    const summary: GetPlayoffSummary = {
      round: this.currentRound,
      leagueId: this.league.id
    };

    // Get the Initial Draft Details
    this.draftService.getDraftPicksForRound(summary).subscribe(result => {
      this.draftPicks = result;
    }, error => {
      this.alertify.error('Error getting Draft Picks');
    }, () => {
      this.onClockLoaded++;
    });
  }

  beginDraft() {
    this.draftService.beginInitialDraft(this.league.id).subscribe(result => {
    }, error => {
      this.alertify.error('Error starting the draft');
    }, () => {
      if (this.league.stateId == 14) {
        this.alertify.success('Draft has begun!');
      } else {
        this.alertify.success('Initial Draft has begun!');
        this.league.stateId = 4;
      }
    });
  }

  public openModal(template: TemplateRef<any>) {
    console.log(template);
    this.modalRef = this.modalService.show(template);
  }

  pageChange(page: number) {
    this.viewingRound = page + 1;
    // To add the paging code here
    const summary: GetPlayoffSummary = {
      round: page + 1,
      leagueId: this.league.id
    };

    if (this.league.stateId == 3 || this.league.stateId == 4 || this.league.stateId == 5) {
      // Get the Initial Draft Details
      this.draftService.getDraftPicksForRound(summary).subscribe(result => {
        this.draftPicks = result;
      }, error => {
        this.alertify.error('Error getting Draft Picks');
      }, () => {

      });
    } else if (this.league.stateId == 14) {

    }
  }

  makeDraftPick() {
    const selectedPick: DraftSelection = {
      pick: this.tracker.pick,
      playerId: +this.draftSelection,
      round: this.tracker.round,
      teamId: this.team.teamId,
      leagueId: this.league.id
    };

    this.draftService.makeDraftPick(selectedPick).subscribe(result => {
    }, error => {
      this.alertify.error('Error making pick');
    }, () => {
      this.modalRef.hide();

      this.getDraftDetails();

      if (this.tracker.round === 13 && this.tracker.pick === 30) {
        // Update the leage state here
        const summary: LeagueStatusUpdate = {
          status: 5,
          leagueId: this.league.id
        };
        this.adminService.updateLeagueStatus(summary).subscribe(result => {
        }, error => {
          this.alertify.error('Error changing league state');
        }, () => {
          this.alertify.success('Draft Completed');
        });
      } else if (this.league.stateId == 14 && this.tracker.round === 2 && this.tracker.pick === 30) {
        // Update the leage state here
        const summary: LeagueStatusUpdate = {
          status: 15,
          leagueId: this.league.id
        };
        this.adminService.updateLeagueStatus(summary).subscribe(result => {
        }, error => {
          this.alertify.error('Error changing league state');
        }, () => {
          this.alertify.success('Draft Completed');
        });
      } else {
        this.getDraftTracker();
        window.location.reload();
      }
    });
  }

  viewTeam(teamId: number) {
    let team: Team;
    const summary: GetRosterQuickView = {
      teamId: teamId,
      leagueId: this.league.id
    };

    // Need to go a call to get the team id
    this.teamService.getTeamForTeamId(summary).subscribe(result => {
      team = result;
    }, error => {
      this.alertify.error('Error getting players team');
    }, () => {
      this.transferService.setData(team.teamId);
      this.router.navigate(['/view-team']);
    });
  }

  autoPickAction() {
    const selectedPick: DraftSelection = {
      pick: this.tracker.pick,
      playerId: 0,
      round: this.tracker.round,
      teamId: 0,
      leagueId: this.league.id
    };

    this.draftService.makeAutoPick(selectedPick).subscribe(result => {
    }, error => {
      this.alertify.error('Error making pick');
    }, () => {
      this.getDraftDetails();

      if (this.tracker.round === 13 && this.tracker.pick === 30) {
        // Update the leage state here
        const summary: LeagueStatusUpdate = {
          status: 5,
          leagueId: this.league.id
        };
        this.adminService.updateLeagueStatus(summary).subscribe(result => {
        }, error => {
          this.alertify.error('Error changing league state');
        }, () => {
          this.alertify.success('Draft Completed');
        });
      } else if (this.league.stateId == 14 && this.tracker.round === 2 && this.tracker.pick === 30) {
        // Update the leage state here
        const summary: LeagueStatusUpdate = {
          status: 15,
          leagueId: this.league.id
        };
        this.adminService.updateLeagueStatus(summary).subscribe(result => {
        }, error => {
          this.alertify.error('Error changing league state');
        }, () => {
          this.alertify.success('Draft Completed');
        });
      } else {
        this.getDraftTracker();
        window.location.reload();
      }
    });
  }

  playerPoolClicked() {
    this.router.navigate(['/draftplayerpool']);
  }

  rankingsClicked() {
    this.router.navigate(['/draftboard']);
  }

  lotteryClicked() {
    this.router.navigate(['/initiallottery']);
  }

  getPickSalary(round: number, pick: number) {
    const value = this.initialPickSalary.find(x => x.round === round && x.pick === pick);
    return value.salary;
  }

  getRegularSalary(round: number, pick: number) {
    const value = this.regularPickSalary.find(x => x.round === round && x.pick === pick);
    return value.salaryAmount;
  }

  getSalaryYears(round: number, pick: number) {
    const value = this.initialPickSalary.find(x => x.round === round && x.pick === pick);
    return value.years;
  }

  intialDraftSelection() {
    this.initialDraftLotterySelected = 0;
    this.initialDraftboardSelected = 0;
    this.initialPlayerPoolSelected = 0;
    this.initialDraftSelected = 1;
  }

  seasonDraftSelection() {
    this.seasonDraftLotterySelected = 0;
    this.seasonDraftboardSelected = 0;
    this.seasonPlayerPoolSelected = 0;
    this.seasonDraftSelected = 1;
  }

  intialDraftLotterySelection() {
    this.initialDraftboardSelected = 0;
    this.initialPlayerPoolSelected = 0;
    this.initialDraftSelected = 0;
    this.initialDraftLotterySelected = 1;
  }

  seasonDraftLotterySelection() {
    this.seasonDraftboardSelected = 0;
    this.seasonPlayerPoolSelected = 0;
    this.seasonDraftSelected = 0;
    this.seasonDraftLotterySelected = 1;
  }

  initialPlayerPoolSelection() {
    this.initialDraftboardSelected = 0;
    this.initialDraftSelected = 0;
    this.initialDraftLotterySelected = 0;
    this.initialPlayerPoolSelected = 1;
  }

  seasonPlayerPoolSelection() {
    this.seasonDraftboardSelected = 0;
    this.seasonDraftSelected = 0;
    this.seasonDraftLotterySelected = 0;
    this.seasonPlayerPoolSelected = 1;
  }

  initialDraftboardSelection() {
    this.initialDraftSelected = 0;
    this.initialDraftLotterySelected = 0;
    this.initialPlayerPoolSelected = 0;
    this.initialDraftboardSelected = 1;
  }

  seasonDraftboardSelection() {
    this.seasonDraftSelected = 0;
    this.seasonDraftLotterySelected = 0;
    this.seasonPlayerPoolSelected = 0;
    this.seasonDraftboardSelected = 1;
  }
}
