import { Component, OnInit, TemplateRef } from '@angular/core';
import { AlertifyService } from '../_services/alertify.service';
import { DraftService } from '../_services/draft.service';
import { DraftPlayer } from '../_models/draftPlayer';
import { AuthService } from '../_services/auth.service';
import { TeamService } from '../_services/team.service';
import { Team } from '../_models/team';
import { AddDraftRank } from '../_models/addDraftRank';
import { Router } from '@angular/router';
import { TransferService } from '../_services/transfer.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { DraftTracker } from '../_models/draftTracker';
import { InitialDraftPicks } from '../_models/initialDraftPicks';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { AdminService } from '../_services/admin.service';
import { DraftSelection } from '../_models/draftSelection';
import { LeagueService } from '../_services/league.service';
import { League } from '../_models/league';
import { GetRosterQuickView } from '../_models/getRosterQuickView';
import { LeagueStatusUpdate } from '../_models/leagueStatusUpdate';

@Component({
  selector: 'app-draftboard',
  templateUrl: './draftboard.component.html',
  styleUrls: ['./draftboard.component.css']
})
export class DraftboardComponent implements OnInit {
  draftPlayers: DraftPlayer[] = [];
  team: Team;
  public modalRef: BsModalRef;
  currentPick: InitialDraftPicks;
  selection: DraftPlayer;
  league: League;

  constructor(private alertify: AlertifyService, private draftService: DraftService, private authService: AuthService,
              private teamService: TeamService, private router: Router, private transferService: TransferService,
              private spinner: NgxSpinnerService, private modalService: BsModalService, private adminService: AdminService,
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

    if (this.league.stateId < 3) {
      // this.currentPick.round = 0;
      // this.currentPick.teamId = -1;
    } else {
      this.draftService.getCurrentInitialDraftPick(this.league.id).subscribe(result => {
        this.currentPick = result;
      }, error => {
        this.alertify.error('Error getting current draft pick');
      });
    }

    
  }

  getDraftboardPlayers() {
    const summary: GetRosterQuickView = {
      teamId: this.team.teamId,
      leagueId: this.league.id
    };
    this.draftService.getDraftBoardForTeam(summary).subscribe(result => {
      this.draftPlayers = result;
    }, error => {
      this.alertify.error('Error getting draftboard');
      this.spinner.hide();
    }, () => {
      this.spinner.hide();
    });
  }

  removeDraftRanking(player: DraftPlayer) {
    const newRanking = {} as AddDraftRank;
    newRanking.playerId = player.playerId;
    newRanking.teamId = this.team.teamId;
    newRanking.leagueId = this.league.id

    this.draftService.removeDraftPlayerRanking(newRanking).subscribe(result => {
    }, error => {
      this.alertify.error('Error removing draftboard record');
    }, () => {
      // Now need to remove the record from the screen
      const index = this.draftPlayers.findIndex(x => x.playerId === player.playerId);
      this.draftPlayers.splice(index, 1);
    });
  }

  moveUp(player: DraftPlayer) {
    const newRanking = {} as AddDraftRank;
    newRanking.playerId = player.playerId;
    newRanking.teamId = this.team.teamId;
    newRanking.leagueId = this.league.id

    this.draftService.moveRankingUp(newRanking).subscribe(result => {
    }, error => {
      this.alertify.error('Error changing ranking');
    }, () => {
      this.getDraftboardPlayers();
    });
  }

  moveDown(player: DraftPlayer) {
    const newRanking = {} as AddDraftRank;
    newRanking.playerId = player.playerId;
    newRanking.teamId = this.team.teamId;
    newRanking.leagueId = this.league.id

    this.draftService.moveRankingDown(newRanking).subscribe(result => {
    }, error => {
      this.alertify.error('Error changing ranking');
    }, () => {
      this.getDraftboardPlayers();
    });
  }

  draftHQClicked() {
    this.router.navigate(['/draft']);
  }

  playerPoolClicked() {
    this.router.navigate(['/draftplayerpool']);
  }

  lotteryClicked() {
    this.router.navigate(['/initiallottery']);
  }

  viewPlayer(player: number) {
    this.transferService.setData(player);
    this.router.navigate(['/view-player']);
  }

  public openModal(template: TemplateRef<any>, selection: DraftPlayer) {
    this.selection = selection;
    this.modalRef = this.modalService.show(template);
  }

  makeDraftPick() {
    const selectedPick: DraftSelection = {
      pick: this.currentPick.pick,
      playerId: this.selection.playerId,
      round: this.currentPick.round,
      teamId: this.team.teamId,
      leagueId: this.league.id
    };

    this.draftService.makeDraftPick(selectedPick).subscribe(result => {
    }, error => {
      this.alertify.error('Error making pick');
    }, () => {
      this.modalRef.hide();
      this.alertify.success('Selection made successfully');

      if (this.currentPick.round === 13 && this.currentPick.pick === 30) {
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
      } else {
        this.draftService.getCurrentInitialDraftPick(this.league.id).subscribe(result => {
          this.currentPick = result;
        }, error => {
          this.alertify.error('Error getting current draft pick');
        }, () => {
          this.getDraftboardPlayers();
        });
      }
    });
   }

}
