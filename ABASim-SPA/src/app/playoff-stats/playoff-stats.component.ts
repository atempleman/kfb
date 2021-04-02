import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LeagueService } from '../_services/league.service';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { TransferService } from '../_services/transfer.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { League } from '../_models/league';
import { LeagueLeadersPoints } from '../_models/leagueLeadersPoints';
import { LeagueLeadersAssists } from '../_models/leagueLeadersAssists';
import { LeagueLeadersRebounds } from '../_models/leagueLeadersRebounds';
import { LeagueLeadersBlocks } from '../_models/leagueLeadersBlocks';
import { LeagueLeadersSteals } from '../_models/leagueLeadersSteals';
import { LeagueLeadersFouls } from '../_models/leagueLeadersFouls';
import { LeagueLeadersMinutes } from '../_models/leagueLeadersMinutes';
import { LeagueLeadersTurnover } from '../_models/leagueLeadersTurnovers';
import { TeamService } from '../_services/team.service';
import { Team } from '../_models/team';
import { GetStatsLeague } from '../_models/getStatsLeague';

@Component({
  selector: 'app-playoff-stats',
  templateUrl: './playoff-stats.component.html',
  styleUrls: ['./playoff-stats.component.css']
})
export class PlayoffStatsComponent implements OnInit {
  league: League;
  pointsStats: LeagueLeadersPoints[] = [];
  assistsStats: LeagueLeadersAssists[] = [];
  reboundStats: LeagueLeadersRebounds[] = [];
  blocksStats: LeagueLeadersBlocks[] = [];
  stealsStats: LeagueLeadersSteals[] = [];
  foulsStats: LeagueLeadersFouls[] = [];
  minutesStats: LeagueLeadersMinutes[] = [];
  turnoversStats: LeagueLeadersTurnover[] = [];

  pager = 1;
  pagerMax = 100;
  recordTotal = 0;

  pointsSelection = true;
  assistsSelection = false;
  reboundsSelection = false;
  foulsSelection = false;
  stealsSelection = false;
  blocksSelection = false;
  turnoversSelection = false;
  minutesSelection = false;

  selectedStat = 0;

  team: Team;

  constructor(private router: Router, private leagueService: LeagueService, private alertify: AlertifyService,
              private authService: AuthService, private transferService: TransferService, private spinner: NgxSpinnerService,
              private teamService: TeamService) { }

