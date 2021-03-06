import { Component, OnInit } from '@angular/core';
import { AlertifyService } from '../_services/alertify.service';
import { PlayerService } from '../_services/player.service';
import { DraftPlayer } from '../_models/draftPlayer';
import { TeamService } from '../_services/team.service';
import { AuthService } from '../_services/auth.service';
import { Team } from '../_models/team';
import { DraftService } from '../_services/draft.service';
import { AddDraftRank } from '../_models/addDraftRank';
import { Router } from '@angular/router';
import { TransferService } from '../_services/transfer.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { FormBuilder, FormGroup } from '@angular/forms';
import { LeagueService } from '../_services/league.service';
import { League } from '../_models/league';
import { GetRosterQuickView } from '../_models/getRosterQuickView';
import { GetPlayoffSummary } from '../_models/getPlayoffSummary';
import { GetPlayerIdLeague } from '../_models/getPlayerIdLeague';
import { GetPlayerLeague } from '../_models/getPlayerLeague';

@Component({
  selector: 'app-draft-player-pool',
  templateUrl: './draft-player-pool.component.html',
  styleUrls: ['./draft-player-pool.component.css']
})
export class DraftPlayerPoolComponent implements OnInit {
  draftPlayers: DraftPlayer[] = [];
  masterDraftPlayers: DraftPlayer[] = [];
  draftboardPlayers: DraftPlayer[] = [];
  pageOfPlayers: DraftPlayer[] = [];
  masterList: DraftPlayer[] = [];
  team: Team;
  pages = 1;
  pager = 1;
  recordTotal = 0;
  searchForm: FormGroup;
  positionFilter = 0;
  displayPaging = 0;

  league: League;

  constructor(private alertify: AlertifyService, private playerService: PlayerService, private teamService: TeamService,
    private authService: AuthService, private draftService: DraftService, private router: Router,
    private transferService: TransferService, private spinner: NgxSpinnerService, private fb: FormBuilder,
    private leagueService: LeagueService) { }

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

