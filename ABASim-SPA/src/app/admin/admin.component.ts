import { Component, OnInit, TemplateRef } from '@angular/core';
import { Router } from '@angular/router';
import { League } from '../_models/league';
import { AuthService } from '../_services/auth.service';
import { LeagueService } from '../_services/league.service';
import { AlertifyService } from '../_services/alertify.service';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { LeagueState } from '../_models/leagueState';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AdminService } from '../_services/admin.service';
import { Team } from '../_models/team';
import { TeamService } from '../_services/team.service';
import { CheckGame } from '../_models/checkGame';
import { GameDisplayCurrent } from '../_models/gameDisplayCurrent';
import { NgxSpinnerService } from 'ngx-spinner';
import { DraftService } from '../_services/draft.service';
import { LeagueStatusUpdate } from '../_models/leagueStatusUpdate';
import { GetRosterQuickView } from '../_models/getRosterQuickView';
import { GetScheduleLeague } from '../_models/getScheduleLeague';
import { GetGameLeague } from '../_models/getGameLeague';
import { RolloverStatus } from '../_models/rolloverStatus';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {
  league: League;
  public modalRef: BsModalRef;

  masterActionAccordion = 0;
  nextActionAccordion = 1;
  otherAccordion = 0;

  statusSelection: number;
  currentStatus = '';
  leagueStates: LeagueState[] = [];
  gamesAllRun = 0;
  rolloverResult = false;

  teams: Team[] = [];
  teamSelected: number;

  rolloverStatus: RolloverStatus;
  rolloverPerc = 0;
  rolloverButton = 0;

  run = 0;
  progress = 0;

  dayEntered = 0;
  dayForm: FormGroup;
  newLeagueForm: FormGroup;
  todaysGames: GameDisplayCurrent[] = [];

  username = 0;
  team: Team;

  userId = 0;

  leaguename = '';
  seasonid = '';
  leaguecode = '';
  newLeagueCreate = 0;

  constructor(private router: Router, private leagueService: LeagueService, private alertify: AlertifyService,
              private authService: AuthService, private modalService: BsModalService, private adminService: AdminService,
              private teamService: TeamService, private fb: FormBuilder, private spinner: NgxSpinnerService,
              private draftService: DraftService) { }

  ngOnInit() {
    this.userId = this.authService.decodedToken.nameid;
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
      this.setupPage();
    });
  }

  setupPage() {
    if (this.league.stateId >= 6)
    {
      this.getTodaysGames();
    } else {
      
    }
    
    this.username = +this.authService.decodedToken.nameid;
  }

  getTodaysGames() {
    this.adminService.getGamesForReset(this.league.id).subscribe(result => {
      this.todaysGames = result;
    }, error => {
      this.alertify.error('Error getting todays games');
    });
  }

  public openConfirm(template: TemplateRef<any>) {
    this.modalRef = this.modalService.show(template);
  }

  // State 2
  moveToInitialLottery() {
    const summary: LeagueStatusUpdate = {
      status: 2,
      leagueId: this.league.id
    };
    this.adminService.updateLeagueStatus(summary).subscribe(result => {
    }, error => {
      this.alertify.error('Error saving league status');
    }, () => {
      this.alertify.success('League Status updated.');
      this.modalRef.hide();
      this.league.stateId = this.statusSelection;
    });
  }

  // Running lottery and changing to state 3
  runInitialDraftLottery() {
    this.adminService.runInitialDraftLottery(this.league.id).subscribe(result => {
    }, error => {
      this.alertify.error('Error running initial draft lottery');
    }, () => {
      this.alertify.success('Initial Draft Lottery Run successfully');
      this.league.stateId = 3;
      this.modalRef.hide();
    });
  }

  // Begin the draft
  beginDraft() {
    this.draftService.beginInitialDraft(this.league.id).subscribe(result => {
    }, error => {
      this.alertify.error('Error starting the draft');
    }, () => {
      this.alertify.success('Initial Draft has begun!');
      this.league.stateId = 4;
      this.modalRef.hide();
    });
  }

  // Begin the preseason
  beginPreseason() {
    const summary: LeagueStatusUpdate = {
      status: 6,
      leagueId: this.league.id
    };
    this.adminService.updateLeagueStatus(summary).subscribe(result => {
    }, error => {
      this.alertify.error('Error saving league status');
    }, () => {
      this.alertify.success('League Status updated.');
      this.modalRef.hide();
      this.league.stateId = 6;
    });
  }

  beginSeason() {
    const summary: LeagueStatusUpdate = {
      status: 7,
      leagueId: this.league.id
    };
    this.adminService.updateLeagueStatus(summary).subscribe(result => {
    }, error => {
      this.alertify.error('Error saving league status');
    }, () => {
      this.alertify.success('League Status updated.');
      this.modalRef.hide();
      this.league.stateId = 7;
    });
  }


  //#region Super Admin Functions

  resetLeague() {
    this.adminService.resetLeague(this.league.id).subscribe(result => {
    }, error => {
      this.alertify.error('Error resetting the league.');
    }, () => {
      this.league.stateId = 1;
      this.modalRef.hide();
      this.alertify.success('League has been completely reset');
    });
  }

  //#endregion




  runEngine() {
    this.router.navigate(['/admintestengine']);
  }

  // League Status
  public openModal(template: TemplateRef<any>, selection: number) {
    if (selection === 0) {
      // League status
      this.getLeagueStatusData();
    } else if (selection === 1) {
      // Remove team rego
      this.getRemoveTeamRegoData();
    } else if (selection === 2) {
      this.rollOverDay();
    } else if (selection === 3) {
      this.dayForm = this.fb.group({
        updateDay: ['', Validators.required]
      });
    }
    this.modalRef = this.modalService.show(template);
  }

  beginPlayoffs() {
    this.adminService.beginPlayoffs(this.league.id).subscribe(result => {
    }, error => {
      this.alertify.error('Error beginning the playoffs');
    }, () => {
      this.alertify.success('Playoffs have been setup');
      this.league.stateId = 8;
      this.league.day = 0;
      this.modalRef.hide();
    });
  }

  beginConfSemis() {
    this.adminService.beginConfSemis(this.league.id).subscribe(result => {
    }, error => {
      this.alertify.error('Error beginning the playoffs');
    }, () => {
      this.alertify.success('Playoffs have been setup');
      this.league.stateId = 9;
      this.league.day = 8;
      this.modalRef.hide();
    });
  }

  beginConfFinals() {
    this.adminService.beginConfFinals(this.league.id).subscribe(result => {
    }, error => {
      this.alertify.error('Error beginning the playoffs');
    }, () => {
      this.alertify.success('Playoffs have been setup');
      this.league.stateId = 10;
      this.league.day = 15;
      this.modalRef.hide();
    });
  }

  beginFinals() {
    this.adminService.beginFinals(this.league.id).subscribe(result => {
    }, error => {
      this.alertify.error('Error beginning the playoffs');
    }, () => {
      this.alertify.success('Playoffs have been setup');
      this.league.stateId = 11;
      this.league.day = 22;
      this.modalRef.hide();
    });
  }

  getLeagueStatusData() {
    this.leagueService.getLeague(this.league.id).subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting league');
    });

    this.leagueService.getLeagueStatuses().subscribe(result => {
      this.leagueStates = result;
    }, error => {
      this.alertify.error('Error getting league statuses');
    }, () => {
      this.currentStatus = this.leagueStates.find(x => x.id === this.league.stateId).state;
    });
  }

  updateLeagueStatus() {
    const summary: LeagueStatusUpdate = {
      status: this.statusSelection,
      leagueId: this.league.id
    };
    this.adminService.updateLeagueStatus(summary).subscribe(result => {
    }, error => {
      this.alertify.error('Error saving league status');
    }, () => {
      this.alertify.success('League Status updated.');
      this.modalRef.hide();
      this.league.stateId = this.statusSelection;
      if (this.statusSelection === 7) {
        this.league.day = 0;
      }

      this.league.state = this.getLeagueStateForId(this.statusSelection);
    });
  }

  getLeagueStateForId(id: number) {
    const state = this.leagueStates.find(x => x.id === +id);
    return state.state;
  }

  getRemoveTeamRegoData() {
    this.teamService.getAllTeams(this.league.id).subscribe(result => {
      this.teams = result;
    }, error => {
      this.alertify.error('Error getting all teams');
    });
  }

  removeTeamRegistration() {
    const summary: GetRosterQuickView = {
      teamId: this.teamSelected,
      leagueId: this.league.id
    };
    this.adminService.removeTeamRegistration(summary).subscribe(result => {
    }, error => {
      this.alertify.error('Error updating team registration');
    }, () => {
      this.alertify.success('Team Rego updated.');
      this.modalRef.hide();
    });
  }

  rollOverDay() {
    // tslint:disable-next-line: prefer-const
    let value = false;
    this.adminService.checkAllGamesRun(this.league.id).subscribe(result => {
      value = result;
    }, error => {
      this.alertify.error('Error checking if games are run');
    }, () => {
      if (value) {
        // Now run the roll over process
        this.alertify.success('Games are run');
        this.gamesAllRun = 1;
      } else {
        this.alertify.error('Not all games are run');
      }
    });
  }

  confirmRollOverDay() {
    this.run = 1;
    this.adminService.rolloverDay(this.league.id).subscribe(result => {
      this.rolloverResult = result;
    }, error => {
      this.alertify.error('Error rolling over day');
    }, () => {
      if (this.rolloverResult) {
        this.alertify.success('Day Rolled over successfully');
        this.modalRef.hide();
        this.run = 0;
        this.gamesAllRun = 0;
        this.league.day = this.league.day + 1;
      } else {
        this.alertify.error('Error rolling over day');
      }
    });
  }

  changeDay() {
    this.dayEntered = this.dayForm.controls.updateDay.value;
    let value = false;

    // Now need to call service to perform the change
    const summary: GetScheduleLeague = {
      day: this.dayEntered,
      leagueId: this.league.id
    };
    this.adminService.changeDay(summary).subscribe(result => {
      value = result;
    }, error => {
      this.alertify.error('Error changing the current day');
    }, () => {
      if (value) {
        this.alertify.success('Day has been changed');
        this.modalRef.hide();
        this.league.day = this.dayEntered;
      } else {
        this.alertify.error('Error changing the current day');
      }
    });
  }

  runDraftPicks() {
    this.adminService.runTeamDraftPicksSetup(this.league.id).subscribe(result => {
    }, error => {
      this.alertify.error('Error running team pick setup');
    }, () => {
      this.alertify.success('Team pick setup run successfully');
      this.modalRef.hide();
    });
  }

  generateControcts() {
    this.adminService.generateInitalContracts(this.league.id).subscribe(result => {

    }, error => {
      this.alertify.error('Error generating contracts');
    }, () => {
      this.alertify.success('Contracts generated');
      this.modalRef.hide();
    });
  }

  generateSalaryCaps() {
    this.adminService.generateInitialSalaryCaps(this.league.id).subscribe(result => {

    }, error => {
      this.alertify.error('Error generating salary caps');
    }, () => {
      this.alertify.success('Salary caps updated');
      this.modalRef.hide();
    });
  }

  generateAutoPick() {
    this.adminService.generateAutoPicks(this.league.id).subscribe(result => {

    }, error => {
      this.alertify.error('Error generating auto picks');
    }, () => {
      this.alertify.success('Autopicks generated');
      this.modalRef.hide();
    });
  }

  resetGame(gameId: number) {
    const summary: GetGameLeague = {
      gameId: gameId,
      leagueId: this.league.id
    };
    this.adminService.resetGame(summary).subscribe(result => {

    }, error => {
      this.alertify.error('Error resetting game');
    }, () => {
      this.alertify.success('Game has been reset');
      this.getTodaysGames();
      this.modalRef.hide();

    });
  }

  rolloverToNextSeason() {
    this.spinner.show();

    this.adminService.rolloverSeasonStats(this.league.id).subscribe(result => {
    }, error => {
      this.alertify.error('Error rolling over season stats');
    }, () => {
      this.progress = 10;
      this.rolloverAwardWinners();
    });
  }

  rolloverAwardWinners() {
    this.adminService.rolloverAwardWinners(this.league.id).subscribe(result => {
    }, error => {
      this.alertify.error('Error rolling over historical records and award winners');
    }, () => {
      this.progress = 20;
      this.rolloverContractUpdates();
    });
  }

  rolloverContractUpdates() {
    this.adminService.rolloverContractUpdates(this.league.id).subscribe(result => {
    }, error => {
      this.alertify.error('Error rolling over contracts');
    }, () => {
      this.progress = 30;
      this.generateDraft();
    });
  }

  generateDraft() {
    this.adminService.generateDraft(this.league.id).subscribe(result => {
    }, error => {
      this.alertify.error('Error setting up draft');
    }, () => {
      this.progress = 40;
      this.deleteData();
    });
  }

  deleteData() {
    this.adminService.deletePreseasonAndPlayoffsData(this.league.id).subscribe(result => {
    }, error => {
      this.alertify.error('Error deleting preason and playoff data');
    }, () => {
      this.progress = 50;
      this.adminService.deleteAwardsData(this.league.id).subscribe(result => {

      }, error => {
        this.alertify.error('Error saving stats and deleting awards');
      }, () => {
        this.progress = 60;
        this.adminService.deleteOtherData(this.league.id).subscribe(result => {
        }, error => {
          this.alertify.error('Error deleting other data');
        }, () => {
          this.progress = 70;
          this.adminService.deleteTeamSettingsData(this.league.id).subscribe(result => {
          }, error => {
            this.alertify.error('Error deleting team settings data');
          }, () => {
            this.progress = 75;
            this.adminService.deleteSeasonData(this.league.id).subscribe(result => {
              this.alertify.error('Error deleting season data');
            }, () => {
              this.progress = 80;
              this.resetStandings();
            });
          });
        });
      });
    });
  }

  resetStandings() {
    this.adminService.resetStandings(this.league.id).subscribe(result => {
    }, error => {
      this.alertify.error('Error resetting standings');
    }, () => {
      this.progress = 90;
      this.rolloverLeague();
    });
  }

  endSeason() {
    this.adminService.endSeason(this.league.id).subscribe(result => {
    }, error => {
      this.alertify.error('Error ending season properly');
    }, () => {
      this.rolloverToNextSeason();
    });
  }

  rolloverLeague() {
    this.adminService.rolloverLeague(this.league.id).subscribe(result => {
    }, error => {
      this.alertify.error('Error rolling over league');
    }, () => {
      this.modalRef.hide();
      this.spinner.hide();
    });
  }

  createNewLeague() {
    this.newLeagueCreate = 1;

    this.spinner.show();

    let newLeague: League = 
    {
      id: 0,
      stateId: 0,
      day: 0,
      state: '',
      year: +this.seasonid,
      leagueName: this.leaguename,
      leagueCode: this.leaguecode
    };

    this.adminService.createNewLeague(this.league).subscribe(result => {

    }, error => {
      this.alertify.error('Error creating new league');
      this.newLeagueCreate = 0;
    }, () => {
      this.alertify.success('New league has successfully been created');
      this.modalRef.hide();
      this.spinner.hide();
      this.newLeagueCreate = 0;
    });
  }

  toggleNextAction() {
    if (this.nextActionAccordion == 0) {
      this.nextActionAccordion = 1;
    } else {
      this.nextActionAccordion = 0;
    }
  }

  toogleMasterAction() {
    if (this.masterActionAccordion == 0) {
      this.newLeagueForm = this.fb.group({
        leaguename: ['', Validators.required],
        seasonid: ['', Validators.required],
        leaguecode: ['', Validators.required]        
      });

      this.masterActionAccordion = 1;
    } else {
      this.masterActionAccordion = 0;
    }
  }

  toggleOtherAction() {
    if (this.otherAccordion == 0) {
      this.otherAccordion = 1;
    } else {
      this.otherAccordion = 0;
    }
  }

  /* The Rollover code begins here */

  endOfSeasonRollOver() {
    this.rolloverButton = 1
      this.spinner.show();
      this.adminService.getLeagueRolloverStatus(this.league.id).subscribe(result => {
        this.rolloverStatus = result;
      }, error => {
        this.alertify.error('Error getting league rollover status');
        this.spinner.hide();
        this.rolloverButton = 0;
      }, () => {
        this.rolloverPerc = 1;
        this.alertify.success('Completed Getting Season Rollover - ' + this.rolloverPerc + '% completed');
        this.rolloverSaveSeasonResults();
      });
  }

  rolloverSaveSeasonResults() {
    // Save season results
    let saveResult = false;
    // Champion, Runners Up, Award Winner, Standings, Player Stats(season & playoffs), PlayoffSeries
    this.adminService.saveSeasonResults(this.league.id).subscribe(result => {
      saveResult = result;
    }, error => {
      this.spinner.hide();
      this.alertify.error('Error saving season results');
      this.rolloverButton = 0;
    }, () => {
      this.rolloverPerc = 10;
      this.alertify.success('Completed Saving Season Results - ' + this.rolloverPerc + '% completed');
      this.rolloverResetStandings();
    });    
  }

  rolloverResetStandings() {
    this.adminService.resetStandings(this.league.id).subscribe(result => {

    }, error => {
      this.spinner.hide();
      this.alertify.error('Error resetting standings');
      this.rolloverButton = 0;
    }, () => {
      this.rolloverPerc = 12;
      this.alertify.success('Completed Resetting standings - ' + this.rolloverPerc + '% completed');
      this.rolloverDeletePlayerStats();
    })
  }

  rolloverDeletePlayerStats() {
    this.adminService.rolloverDeletePlayerData(this.league.id).subscribe(result => {

    }, error => {
      this.spinner.hide();
      this.alertify.error('Error deleting player data');
      this.rolloverButton = 0;
    }, () => {
      this.rolloverPerc = 15;
      this.alertify.success('Completed Deleting Player Data - ' + this.rolloverPerc + '% completed');
      this.rolloverDeleteSeasonPBP();
    })
  }

  rolloverDeleteSeasonPBP() {
    this.adminService.rolloverDeletePlayByPlaySeason(this.league.id).subscribe(result => {

    }, error => {
      this.spinner.hide();
      this.alertify.error('Error deleing season play by play');
      this.rolloverButton = 0;
    }, () => {
      this.rolloverPerc = 25;
      this.alertify.success('Completed Deleting Season Play by Play - ' + this.rolloverPerc + '% completed');
      this.rolloverDeletePlayoffPBP();
    });
  }

  rolloverDeletePlayoffPBP() {
    this.adminService.rolloverDeletePlayByPlayPlayoffs(this.league.id).subscribe(result => {

    }, error => {
      this.spinner.hide();
      this.alertify.error('Error deleing playoffs play by play');
      this.rolloverButton = 0;
    }, () => {
      this.rolloverPerc = 30;
      this.alertify.success('Completed Deleting Playoffs Play by Play - ' + this.rolloverPerc + '% completed');
      this.rolloverDeletePlayoffSerieses();
    });
  }

  rolloverDeletePlayoffSerieses() {
    this.adminService.rolloverDeletePlayoffSerieses(this.league.id).subscribe(result => {

    }, error => {
      this.spinner.hide();
      this.alertify.error('Error deleing playoff serieses');
      this.rolloverButton = 0;
    }, () => {
      this.rolloverPerc = 32;
      this.alertify.success('Completed Deleting Playoff Series - ' + this.rolloverPerc + '% completed');
      this.rolloverDeleteGameResults();
    });
  }

  rolloverDeleteGameResults() {
    this.adminService.rolloverDeleteGameResults(this.league.id).subscribe(result => {

    }, error => {
      this.spinner.hide();
      this.alertify.error('Error deleing game results');
      this.rolloverButton = 0;
    }, () => {
      this.rolloverPerc = 37;
      this.alertify.success('Completed Deleting Game Results - ' + this.rolloverPerc + '% completed');
      this.rolloverDeleteBoxScores();
    });
  }

  rolloverDeleteBoxScores() {
    this.adminService.rolloverDeleteBoxScores(this.league.id).subscribe(result => {

    }, error => {
      this.spinner.hide();
      this.alertify.error('Error deleing box scores');
      this.rolloverButton = 0;
    }, () => {
      this.rolloverPerc = 40;
      this.alertify.success('Completed Deleting Box Scores - ' + this.rolloverPerc + '% completed');
      this.rolloverDeleteAwards();
    });
  }

  rolloverDeleteAwards() {
    this.adminService.rolloverDeleteAwards(this.league.id).subscribe(result => {

    }, error => {
      this.spinner.hide();
      this.alertify.error('Error deleing awards');
      this.rolloverButton = 0;
    }, () => {
      this.rolloverPerc = 45;
      this.alertify.success('Completed Deleting Awards - ' + this.rolloverPerc + '% completed');
      this.rolloverNextAdditionalSeasonDraftPicks();
    });
  }

  rolloverNextAdditionalSeasonDraftPicks() {
    this.adminService.rolloverLoadNextDraftPicks(this.league.id).subscribe(result => {

    }, error => {
      this.spinner.hide();
      this.alertify.error('Error loading next draft picks');
      this.rolloverButton = 0;
    }, () => {
      this.rolloverPerc = 50;
      this.alertify.success('Completed Loading Next Draft Picks - ' + this.rolloverPerc + '% completed');
      this.rolloverDeleteTeamSettings();
    });
  }

  rolloverDeleteTeamSettings() {
    this.adminService.rolloverDeleteTeamSettings(this.league.id).subscribe(result => {

    }, error => {
      this.spinner.hide();
      this.alertify.error('Error deleting team settings');
      this.rolloverButton = 0;
    }, () => {
      this.rolloverPerc = 60;
      this.alertify.success('Completed Deleting Team Settings - ' + this.rolloverPerc + '% completed');
      this.rolloverDeleteMessagesAndOffers();
    });
  }

  rolloverDeleteMessagesAndOffers() {
    this.adminService.rolloverDeleteMessagesAndOffers(this.league.id).subscribe(result => {

    }, error => {
      this.spinner.hide();
      this.alertify.error('Error deleting messages and offers');
      this.rolloverButton = 0;
    }, () => {
      this.rolloverPerc = 62;
      this.alertify.success('Completed Deleting messages and offers - ' + this.rolloverPerc + '% completed');
      this.rolloverRetiredPlayers();
    });
  }

  rolloverRetiredPlayers() {
    this.adminService.rolloverRetiredPlayers(this.league.id).subscribe(result => {

    }, error => {
      this.spinner.hide();
      this.alertify.error('Error with retired player process');
      this.rolloverButton = 0;
    }, () => {
      this.rolloverPerc = 62;
      this.alertify.success('Completed Retired Player Process - ' + this.rolloverPerc + '% completed');
      this.rolloverUpdateContractsAndPlayers();
    });
  }

  rolloverUpdateContractsAndPlayers() {
    this.adminService.rolloverUpdateContractsAndPlayers(this.league.id).subscribe(result => {

    }, error => {
      this.spinner.hide();
      this.alertify.error('Error updating players and contracts');
      this.rolloverButton = 0;
    }, () => {
      this.rolloverPerc = 67;
      this.alertify.success('Completed Updating Players and contracts - ' + this.rolloverPerc + '% completed');
      this.rolloverDeletePlayerData();
    });
  }

  rolloverDeletePlayerData() {
    this.adminService.rolloverDeletePlayerDetailsData(this.league.id).subscribe(result => {

    }, error => {
      this.spinner.hide();
      this.alertify.error('Error deleting player details');
      this.rolloverButton = 0;
    }, () => {
      this.rolloverPerc = 75;
      this.alertify.success('Completed Deleting Player Details - ' + this.rolloverPerc + '% completed');
      this.rolloverUpdateSeasonId();
    });
  }

  rolloverUpdateSeasonId() {
    this.adminService.rolloverUpdateSeason(this.league.id).subscribe(result => {

    }, error => {
      this.spinner.hide();
      this.alertify.error('Error updating league');
      this.rolloverButton = 0;
    }, () => {
      this.rolloverPerc = 80;
      this.alertify.success('Completed Updating League - ' + this.rolloverPerc + '% completed');
       this.rolloverAddPlayerData();
    });
  }

  rolloverAddPlayerData() {
    this.adminService.rolloverAddPlayerData(this.league.id).subscribe(result => {

    }, error => {
      this.spinner.hide();
      this.alertify.error('Error updating new player data');
      this.rolloverButton = 0;
    }, () => {
      this.rolloverPerc = 90;
      this.alertify.success('Completed Updating New Player Data - ' + this.rolloverPerc + '% completed');
      this.rolloverSetPlayerTeams();
    });
  }

  rolloverSetPlayerTeams() {
    this.adminService.rolloverSetPlayerTeams(this.league.id).subscribe(result => {

    }, error => {
      this.spinner.hide();
      this.alertify.error('Error updating player teams');
      this.rolloverButton = 0;
    }, () => {
      this.rolloverPerc = 95;
      this.alertify.success('Completed Updating Player Teams - ' + this.rolloverPerc + '% completed');
      this.rolloverFinishUp();
    });
  }

  rolloverFinishUp() {
    this.adminService.rolloverFinishUp(this.league.id).subscribe(result => {

    }, error => {
      this.spinner.hide();
      this.alertify.error('Error finishing rollover');
      this.rolloverButton = 0;
    }, () => {
      this.rolloverPerc = 100;
      this.alertify.success('Completed Season Rollover - ' + this.rolloverPerc + '% completed');
      this.spinner.hide();
    });
  }
}
