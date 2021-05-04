import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { TeamService } from '../_services/team.service';
import { Team } from '../_models/team';
import { CoachSetting } from '../_models/coachSetting';
import { ExtendedPlayer } from '../_models/extendedPlayer';
import { Player } from '../_models/player';
import { PlayerInjury } from '../_models/playerInjury';
import { OffensiveStrategy } from '../_models/offensiveStrategy';
import { DefensiveStrategy } from '../_models/defensiveStrategyId';
import { Strategy } from '../_models/strategy';
import { SaveStrategy } from '../_models/saveStrategy';
import { NgxSpinnerService } from 'ngx-spinner';
import { League } from '../_models/league';
import { LeagueService } from '../_services/league.service';
import { GetRosterQuickView } from '../_models/getRosterQuickView';

@Component({
  selector: 'app-coaching',
  templateUrl: './coaching.component.html',
  styleUrls: ['./coaching.component.css']
})
export class CoachingComponent implements OnInit {
  depthAccordion = 0;

  gotoAccordion = 0;
  isGoToEdit = 0;
  gotoOne: number;
  gotoTwo: number;
  gotoThree: number;

  strategyAccordion = 0;
  isStrategyEdit = 0;

  isAdmin: number;
  team: Team;
  coachSetting: CoachSetting;
  extendedPlayers: Player[] = [];

  teamsInjuries: PlayerInjury[] = [];

  injuredOne = 0;
  injuredTwo = 0;
  injuredThree = 0;

  goToTab = 1;
  offensiveStrategyTab = 0;
  defensiveStrategyTab = 0;

  defStrategySelection = 0;
  offStrategySelection = 0;

  offStrategies: OffensiveStrategy[] = [];
  defStrategies: DefensiveStrategy[] = [];
  teamStrategy: Strategy;

  league: League;

  constructor(private router: Router, private alertify: AlertifyService, private authService: AuthService,
              private teamService: TeamService, private spinner: NgxSpinnerService, private leagueService:LeagueService) { }

  ngOnInit() {
    // Check to see if the user is an admin user
    this.isAdmin = this.authService.isAdmin();

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
    this.getPlayerInjuries();
    this.getCoachSettings();
    this.getStrategies();
  }

  getPlayerInjuries() {
    const summary: GetRosterQuickView = {
      teamId: this.team.teamId,
      leagueId: this.league.id
    };

    this.teamService.getPlayerInjuriesForTeam(summary).subscribe(result => {
      this.teamsInjuries = result;
    }, error => {
      this.alertify.error('Error getting teams injuries');
    });
  }

  getCoachSettings() {
    const summary: GetRosterQuickView = {
      teamId: this.team.teamId,
      leagueId: this.league.id
    };

    this.teamService.getRosterForTeam(summary).subscribe(result => {
      this.extendedPlayers = result;
    }, error => {
      this.alertify.error('Error getting players');
    }, () => {
      this.extendedPlayers.forEach(element => {
        const injured = this.teamsInjuries.find(x => x.playerId === element.playerId);

        if (injured) {
          const index = this.extendedPlayers.indexOf(element, 0);
          this.extendedPlayers.splice(index, 1);
        }
      });
    });

    this.teamService.getCoachingSettings(summary).subscribe(result => {
      this.coachSetting = result;
    }, error => {
      this.alertify.error('Error getting Coach Settings');
    }, () => {
      this.gotoOne = this.coachSetting.goToPlayerOne;
      this.gotoTwo = this.coachSetting.goToPlayerTwo;
      this.gotoThree = this.coachSetting.goToPlayerThree;

      console.log(this.coachSetting);
    });
  }

  getStrategies() {
    const summary: GetRosterQuickView = {
      teamId: this.team.teamId,
      leagueId: this.league.id
    };

    this.teamService.getStrategyForTeam(summary).subscribe(result => {
      this.teamStrategy = result;
    }, error => {
      this.alertify.error('Error getting team strategy');
    }, () => {
      this.offStrategySelection = this.teamStrategy.offensiveStrategyId;
      this.defStrategySelection = this.teamStrategy.defensiveStrategyId;
    });

    this.teamService.getOffensiveStrategies().subscribe(result => {
      this.offStrategies = result;
    }, error => {
      this.alertify.error('Error getting offensive strategies');
    });

    this.teamService.getDefensiveStrategies().subscribe(result => {
      this.defStrategies = result;
    }, error => {
      this.alertify.error('Error getting defensive stratgies');
    });
  }

