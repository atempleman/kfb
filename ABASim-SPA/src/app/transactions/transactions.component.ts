import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LeagueService } from '../_services/league.service';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { League } from '../_models/league';
import { Transaction } from '../_models/transaction';
import { TransferService } from '../_services/transfer.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { TeamService } from '../_services/team.service';
import { Team } from '../_models/team';

@Component({
  selector: 'app-transactions',
  templateUrl: './transactions.component.html',
  styleUrls: ['./transactions.component.css']
})
export class TransactionsComponent implements OnInit {
  transactions: Transaction[] = [];
  transCount = 0;
  team: Team;
  league: League;

  constructor(private router: Router, private leagueService: LeagueService, private alertify: AlertifyService,
              private authService: AuthService, private transferService: TransferService, private spinner: NgxSpinnerService,
              private teamService: TeamService) { }

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
    this.leagueService.getTransactions(this.league.id).subscribe(result => {
      this.transactions = result;
      this.transCount = this.transactions.length;
    }, error => {
      this.alertify.error('Error getting league transactions');
    }, () => {
      this.spinner.hide();
    });
  }

  goToStandings() {
    this.router.navigate(['/standings']);
  }

  goToLeague() {
    this.router.navigate(['/league']);
  }

  goToSchedule() {
    this.router.navigate(['/schedule']);
  }

  goToStats() {
    this.router.navigate(['/stats']);
  }

  viewPlayer(player: Transaction) {
    this.transferService.setData(player.playerId);
    this.router.navigate(['/view-player']);
  }

}