  ngOnInit() {
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
    this.leagueService.getLeagueForUserId(this.team.id).subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting League Details');
    }, () => {
      this.setupPage();
    });
  }

  setupPage() {
    this.selectedStat = this.transferService.getData();

    if(!this.selectedStat) {
      this.selectedStat = 0;
    }

    if (this.selectedStat === 0 || this.selectedStat === 1) {

      const summary: GetStatsLeague = {
        page: 1,
        leagueId: this.league.id
      };

      this.leagueService.getPlayoffsPointsLeagueLeadersForPage(summary).subscribe(result => {
        console.log(result);
        this.pointsStats = result;
      }, error => {
        this.alertify.error('Error getting scoring stats');
      }, () => {
        this.getCountForPoints();
      });
    } else if (this.selectedStat === 2) {
      this.getCountForPoints();
      this.reboundsClick();
    } else if (this.selectedStat === 3) {
      this.getCountForPoints();
      this.assistsClick();
    } else if (this.selectedStat === 4) {
      this.getCountForPoints();
      this.stealsClick();
    } else if (this.selectedStat === 5) {
      this.getCountForPoints();
      this.blocksClick();
    }

    const summary: GetStatsLeague = {
      page: 1,
      leagueId: this.league.id
    };
    this.leagueService.getPlayoffsPointsLeagueLeadersForPage(summary).subscribe(result => {
      this.pointsStats = result;
    }, error => {
      this.alertify.error('Error getting scoring stats');
    }, () => {
      this.getCountForPoints();
    });
  }

  getCountForPoints() {
    this.leagueService.getCountOfPointsLeagueLeadersPlayoffs(this.league.id).subscribe(result => {
      this.recordTotal = result;
    }, error => {
      this.alertify.error('Error getting total records');
    }, () => {
      this.pagerMax = +(this.recordTotal / 10).toFixed(0);
      this.spinner.hide();
    });
  }

  pagerNext() {
    this.spinner.show();

    this.pager = this.pager + 1;
    if (this.pager > this.pagerMax) {
      this.pager = this.pager - 1;
    }

    const summary: GetStatsLeague = {
      page: this.pager,
      leagueId: this.league.id
    };

    if (this.pointsSelection) {
      this.leagueService.getPlayoffsPointsLeagueLeadersForPage(summary).subscribe(result => {
        this.pointsStats = result;
      }, error => {
        this.alertify.error('Error getting scoring stats');
      }, () => {
        this.spinner.hide();
      });
    } else if (this.assistsSelection) {
      this.leagueService.getPlayoffsAssistsLeagueLeadersForPage(summary).subscribe(result => {
        this.assistsStats = result;
      }, error => {
        this.alertify.error('Error getting scoring stats');
      }, () => {
        this.spinner.hide();
      });
    } else if (this.reboundsSelection) {
      this.leagueService.getPlayoffsReboundsLeagueLeadersForPage(summary).subscribe(result => {
        this.reboundStats = result;
      }, error => {
        this.alertify.error('Error getting scoring stats');
      }, () => {
        this.spinner.hide();
      });
    } else if (this.stealsSelection) {
      this.leagueService.getPlayoffsStealsLeagueLeadersForPage(summary).subscribe(result => {
        this.stealsStats = result;
      }, error => {
        this.alertify.error('Error getting scoring stats');
      }, () => {
        this.spinner.hide();
      });
    } else if (this.blocksSelection) {
      this.leagueService.getPlayoffsBlocksLeagueLeadersForPage(summary).subscribe(result => {
        this.blocksStats = result;
      }, error => {
        this.alertify.error('Error getting scoring stats');
      }, () => {
        this.spinner.hide();
      });
    } else if (this.turnoversSelection) {

    } else if (this.foulsSelection) {
      this.leagueService.getPlayoffsFoulsLeagueLeadersForPage(summary).subscribe(result => {
        this.foulsStats = result;
      }, error => {
        this.alertify.error('Error getting scoring stats');
      }, () => {
        this.spinner.hide();
      });
    } else if (this.minutesSelection) {
      this.leagueService.getPlayoffsMinutesLeagueLeadersForPage(summary).subscribe(result => {
        this.minutesStats = result;
      }, error => {
        this.alertify.error('Error getting scoring stats');
      }, () => {
        this.spinner.hide();
      });
    }
  }

  pagerPrev() {
    this.spinner.show();

    this.pager = this.pager - 1;
    if (this.pager < 1) {
      this.pager = this.pager + 1;
    }

    const summary: GetStatsLeague = {
      page: this.pager,
      leagueId: this.league.id
    };

    if (this.pointsSelection) {
      this.leagueService.getPlayoffsPointsLeagueLeadersForPage(summary).subscribe(result => {
        this.pointsStats = result;
      }, error => {
        this.alertify.error('Error getting scoring stats');
      }, () => {
        this.spinner.hide();
      });
    } else if (this.assistsSelection) {
      this.leagueService.getPlayoffsAssistsLeagueLeadersForPage(summary).subscribe(result => {
        this.assistsStats = result;
      }, error => {
        this.alertify.error('Error getting scoring stats');
      }, () => {
        this.spinner.hide();
      });
    } else if (this.reboundsSelection) {
      this.leagueService.getPlayoffsReboundsLeagueLeadersForPage(summary).subscribe(result => {
        this.reboundStats = result;
      }, error => {
        this.alertify.error('Error getting scoring stats');
      }, () => {
        this.spinner.hide();
      });
    } else if (this.stealsSelection) {
      this.leagueService.getPlayoffsStealsLeagueLeadersForPage(summary).subscribe(result => {
        this.stealsStats = result;
      }, error => {
        this.alertify.error('Error getting scoring stats');
      }, () => {
        this.spinner.hide();
      });
    } else if (this.blocksSelection) {
      this.leagueService.getPlayoffsBlocksLeagueLeadersForPage(summary).subscribe(result => {
        this.blocksStats = result;
      }, error => {
        this.alertify.error('Error getting scoring stats');
      }, () => {
        this.spinner.hide();
      });
    } else if (this.turnoversSelection) {
      this.leagueService.getPlayoffsTurnoversLeagueLeadersForPage(summary).subscribe(result => {
        this.turnoversStats = result;
      }, error => {
        this.alertify.error('Error getting turnover stats');
      }, () => {
        this.spinner.hide();
      });
    } else if (this.foulsSelection) {
      this.leagueService.getPlayoffsFoulsLeagueLeadersForPage(summary).subscribe(result => {
        this.foulsStats = result;
      }, error => {
        this.alertify.error('Error getting scoring stats');
      }, () => {
        this.spinner.hide();
      });
    } else if (this.minutesSelection) {
      this.leagueService.getPlayoffsMinutesLeagueLeadersForPage(summary).subscribe(result => {
        this.minutesStats = result;
      }, error => {
        this.alertify.error('Error getting scoring stats');
      }, () => {
        this.spinner.hide();
      });
    }
  }

  pointsClick() {
    this.spinner.show();

    this.assistsSelection = false;
    this.reboundsSelection = false;
    this.foulsSelection = false;
    this.stealsSelection = false;
    this.blocksSelection = false;
    this.turnoversSelection = false;
    this.minutesSelection = false;

    this.pointsSelection = true;

    this.pager = 1;

    const summary: GetStatsLeague = {
      page: this.pager,
      leagueId: this.league.id
    };

    this.leagueService.getPlayoffsPointsLeagueLeadersForPage(summary).subscribe(result => {
      this.pointsStats = result;
    }, error => {
      this.alertify.error('Error getting scoring stats');
    }, () => {
      this.spinner.hide();
    });
  }

  blocksClick() {
    this.spinner.show();

    this.assistsSelection = false;
    this.reboundsSelection = false;
    this.foulsSelection = false;
    this.stealsSelection = false;
    this.turnoversSelection = false;
    this.minutesSelection = false;
    this.pointsSelection = false;

    this.blocksSelection = true;

    this.pager = 1;

    const summary: GetStatsLeague = {
      page: this.pager,
      leagueId: this.league.id
    };

    this.leagueService.getPlayoffsBlocksLeagueLeadersForPage(summary).subscribe(result => {
      this.blocksStats = result;
    }, error => {
      this.alertify.error('Error getting scoring stats');
    }, () => {
      this.spinner.hide();
    });
  }

  assistsClick() {
    this.spinner.show();

    this.blocksSelection = false;
    this.reboundsSelection = false;
    this.foulsSelection = false;
    this.stealsSelection = false;
    this.turnoversSelection = false;
    this.minutesSelection = false;
    this.pointsSelection = false;

    this.assistsSelection = true;

    this.pager = 1;

    const summary: GetStatsLeague = {
      page: this.pager,
      leagueId: this.league.id
    };

    this.leagueService.getPlayoffsAssistsLeagueLeadersForPage(summary).subscribe(result => {
      this.assistsStats = result;
    }, error => {
      this.alertify.error('Error getting scoring stats');
    }, () => {
      this.spinner.hide();
    });
  }

  reboundsClick() {
    this.spinner.show();

    this.blocksSelection = false;
    this.assistsSelection = false;
    this.foulsSelection = false;
    this.stealsSelection = false;
    this.turnoversSelection = false;
    this.minutesSelection = false;
    this.pointsSelection = false;

    this.reboundsSelection = true;

    this.pager = 1;

    const summary: GetStatsLeague = {
      page: this.pager,
      leagueId: this.league.id
    };

    this.leagueService.getPlayoffsReboundsLeagueLeadersForPage(summary).subscribe(result => {
      this.reboundStats = result;
    }, error => {
      this.alertify.error('Error getting scoring stats');
    }, () => {
      this.spinner.hide();
    });
  }

  stealsClick() {
    this.spinner.show();

    this.blocksSelection = false;
    this.assistsSelection = false;
    this.foulsSelection = false;
    this.reboundsSelection = false;
    this.turnoversSelection = false;
    this.minutesSelection = false;
    this.pointsSelection = false;

    this.stealsSelection = true;

    this.pager = 1;

    const summary: GetStatsLeague = {
      page: this.pager,
      leagueId: this.league.id
    };

    this.leagueService.getPlayoffsStealsLeagueLeadersForPage(summary).subscribe(result => {
      this.stealsStats = result;
    }, error => {
      this.alertify.error('Error getting scoring stats');
    }, () => {
      this.spinner.hide();
    });
  }

  minutesClick() {
    this.spinner.show();

    this.blocksSelection = false;
    this.assistsSelection = false;
    this.foulsSelection = false;
    this.reboundsSelection = false;
    this.turnoversSelection = false;
    this.stealsSelection = false;
    this.pointsSelection = false;

    this.minutesSelection = true;

    this.pager = 1;

    const summary: GetStatsLeague = {
      page: this.pager,
      leagueId: this.league.id
    };

    this.leagueService.getPlayoffsMinutesLeagueLeadersForPage(summary).subscribe(result => {
      this.minutesStats = result;
    }, error => {
      this.alertify.error('Error getting scoring stats');
    }, () => {
      this.spinner.hide();
    });
  }

  foulsClick() {
    this.spinner.show();

    this.blocksSelection = false;
    this.assistsSelection = false;
    this.minutesSelection = false;
    this.reboundsSelection = false;
    this.turnoversSelection = false;
    this.stealsSelection = false;
    this.pointsSelection = false;

    this.foulsSelection = true;

    this.pager = 1;

    const summary: GetStatsLeague = {
      page: this.pager,
      leagueId: this.league.id
    };

    this.leagueService.getPlayoffsFoulsLeagueLeadersForPage(summary).subscribe(result => {
      this.foulsStats = result;
    }, error => {
      this.alertify.error('Error getting scoring stats');
    }, () => {
      this.spinner.hide();
    });
  }

  turnoversClick() {
    this.spinner.show();

    this.blocksSelection = false;
    this.assistsSelection = false;
    this.foulsSelection = false;
    this.reboundsSelection = false;
    this.foulsSelection = false;
    this.stealsSelection = false;
    this.pointsSelection = false;

    this.turnoversSelection = true;

    this.pager = 1;

    const summary: GetStatsLeague = {
      page: this.pager,
      leagueId: this.league.id
    };

    this.leagueService.getPlayoffsTurnoversLeagueLeadersForPage(summary).subscribe(result => {
      this.turnoversStats = result;
    }, error => {
      this.alertify.error('Error getting scoring stats');
    }, () => {
      this.spinner.hide();
    });
  }

  viewPlayer(player: number) {
    this.transferService.setData(player);
    this.router.navigate(['/view-player']);
  }
}