  getPlayerNameWithInjuredCheck(playerId: number, gtPlayerNumber: number) {
    // Check if the player is injured
    const injured = this.teamsInjuries.find(x => x.playerId === playerId);
    if (injured) {
      if (gtPlayerNumber === 1) {
        this.injuredOne = 1;
      } else if (gtPlayerNumber === 2) {
        this.injuredTwo = 1;
      } else if (gtPlayerNumber === 3) {
        this.injuredThree = 1;
      }
    } else {
      const player = this.extendedPlayers.find(x => x.playerId === playerId);
      return player.firstName + ' ' + player.surname;
    }
  }

  getPlayerName(playerId: number) {
    const player = this.extendedPlayers.find(x => x.playerId === playerId);
    return player.firstName + ' ' + player.surname;
  }

  editCoaching() {
    this.isGoToEdit = 1;
  }

  editStrategy() {
    this.isStrategyEdit = 1;
  }

  saveCoaching() {
    // Need to get the values
    console.log(this.coachSetting);

    this.coachSetting.goToPlayerOne = +this.gotoOne;
    this.coachSetting.goToPlayerTwo = +this.gotoTwo;
    this.coachSetting.goToPlayerThree = +this.gotoThree;
    this.coachSetting.leagueId = this.league.id;

    // Now pass this through to ther servie

    this.teamService.saveCoachingSettings(this.coachSetting).subscribe(result => {
    }, error => {
      this.alertify.error('Error saving Coaching Settings');
    }, () => {
      this.alertify.success('Coach Settings saved successfully');
    });

    this.isGoToEdit = 0;
  }

  saveStrategy() {
    if (this.offStrategySelection !== 0) {
      // tslint:disable-next-line: triple-equals
      const value = this.offStrategies.find(x => x.id == this.offStrategySelection);
      if (this.teamStrategy !== null) {
        this.teamStrategy.offensiveStrategyId = +this.offStrategySelection;
        this.teamStrategy.offensiveStrategyName = value.name;
        this.teamStrategy.offensiveStrategyDesc = value.description;
      } else {
        const ts: Strategy = {
          teamId: this.team.teamId,
          offensiveStrategyId: +this.offStrategySelection,
          defensiveStrategyId: 0,
          offensiveStrategyName: value.name,
          offensiveStrategyDesc: value.description,
          defensiveStrategyName: '',
          defensiveStrategyDesc: '',
          leagueId: this.league.id
        };
        this.teamStrategy = ts;
      }
    }

    if (this.defStrategySelection !== 0) {
      if (this.teamStrategy !== null) {
        // tslint:disable-next-line: triple-equals
        const value = this.defStrategies.find(x => x.id == this.defStrategySelection);

        if (this.teamStrategy !== null) {
          this.teamStrategy.defensiveStrategyId = +this.defStrategySelection;
          this.teamStrategy.defensiveStrategyName = value.name;
          this.teamStrategy.defensiveStrategyDesc = value.description;
        } else {
          const ts: Strategy = {
            teamId: this.team.teamId,
            offensiveStrategyId: 0,
            defensiveStrategyId: +this.defStrategySelection,
            offensiveStrategyName: '',
            offensiveStrategyDesc: '',
            defensiveStrategyName: value.name,
            defensiveStrategyDesc: value.description,
            leagueId: this.league.id
          };
          this.teamStrategy = ts;
        }
      }
    }

    // Now to save the update team strategy
    this.teamService.saveStrategy(this.teamStrategy).subscribe(result => {
    }, error => {
      this.alertify.error('Error saving strategy');
    }, () => {
      this.alertify.success('Team strategy saved successfully');
      this.isStrategyEdit = 0;
    });
  }

  toggleDepth() {
    if (this.depthAccordion == 0) {
      this.depthAccordion = 1;
    } else {
      this.depthAccordion = 0;
    }
  }

  toggleStrategy() {
    if (this.strategyAccordion == 0) {
      this.strategyAccordion = 1;
    } else {
      this.strategyAccordion = 0;
    }
  }

  toggleGoTo() {
    if (this.gotoAccordion == 0) {
      this.gotoAccordion = 1;
      this.isGoToEdit = 0;
    } else {
      this.gotoAccordion = 0;
    }
  }

  cancelCoaching() {
    this.isGoToEdit = 0;
  }

  cancelStrategy() {
    this.isStrategyEdit = 0;
  }

  goToFreeAgents() {
    this.router.navigate(['/freeagents']);
  }

  goToTrades() {
    this.router.navigate(['/trades']);
  }
}