    this.searchForm = this.fb.group({
      filter: ['']
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
    this.getDraftboardPlayers();
    this.getCountOfAvailablePlayers();
  }

  getCountOfAvailablePlayers() {
    this.playerService.getCountOfAvailableDraftPlayers(this.league.id).subscribe(result => {
      this.recordTotal = result;
    }, error => {
      this.alertify.error('Error getting count of available players');
    }, () => {
      this.pages = +(this.recordTotal / 50).toFixed(0) + 1;
    });
  }

  getDraftboardPlayers() {
    // Need to get the draftboard players
    const summary: GetRosterQuickView = {
      teamId: this.team.teamId,
      leagueId: this.league.id
    };
    this.draftService.getDraftBoardForTeam(summary).subscribe(result => {
      this.draftboardPlayers = result;
    }, error => {
      this.alertify.error('Error getting draftboard');
    }, () => {
      this.getDraftPlayers();
    });
  }

  getDraftPlayers() {
    // Get all draft players
    // const summary: GetPlayoffSummary = {
    //   round: this.pager,
    //   leagueId: this.league.id
    // };
    // this.playerService.getInitialDraftPlayers(summary).subscribe(result => {
    this.playerService.getAllInitialDraftPlayers(this.league.id).subscribe(result => {
      this.draftPlayers = result;
      this.masterList = result;
      // this.setPageArray();
    }, error => {
      this.alertify.error('Error getting players available for the draft');
      this.spinner.hide();
    }, () => {
      this.spinner.hide();
    });
  }

  counter(i: number) {
    return new Array(i);
  }

  viewPlayer(player: number) {
    this.transferService.setData(player);
    this.router.navigate(['/view-player']);
  }

  checkPlayer(playerId: number) {
    const db = this.draftboardPlayers.find(x => x.playerId === playerId);
    if (db) {
      return 1;
    } else {
      return 0;
    }
  }

  pagerNext() {
    this.spinner.show();
    this.pager = this.pager + 1;
    if (this.pager > this.pages) {
      this.pager = this.pager - 1;
    }

    const summary: GetPlayoffSummary = {
      round: this.pager,
      leagueId: this.league.id
    };
    this.playerService.getInitialDraftPlayers(summary).subscribe(result => {
      this.draftPlayers = result;
      this.masterList = result;
    }, error => {
      this.alertify.error('Error getting players available for the draft');
      this.spinner.hide();
    }, () => {
      this.spinner.hide();
    });
  }

  pagerPrev() {
    this.spinner.show();

    this.pager = this.pager - 1;
    if (this.pager < 1) {
      this.pager = this.pager + 1;
    }

    const summary: GetPlayoffSummary = {
      round: this.pager,
      leagueId: this.league.id
    };

    this.playerService.getInitialDraftPlayers(summary).subscribe(result => {
      this.draftPlayers = result;
      this.masterList = result;
    }, error => {
      this.alertify.error('Error getting players available for the draft');
      this.spinner.hide();
    }, () => {
      this.spinner.hide();
    });
  }

  goToPage(page: number) {
    this.spinner.show();

    this.pager = page;
    const summary: GetPlayoffSummary = {
      round: this.pager,
      leagueId: this.league.id
    };

    this.playerService.getInitialDraftPlayers(summary).subscribe(result => {
      this.draftPlayers = result;
      this.masterList = result;
    }, error => {
      this.alertify.error('Error getting players available for the draft');
      this.spinner.hide();
    }, () => {
      this.spinner.hide();
    });
  }

  addPlayerToDraftRank(selectedPlayer: DraftPlayer) {
    const newRanking = {} as AddDraftRank;
    newRanking.playerId = selectedPlayer.playerId;
    newRanking.teamId = this.team.teamId;
    newRanking.leagueId = this.league.id;

    this.draftService.addDraftPlayerRanking(newRanking).subscribe(result => {
    }, error => {
      this.alertify.error('Error adding Player to Draft Board');
    }, () => {
      this.alertify.success('Player added to Draft Board.');
      const record = this.draftPlayers.find(x => x.playerId === selectedPlayer.playerId);
      this.draftboardPlayers.push(record);
    });
  }

  removePlayerDraftRank(selectedPlayer: DraftPlayer) {
    const newRanking = {} as AddDraftRank;
    newRanking.playerId = selectedPlayer.playerId;
    newRanking.teamId = this.team.teamId;
    newRanking.leagueId = this.league.id;

    this.draftService.removeDraftPlayerRanking(newRanking).subscribe(result => {

    }, error => {
      this.alertify.error('Error removing Player from Draft board');
    }, () => {
      const index = this.draftboardPlayers.findIndex(x => x.playerId === selectedPlayer.playerId);
      this.draftboardPlayers.splice(index, 1);
    });
  }

  draftHQClicked() {
    this.router.navigate(['/draft']);
  }

  rankingsClicked() {
    this.router.navigate(['/draftboard']);
  }

  lotteryClicked() {
    this.router.navigate(['/initiallottery']);
  }

  filterByPos(pos: number) {
    this.spinner.show();
    this.positionFilter = pos;

    if (pos === 0) {
      this.displayPaging = 0;

      this.getDraftPlayers();
    } else {
      this.displayPaging = 1;

      // Now we need to update the listing appropriately
      const summary: GetPlayerIdLeague = {
        playerId: this.positionFilter,
        leagueId: this.league.id
      };
      this.playerService.getDraftPlayerPoolByPos(summary).subscribe(result => {
        this.draftPlayers = result;
      }, error => {
        this.alertify.error('Error getting filtered players');
        this.spinner.hide();
      }, () => {
        this.spinner.hide();
      });
    }
  }

  filterTable() {
    this.spinner.show();
    this.displayPaging = 1;
    const filter = this.searchForm.value.filter;

    // Need to call service
    const summary: GetPlayerLeague = {
      playerName: filter,
      leagueId: this.league.id
    };
    this.playerService.filterDraftPlayerPool(summary).subscribe(result => {
      this.draftPlayers = result;
    }, error => {
      this.alertify.error('Error getting filtered players');
      this.spinner.hide();
    }, () => {
      this.spinner.hide();
    });
  }

  resetFilter() {
    this.spinner.show();
    this.displayPaging = 0;
    this.getDraftPlayers();

    this.searchForm = this.fb.group({
      filter: ['']
    });
  }

}
