using System.Linq;
using System.Threading.Tasks;
using ABASim.api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using ABASim.api.Dtos;
using Microsoft.Data.SqlClient;

namespace ABASim.api.Data
{
    public class AdminRepository : IAdminRepository
    {
        private readonly DataContext _context;

        private static Random rng = new Random();

        public AdminRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> UpdateLeagueState(int status, int leagueId)
        {
            var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == leagueId);
            var currentState = league.StateId;
            league.StateId = status;

            if (status == 2)
            {
                league.Day = 1;
            }
            else if (status == 3)
            {
                var result = GenerateAutoPickOrder(leagueId);
                league.Day = 2;
            }
            else if (status == 6)
            {
                league.Day = 30;
            }
            else if (status == 7)
            {
                league.Day = 45;
            }
            else if (status == 8)
            {
                league.Day = 218;
            }

            _context.Update(league);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveTeamRegistration(int teamId, int leagueId)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(x => x.TeamId == teamId && x.LeagueId == leagueId);
            team.UserId = 0;
            _context.Update(team);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RunInitialDraftLottery(int leagueId)
        {
            var teams = await _context.Teams.Where(x => x.LeagueId == leagueId).ToListAsync();

            List<int> teamIds = new List<int>();
            // Now get a list of the TeamIds
            foreach (Team t in teams)
            {
                teamIds.Add(t.TeamId);
            }

            // Now need to randomly sort the list
            int n = teamIds.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                int value = teamIds[k];
                teamIds[k] = teamIds[n];
                teamIds[n] = value;
            }

            // TeamIds is now sorted
            // Now need to go through and save the draft picks
            for (int i = 1; i < 14; i++)
            {
                if (i % 2 == 1)
                {
                    for (int j = 1; j < 31; j++)
                    {
                        InitialDraft draftPick = new InitialDraft
                        {
                            Round = i,
                            Pick = j,
                            TeamId = teamIds[j - 1],
                            PlayerId = 0,
                            LeagueId = leagueId
                        };
                        await _context.AddAsync(draftPick);
                    }
                }
                else
                {
                    int pickNo = 1;
                    for (int j = 30; j > 0; j--)
                    {
                        InitialDraft draftPick = new InitialDraft
                        {
                            Round = i,
                            Pick = pickNo,
                            TeamId = teamIds[j - 1],
                            PlayerId = 0,
                            LeagueId = leagueId
                        };
                        await _context.AddAsync(draftPick);
                        pickNo++;
                    }
                }



            }

            // Now need to update the league status
            var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == leagueId);
            league.StateId = 3;
            _context.Update(league);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RunDayRollOver(int leagueId)
        {
            var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == leagueId);
            var lc = await _context.LeagueConfigs.FirstOrDefaultAsync(x => x.LeagueId == leagueId);

            if (league.Day >= 175 && lc.NewDrafteesLoaded == 0)
            {
                try
                {
                    int nextSeasonYear = 0;
                    if (league.Year == 1314) {
                        nextSeasonYear = 1415;
                    } else if (league.Year == 1415) {
                        nextSeasonYear = 1516;
                    } else if (league.Year == 1516) {
                        nextSeasonYear = 1617;
                    } else if (league.Year == 1617) {
                        nextSeasonYear = 1718;
                    } else if (league.Year == 1718) {
                        nextSeasonYear = 1819;
                    } else if (league.Year == 1819) {
                        nextSeasonYear = 1920;
                    } else if (league.Year == 1920) {
                        nextSeasonYear = 2021;
                    }

                    var currentSeasonId = new SqlParameter("@currentSeasonId", league.Year);
                    var nextSeasonId = new SqlParameter("@nextSeasonId", nextSeasonYear);
                    var lid = new SqlParameter("@leagueId", league.Id);
                    await _context.Database.ExecuteSqlRawAsync("exec spNewDraftees @currentSeasonId, @nextSeasonId, @leagueId", currentSeasonId, nextSeasonId, lid);
                    lc.NewDrafteesLoaded = 1;
                    _context.Update(lc);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            // Need to update player stats
            if (league.StateId == 7)
            {
                var todaysGames = await _context.Schedules.Where(x => x.GameDay == league.Day).ToListAsync();

                foreach (var game in todaysGames)
                {
                    // Now need to get the boxscores for the game
                    var boxScores = await _context.GameBoxScores.Where(x => x.GameId == game.Id && x.LeagueId == leagueId).ToListAsync();

                    foreach (var bs in boxScores)
                    {
                        if (bs.Minutes != 0)
                        {
                            // Now need to see if the player stat record exists for the player
                            var playerStats = await _context.PlayerStats.FirstOrDefaultAsync(x => x.PlayerId == bs.PlayerId && x.LeagueId == leagueId);
                            if (playerStats != null)
                            {
                                // Player already has a player stats record
                                playerStats.Assists = playerStats.Assists + bs.Assists;
                                playerStats.Blocks = playerStats.Blocks + bs.Blocks;
                                playerStats.DRebs = playerStats.DRebs + bs.DRebs;
                                playerStats.FieldGoalsAttempted = playerStats.FieldGoalsAttempted + bs.FieldGoalsAttempted;
                                playerStats.FieldGoalsMade = playerStats.FieldGoalsMade + bs.FieldGoalsMade;
                                playerStats.Fouls = playerStats.Fouls + bs.Fouls;
                                playerStats.FreeThrowsAttempted = playerStats.FreeThrowsAttempted + bs.FreeThrowsAttempted;
                                playerStats.FreeThrowsMade = playerStats.FreeThrowsMade + bs.FreeThrowsMade;
                                playerStats.GamesPlayed = playerStats.GamesPlayed + 1;
                                playerStats.Minutes = playerStats.Minutes + bs.Minutes;
                                playerStats.ORebs = playerStats.ORebs + bs.ORebs;
                                playerStats.Points = playerStats.Points + bs.Points;
                                playerStats.Rebounds = playerStats.Rebounds + bs.Rebounds;
                                playerStats.Steals = playerStats.Steals + bs.Steals;
                                playerStats.ThreeFieldGoalsAttempted = playerStats.ThreeFieldGoalsAttempted + bs.ThreeFieldGoalsAttempted;
                                playerStats.ThreeFieldGoalsMade = playerStats.ThreeFieldGoalsMade + bs.ThreeFieldGoalsMade;
                                playerStats.Turnovers = playerStats.Turnovers + bs.Turnovers;

                                // Now need to update the averages
                                playerStats.Ppg = Decimal.Divide(playerStats.Points, playerStats.GamesPlayed);
                                playerStats.Apg = Decimal.Divide(playerStats.Assists, playerStats.GamesPlayed);
                                playerStats.Rpg = Decimal.Divide(playerStats.Rebounds, playerStats.GamesPlayed);
                                playerStats.Spg = Decimal.Divide(playerStats.Steals, playerStats.GamesPlayed);
                                playerStats.Bpg = Decimal.Divide(playerStats.Blocks, playerStats.GamesPlayed);
                                playerStats.Mpg = Decimal.Divide(playerStats.Minutes, playerStats.GamesPlayed);
                                playerStats.Fpg = Decimal.Divide(playerStats.Fouls, playerStats.GamesPlayed);
                                playerStats.Tpg = Decimal.Divide(playerStats.Turnovers, playerStats.GamesPlayed);

                                _context.PlayerStats.Update(playerStats);
                            }
                            else
                            {
                                PlayerStat newPlayerStats = new PlayerStat
                                {
                                    Assists = bs.Assists,
                                    Blocks = bs.Blocks,
                                    DRebs = bs.DRebs,
                                    FieldGoalsAttempted = bs.FieldGoalsAttempted,
                                    FieldGoalsMade = bs.FieldGoalsMade,
                                    Fouls = bs.Fouls,
                                    FreeThrowsAttempted = bs.FreeThrowsAttempted,
                                    FreeThrowsMade = bs.FreeThrowsMade,
                                    GamesPlayed = 1,
                                    Minutes = bs.Minutes,
                                    ORebs = bs.ORebs,
                                    Points = bs.Points,
                                    PlayerId = bs.PlayerId,
                                    Rebounds = bs.Rebounds,
                                    Steals = bs.Steals,
                                    ThreeFieldGoalsAttempted = bs.ThreeFieldGoalsAttempted,
                                    ThreeFieldGoalsMade = bs.ThreeFieldGoalsMade,
                                    Turnovers = bs.Turnovers,
                                    Ppg = bs.Points / 1,
                                    Apg = bs.Assists / 1,
                                    Rpg = bs.Rebounds / 1,
                                    Spg = bs.Steals / 1,
                                    Bpg = bs.Blocks / 1,
                                    Mpg = bs.Minutes / 1,
                                    Fpg = bs.Fouls / 1,
                                    Tpg = bs.Turnovers / 1,
                                    LeagueId = leagueId
                                };
                                await _context.AddAsync(newPlayerStats);
                            }
                        }
                    }
                }
                // Need to check the injuries and update appropriately
                await DailyInjuriesUpdate(league.StateId, league.Day, leagueId);

                // league.Day = league.Day + 1;
            }
            else if (league.StateId == 8 || league.StateId == 9 || league.StateId == 10 || league.StateId == 11)
            {
                var todaysGames = await _context.SchedulesPlayoffs.Where(x => x.GameDay == (league.Day) && x.LeagueId == leagueId).ToListAsync();

                foreach (var game in todaysGames)
                {
                    // Now need to get the boxscores for the game
                    var boxScores = await _context.PlayoffBoxScores.Where(x => x.GameId == game.Id && x.LeagueId == leagueId).ToListAsync();

                    foreach (var bs in boxScores)
                    {
                        if (bs.Minutes != 0)
                        {
                            // Now need to see if the player stat record exists for the player
                            var playerStats = await _context.PlayerStatsPlayoffs.FirstOrDefaultAsync(x => x.PlayerId == bs.PlayerId && x.LeagueId == leagueId);
                            if (playerStats != null)
                            {
                                // Player already has a player stats record
                                playerStats.Assists = playerStats.Assists + bs.Assists;
                                playerStats.Blocks = playerStats.Blocks + bs.Blocks;
                                playerStats.DRebs = playerStats.DRebs + bs.DRebs;
                                playerStats.FieldGoalsAttempted = playerStats.FieldGoalsAttempted + bs.FieldGoalsAttempted;
                                playerStats.FieldGoalsMade = playerStats.FieldGoalsMade + bs.FieldGoalsMade;
                                playerStats.Fouls = playerStats.Fouls + bs.Fouls;
                                playerStats.FreeThrowsAttempted = playerStats.FreeThrowsAttempted + bs.FreeThrowsAttempted;
                                playerStats.FreeThrowsMade = playerStats.FreeThrowsMade + bs.FreeThrowsMade;
                                playerStats.GamesPlayed = playerStats.GamesPlayed + 1;
                                playerStats.Minutes = playerStats.Minutes + bs.Minutes;
                                playerStats.ORebs = playerStats.ORebs + bs.ORebs;
                                playerStats.Points = playerStats.Points + bs.Points;
                                playerStats.Rebounds = playerStats.Rebounds + bs.Rebounds;
                                playerStats.Steals = playerStats.Steals + bs.Steals;
                                playerStats.ThreeFieldGoalsAttempted = playerStats.ThreeFieldGoalsAttempted + bs.ThreeFieldGoalsAttempted;
                                playerStats.ThreeFieldGoalsMade = playerStats.ThreeFieldGoalsMade + bs.ThreeFieldGoalsMade;
                                playerStats.Turnovers = playerStats.Turnovers + bs.Turnovers;

                                // Now need to update the averages
                                playerStats.Ppg = Decimal.Divide(playerStats.Points, playerStats.GamesPlayed);
                                playerStats.Apg = Decimal.Divide(playerStats.Assists, playerStats.GamesPlayed);
                                playerStats.Rpg = Decimal.Divide(playerStats.Rebounds, playerStats.GamesPlayed);
                                playerStats.Spg = Decimal.Divide(playerStats.Steals, playerStats.GamesPlayed);
                                playerStats.Bpg = Decimal.Divide(playerStats.Blocks, playerStats.GamesPlayed);
                                playerStats.Mpg = Decimal.Divide(playerStats.Minutes, playerStats.GamesPlayed);
                                playerStats.Fpg = Decimal.Divide(playerStats.Fouls, playerStats.GamesPlayed);
                                playerStats.Tpg = Decimal.Divide(playerStats.Turnovers, playerStats.GamesPlayed);

                                _context.PlayerStatsPlayoffs.Update(playerStats);
                            }
                            else
                            {
                                PlayerStatsPlayoff newPlayerStats = new PlayerStatsPlayoff
                                {
                                    Assists = bs.Assists,
                                    Blocks = bs.Blocks,
                                    DRebs = bs.DRebs,
                                    FieldGoalsAttempted = bs.FieldGoalsAttempted,
                                    FieldGoalsMade = bs.FieldGoalsMade,
                                    Fouls = bs.Fouls,
                                    FreeThrowsAttempted = bs.FreeThrowsAttempted,
                                    FreeThrowsMade = bs.FreeThrowsMade,
                                    GamesPlayed = 1,
                                    Minutes = bs.Minutes,
                                    ORebs = bs.ORebs,
                                    Points = bs.Points,
                                    PlayerId = bs.PlayerId,
                                    Rebounds = bs.Rebounds,
                                    Steals = bs.Steals,
                                    ThreeFieldGoalsAttempted = bs.ThreeFieldGoalsAttempted,
                                    ThreeFieldGoalsMade = bs.ThreeFieldGoalsMade,
                                    Turnovers = bs.Turnovers,
                                    Ppg = bs.Points / 1,
                                    Apg = bs.Assists / 1,
                                    Rpg = bs.Rebounds / 1,
                                    Spg = bs.Steals / 1,
                                    Bpg = bs.Blocks / 1,
                                    Mpg = bs.Minutes / 1,
                                    Fpg = bs.Fouls / 1,
                                    Tpg = bs.Turnovers / 1,
                                    LeagueId = leagueId
                                };
                                await _context.AddAsync(newPlayerStats);
                            }
                        }
                    }
                    await _context.SaveChangesAsync(); // something here is breaking!
                }

                // Need to check the injuries and update appropriately
                await DailyInjuriesUpdate(league.StateId, league.Day, leagueId);

                // Need to do the next days schedule
                league.Day = league.Day + 1;

                if (league.StateId == 8)
                {
                    // Now get list of all PlayOff series for Round 1
                    var allSeries = await _context.PlayoffSerieses.Where(x => x.Round == 1 && x.LeagueId == leagueId).ToListAsync();

                    if (allSeries != null)
                    {
                        foreach (var series in allSeries)
                        {
                            if (series.HomeWins != 4 && series.AwayWins != 4)
                            {
                                int homeTeamId = 0;
                                int awayTeamId = 0;

                                int totalGamesPlayed = series.HomeWins + series.AwayWins;

                                // This is where the day needs to be determined
                                int nextGameDay = 0;
                                if (league.Day == 219)
                                {
                                    // Game 2
                                    nextGameDay = 220;
                                }
                                else if (league.Day == 221)
                                {
                                    // Game 3
                                    nextGameDay = 223;
                                }
                                else if (league.Day == 225)
                                {
                                    // Game 4
                                    nextGameDay = 226;
                                }
                                else if (league.Day == 228)
                                {
                                    // Game 5
                                    nextGameDay = 229;
                                }
                                else if (league.Day == 230)
                                {
                                    // Game 6
                                    nextGameDay = 231;
                                }
                                else if (league.Day == 232)
                                {
                                    // Game 7
                                    nextGameDay = 233;
                                }

                                if (totalGamesPlayed == 2 || totalGamesPlayed == 3 || totalGamesPlayed == 5)
                                {
                                    homeTeamId = series.AwayTeamId;
                                    awayTeamId = series.HomeTeamId;
                                }
                                else
                                {
                                    homeTeamId = series.HomeTeamId;
                                    awayTeamId = series.AwayTeamId;
                                }

                                SchedulesPlayoff sched = new SchedulesPlayoff
                                {
                                    AwayTeamId = awayTeamId,
                                    HomeTeamId = homeTeamId,
                                    SeriesId = series.Id,
                                    GameDay = nextGameDay,
                                    LeagueId = leagueId
                                };
                                await _context.AddAsync(sched);
                            }
                        }
                        await _context.SaveChangesAsync();
                    }
                }
                else if (league.StateId == 9)
                {
                    // Now get list of all PlayOff series for Round 1
                    var allSeries = await _context.PlayoffSerieses.Where(x => x.Round == 2 && x.LeagueId == leagueId).ToListAsync();

                    if (allSeries != null)
                    {
                        foreach (var series in allSeries)
                        {
                            if (series.HomeWins != 4 && series.AwayWins != 4)
                            {
                                int homeTeamId = 0;
                                int awayTeamId = 0;

                                int totalGamesPlayed = series.HomeWins + series.AwayWins;

                                // This is where the day needs to be determined
                                int nextGameDay = 0;
                                if (league.Day == 236)
                                {
                                    // Game 2
                                    nextGameDay = 237;
                                }
                                else if (league.Day == 238)
                                {
                                    // Game 3
                                    nextGameDay = 240;
                                }
                                else if (league.Day == 241)
                                {
                                    // Game 4
                                    nextGameDay = 242;
                                }
                                else if (league.Day == 243)
                                {
                                    // Game 5
                                    nextGameDay = 245;
                                }
                                else if (league.Day == 246)
                                {
                                    // Game 6
                                    nextGameDay = 247;
                                }
                                else if (league.Day == 248)
                                {
                                    // Game 7
                                    nextGameDay = 249;
                                }

                                if (totalGamesPlayed == 3 || totalGamesPlayed == 4 || totalGamesPlayed == 6)
                                {
                                    homeTeamId = series.AwayTeamId;
                                    awayTeamId = series.HomeTeamId;
                                }
                                else
                                {
                                    homeTeamId = series.HomeTeamId;
                                    awayTeamId = series.AwayTeamId;
                                }

                                SchedulesPlayoff sched = new SchedulesPlayoff
                                {
                                    AwayTeamId = awayTeamId,
                                    HomeTeamId = homeTeamId,
                                    SeriesId = series.Id,
                                    GameDay = nextGameDay,
                                    LeagueId = leagueId
                                };
                                await _context.AddAsync(sched);
                            }
                        }
                        await _context.SaveChangesAsync();
                    }
                }
                else if (league.StateId == 10)
                {
                    // Now get list of all PlayOff series for Round 1
                    var allSeries = await _context.PlayoffSerieses.Where(x => x.Round == 3 && x.LeagueId == leagueId).ToListAsync();

                    if (allSeries != null)
                    {
                        foreach (var series in allSeries)
                        {
                            if (series.HomeWins != 4 && series.AwayWins != 4)
                            {
                                int homeTeamId = 0;
                                int awayTeamId = 0;

                                int totalGamesPlayed = series.HomeWins + series.AwayWins;

                                // This is where the day needs to be determined
                                int nextGameDay = 0;
                                if (league.Day == 251)
                                {
                                    // Game 2
                                    nextGameDay = 252;
                                }
                                else if (league.Day == 253)
                                {
                                    // Game 3
                                    nextGameDay = 255;
                                }
                                else if (league.Day == 256)
                                {
                                    // Game 4
                                    nextGameDay = 257;
                                }
                                else if (league.Day == 258)
                                {
                                    // Game 5
                                    nextGameDay = 260;
                                }
                                else if (league.Day == 261)
                                {
                                    // Game 6
                                    nextGameDay = 262;
                                }
                                else if (league.Day == 263)
                                {
                                    // Game 7
                                    nextGameDay = 264;
                                }

                                if (totalGamesPlayed == 3 || totalGamesPlayed == 4 || totalGamesPlayed == 6)
                                {
                                    homeTeamId = series.AwayTeamId;
                                    awayTeamId = series.HomeTeamId;
                                }
                                else
                                {
                                    homeTeamId = series.HomeTeamId;
                                    awayTeamId = series.AwayTeamId;
                                }

                                SchedulesPlayoff sched = new SchedulesPlayoff
                                {
                                    AwayTeamId = awayTeamId,
                                    HomeTeamId = homeTeamId,
                                    SeriesId = series.Id,
                                    GameDay = nextGameDay,
                                    LeagueId = leagueId
                                };
                                await _context.AddAsync(sched);
                            }
                        }
                        await _context.SaveChangesAsync();
                    }
                }
                else if (league.StateId == 11)
                {
                    // Now get list of all PlayOff series for Round 1
                    var allSeries = await _context.PlayoffSerieses.Where(x => x.Round == 4 && x.LeagueId == leagueId).ToListAsync();

                    if (allSeries != null)
                    {
                        foreach (var series in allSeries)
                        {
                            if (series.HomeWins != 4 && series.AwayWins != 4)
                            {
                                int homeTeamId = 0;
                                int awayTeamId = 0;

                                int totalGamesPlayed = series.HomeWins + series.AwayWins;

                                // This is where the day needs to be determined
                                int nextGameDay = 0;
                                if (league.Day == 268)
                                {
                                    // Game 2
                                    nextGameDay = 269;
                                }
                                else if (league.Day == 270)
                                {
                                    // Game 3
                                    nextGameDay = 272;
                                }
                                else if (league.Day == 273)
                                {
                                    // Game 4
                                    nextGameDay = 274;
                                }
                                else if (league.Day == 275)
                                {
                                    // Game 5
                                    nextGameDay = 277;
                                }
                                else if (league.Day == 278)
                                {
                                    // Game 6
                                    nextGameDay = 279;
                                }
                                else if (league.Day == 280)
                                {
                                    // Game 7
                                    nextGameDay = 281;
                                }

                                if (totalGamesPlayed == 3 || totalGamesPlayed == 4 || totalGamesPlayed == 6)
                                {
                                    homeTeamId = series.AwayTeamId;
                                    awayTeamId = series.HomeTeamId;
                                }
                                else
                                {
                                    homeTeamId = series.HomeTeamId;
                                    awayTeamId = series.AwayTeamId;
                                }

                                SchedulesPlayoff sched = new SchedulesPlayoff
                                {
                                    AwayTeamId = awayTeamId,
                                    HomeTeamId = homeTeamId,
                                    SeriesId = series.Id,
                                    GameDay = nextGameDay,
                                    LeagueId = leagueId
                                };
                                await _context.AddAsync(sched);
                            }
                        }
                        await _context.SaveChangesAsync();
                    }
                }

            }
            // else if (league.StateId == 6)
            // {
            // Preseaon - just rollover day
            // league.Day = league.Day + 1;
            // }
            if (league.StateId != 8 && league.StateId != 9 && league.StateId != 10 && league.StateId != 11)
            {
                league.Day = league.Day + 1;
            }

            // Need to do the free agency checks here - TODO
            await FreeAgentDecisionMaking(leagueId);

            // Need to rollover the day to the next day
            _context.Update(league);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> FreeAgentDecisionMaking(int leagueId)
        {
            var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == leagueId);
            int leageState = league.StateId;

            // var freeAgentDecisions = await _context.FreeAgencyDecisions.FromSqlRaw("SELECT * FROM FreeAgencyDecisions WHERE LeagueId = {1} AND PlayerId IN (SELECT DISTINCT PlayerId FROM FreeAgencyDecisions WHERE DaysToDecide <= {0} AND LeagueId = {1})", league.Day + 1, leagueId).ToListAsync();
            var freeAgentDecisions = await _context.FreeAgencyDecisions.Where(x => (x.DayToDecide <= (league.Day)) && x.LeagueId == leagueId).ToListAsync();

            foreach (var fa in freeAgentDecisions)
            {
                var playerId = fa.PlayerId;

                var offers = await _context.ContractOffers.Where(x => x.PlayerId == playerId && x.Decision == 0 && x.LeagueId == leagueId).ToListAsync();

                ContractOffer acceptedOffer = null;
                if (offers.Count > 1)
                {
                    // We have multiple offers
                    foreach (var off in offers)
                    {
                        var rosterSpotChk = await _context.Rosters.Where(x => x.TeamId == off.TeamId && x.LeagueId == leagueId).ToListAsync();
                        int rosterCountChk = rosterSpotChk.Count;

                        if (rosterCountChk < 15)
                        {
                            // Now need to check if the team can still afford the player
                            var teamSalary = await _context.TeamSalaryCaps.FirstOrDefaultAsync(x => x.TeamId == off.TeamId && x.LeagueId == leagueId);
                            var cap = await _context.SalaryCaps.FirstOrDefaultAsync(x => x.LeagueId == leagueId);
                            if ((cap.Cap - (teamSalary.CurrentCapAmount - off.YearOne)) > off.YearOne || off.YearOne == 1000000)
                            {
                                // Then can sign
                                int offerTotalMoney = off.YearOne + off.YearTwo + off.YearThree + off.YearFour + off.YearFive;
                                int offerYears = GetContractYears(off);

                                int acceptedTotalMoney = 0;
                                int acceptedYears = 0;
                                if (acceptedOffer != null)
                                {
                                    acceptedTotalMoney = acceptedOffer.YearOne + acceptedOffer.YearTwo + acceptedOffer.YearThree + acceptedOffer.YearFour + acceptedOffer.YearFive;
                                    acceptedYears = GetContractYears(acceptedOffer);
                                }

                                if (acceptedOffer == null)
                                {
                                    acceptedOffer = off;
                                }
                                else
                                {
                                    // Now need to check if these are the same amount of guarenteed years
                                    int offGuarenteed = GetGuarenteedCount(off);
                                    int accGuarenteed = GetGuarenteedCount(acceptedOffer);

                                    if (offerYears > acceptedYears)
                                    {
                                        int yearDifference = offerYears - acceptedYears;
                                        int amountDifference = offerTotalMoney - acceptedTotalMoney;

                                        decimal offerValue = amountDifference / yearDifference;
                                        decimal acceptValue = acceptedTotalMoney / acceptedYears;

                                        if (offerValue > acceptValue)
                                        {
                                            acceptedOffer = off;
                                        }
                                        else if (offerValue == acceptValue)
                                        {
                                            if (offGuarenteed > (accGuarenteed))
                                            {
                                                acceptedOffer = off;
                                            }

                                        }
                                    }
                                    else if (offerYears == acceptedYears)
                                    {
                                        // Years are the same, so we check the money
                                        int amountDifference = offerTotalMoney - acceptedTotalMoney;

                                        if (amountDifference > 0)
                                        {
                                            acceptedOffer = off;
                                        }
                                        else if (amountDifference == 0)
                                        {
                                            if (offGuarenteed > (accGuarenteed))
                                            {
                                                acceptedOffer = off;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // The current offer is longer
                                        int yearDifference = acceptedYears - offerYears;
                                        int amountDifference = acceptedTotalMoney - offerTotalMoney;

                                        decimal acceptValue = amountDifference / yearDifference;
                                        decimal offerValue = acceptedTotalMoney / acceptedYears;

                                        if (offerValue > acceptValue)
                                        {
                                            acceptedOffer = off;
                                        }
                                        else if (offerValue == acceptValue)
                                        {
                                            if (offGuarenteed > (accGuarenteed))
                                            {
                                                acceptedOffer = off;
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    // we have only one offer - so the player will sign on
                    acceptedOffer = offers.FirstOrDefault();
                }

                if (acceptedOffer != null)
                {
                    var rosterSpot = await _context.Rosters.Where(x => x.TeamId == acceptedOffer.TeamId && x.LeagueId == leagueId).ToListAsync();
                    int rosterCount = rosterSpot.Count;

                    if (rosterCount < 15)
                    {
                        var teamSalary = await _context.TeamSalaryCaps.FirstOrDefaultAsync(x => x.TeamId == acceptedOffer.TeamId && x.LeagueId == leagueId);
                        var cap = await _context.SalaryCaps.FirstOrDefaultAsync(x => x.LeagueId == leagueId);
                        if ((cap.Cap - (teamSalary.CurrentCapAmount - acceptedOffer.YearOne) > acceptedOffer.YearOne) || acceptedOffer.YearOne == 1000000)
                        {
                            // Now setting up the details of the signing
                            // Creating the players contract
                            PlayerContract contract = new PlayerContract
                            {
                                PlayerId = playerId,
                                TeamId = acceptedOffer.TeamId,
                                YearOne = acceptedOffer.YearOne,
                                GuranteedOne = acceptedOffer.GuranteedOne,
                                YearTwo = acceptedOffer.YearTwo,
                                GuranteedTwo = acceptedOffer.GuranteedTwo,
                                YearThree = acceptedOffer.YearThree,
                                GuranteedThree = acceptedOffer.GuranteedThree,
                                YearFour = acceptedOffer.YearFour,
                                GuranteedFour = acceptedOffer.GuranteedFour,
                                YearFive = acceptedOffer.YearFive,
                                GuranteedFive = acceptedOffer.GuranteedFive,
                                TeamOption = acceptedOffer.TeamOption,
                                PlayerOption = acceptedOffer.PlayerOption,
                                LeagueId = leagueId
                            };
                            await _context.AddAsync(contract);

                            var pt = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == playerId && x.LeagueId == leagueId);
                            pt.TeamId = acceptedOffer.TeamId;
                            _context.Update(pt);

                            // Now add the player to the team roster
                            Roster roster = new Roster
                            {
                                TeamId = acceptedOffer.TeamId,
                                PlayerId = playerId,
                                LeagueId = leagueId
                            };
                            await _context.AddAsync(roster);

                            // Now need to record a transaction
                            Transaction trans = new Transaction
                            {
                                TeamId = acceptedOffer.TeamId,
                                PlayerId = playerId,
                                TransactionType = 1,
                                Day = league.Day,
                                Pick = 0,
                                PickText = "",
                                LeagueId = leagueId
                            };
                            await _context.AddAsync(trans);

                            // Now need to add inbox messages - for successful signing
                            var team = await _context.Teams.FirstOrDefaultAsync(x => x.TeamId == acceptedOffer.TeamId && x.LeagueId == leagueId);
                            var player = await _context.Players.FirstOrDefaultAsync(x => x.PlayerId == playerId && x.LeagueId == leagueId);

                            // Now need to send an inbox message
                            // DateTime date = new DateTime();
                            var dd = DateTime.Now.Day.ToString();   //.getDate(); 
                            var mm = DateTime.Now.Month.ToString();
                            var yyyy = DateTime.Now.Year.ToString();

                            InboxMessage im = new InboxMessage
                            {
                                SenderId = 0,
                                SenderName = "Admin",
                                SenderTeam = "System",
                                ReceiverId = acceptedOffer.TeamId,
                                ReceiverName = team.Mascot,
                                ReceiverTeam = team.Mascot,
                                Subject = player.FirstName + " " + player.Surname + " has signed with your team",
                                Body = player.FirstName + " " + player.Surname + " has signed your offered contract with your team. They are now available on your roster.",
                                MessageDate = dd + "/" + mm + "/" + yyyy,
                                IsNew = 1,
                                LeagueId = leagueId
                            };
                            await _context.AddAsync(im);

                            // Now need to update any other teams who were trying to sign the player
                            if (offers.Count > 1)
                            {
                                foreach (var off in offers)
                                {
                                    if (off.TeamId != acceptedOffer.TeamId)
                                    {
                                        var t = await _context.Teams.FirstOrDefaultAsync(x => x.TeamId == off.TeamId && x.LeagueId == leagueId);
                                        InboxMessage rejectMessage = new InboxMessage
                                        {
                                            SenderId = 0,
                                            SenderName = "Admin",
                                            SenderTeam = "System",
                                            ReceiverId = t.TeamId,
                                            ReceiverName = t.Mascot,
                                            ReceiverTeam = t.Mascot,
                                            Subject = player.FirstName + " " + player.Surname + " has rejected your offer",
                                            Body = player.FirstName + " " + player.Surname + " has rejected your offer and signed with another team.",
                                            MessageDate = dd + "/" + mm + "/" + yyyy,
                                            IsNew = 1,
                                            LeagueId = leagueId
                                        };
                                        await _context.AddAsync(rejectMessage);

                                        // Need to put the salary back on the teams salary cap
                                        var capUpdate = await _context.TeamSalaryCaps.FirstOrDefaultAsync(x => x.LeagueId == off.LeagueId && x.TeamId == off.TeamId);
                                        capUpdate.CurrentCapAmount = capUpdate.CurrentCapAmount - off.YearOne;
                                        _context.Update(capUpdate);
                                    }
                                    _context.Remove(off);
                                }
                            }

                            // Now to remove the free agency decision
                            _context.Remove(fa);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }
            return true;
        }

        public int GetActualContractYears(PlayerContract offer)
        {
            if (offer.YearFive > 0)
            {
                return 5;
            }
            else if (offer.YearFour > 0)
            {
                return 4;
            }
            else if (offer.YearThree > 0)
            {
                return 3;
            }
            else if (offer.YearTwo > 0)
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }

        public async Task<bool> CreateNewLeague(League league)
        {
            try
            {
                var leagueName = new SqlParameter("@leagueName", league.LeagueName);
                var seasonId = new SqlParameter("@seasonId", league.Year);
                var leagueCode = new SqlParameter("@leagueCode", league.LeagueCode);
                await _context.Database.ExecuteSqlRawAsync("exec spCreateNewLeague @leagueName, @seasonId, @leagueCode", leagueName, seasonId, leagueCode);
            }
            catch (Exception)
            {
                throw;
            }
            return true;
        }

        public int GetWaivedContractYears(WaivedContract offer)
        {
            if (offer.YearFive > 0)
            {
                return 5;
            }
            else if (offer.YearFour > 0)
            {
                return 4;
            }
            else if (offer.YearThree > 0)
            {
                return 3;
            }
            else if (offer.YearTwo > 0)
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }

        public int GetContractYears(ContractOffer offer)
        {
            if (offer.YearFive > 0)
            {
                return 5;
            }
            else if (offer.YearFour > 0)
            {
                return 4;
            }
            else if (offer.YearThree > 0)
            {
                return 3;
            }
            else if (offer.YearTwo > 0)
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }

        public int GetGuarenteedCount(ContractOffer offer)
        {
            int count = 0;

            if (offer.GuranteedFive > 0)
            {
                count++;
            }

            if (offer.GuranteedFour > 0)
            {
                count++;
            }

            if (offer.GuranteedThree > 0)
            {
                count++;
            }

            if (offer.GuranteedTwo > 0)
            {
                count++;
            }

            if (offer.GuranteedOne > 0)
            {
                count++;
            }

            return count;
        }

        public async Task<bool> DailyInjuriesUpdate(int state, int day, int leagueId)
        {
            // Need to get the lists of injuries first
            var newInjuries = await _context.PlayerInjuries.Where(x => x.CurrentlyInjured == 0 && x.StartDay == 0 && x.LeagueId == leagueId).ToListAsync();
            var existingActiveInjuries = await _context.PlayerInjuries.Where(x => x.CurrentlyInjured == 1 && x.LeagueId == leagueId).ToListAsync();

            // Now work out new injuries
            foreach (var injury in newInjuries)
            {
                int timeMissed = 0;
                int currentlyInjured = 1;
                if (injury.Severity == 1)
                {
                    timeMissed = 0;
                    currentlyInjured = 0;
                }
                else if (injury.Severity == 2)
                {
                    int tm = rng.Next(1, 1000);
                    if (tm >= 900 && tm < 950)
                    {
                        timeMissed = 1;
                        currentlyInjured = 1;
                    }
                    else if (tm >= 950)
                    {
                        timeMissed = 2;
                        currentlyInjured = 1;
                    }
                }
                else if (injury.Severity == 3)
                {
                    int tm = rng.Next(3, 16);
                    timeMissed = tm;
                    currentlyInjured = 1;
                }
                else if (injury.Severity == 4)
                {
                    int tm = rng.Next(10, 40);
                    timeMissed = tm;
                    currentlyInjured = 1;
                }
                else if (injury.Severity == 5)
                {
                    int tm = 0;
                    int rand = rng.Next(0, 100);

                    if (rand <= 70)
                    {
                        tm = rng.Next(28, 60);
                    }
                    else if (rand <= 90)
                    {
                        tm = rng.Next(60, 100);
                    }
                    else
                    {
                        tm = rng.Next(100, 180);
                    }
                    timeMissed = tm;
                    currentlyInjured = 1;
                }
                injury.StartDay = day;
                injury.TimeMissed = timeMissed;
                int endDay = day + timeMissed;
                injury.EndDay = endDay;
                injury.CurrentlyInjured = currentlyInjured;
                _context.Update(injury);
            }

            // Now to check the exisiting Injuries
            foreach (var injury in existingActiveInjuries)
            {
                if (injury.EndDay <= day)
                {
                    injury.CurrentlyInjured = 0;
                    injury.TimeMissed = 0;
                    _context.Remove(injury);
                }
                else
                {
                    int daysLeft = injury.TimeMissed - 1;
                    injury.TimeMissed = daysLeft;
                    _context.Update(injury);
                }
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CheckGamesRun(int leagueId)
        {
            int gameNotRun = 0;
            var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == leagueId);

            if (league.StateId == 6)
            {
                var todaysGames = await _context.PreseasonSchedules.Where(x => x.Day == (league.Day) && x.LeagueId == leagueId).ToListAsync();
                if (todaysGames.Count != 0)
                {
                    foreach (var game in todaysGames)
                    {
                        var gameResult = await _context.PreseasonGameResults.FirstOrDefaultAsync(x => x.GameId == game.Id && x.LeagueId == leagueId);
                        if (gameResult != null)
                        {
                            if (gameResult.Completed == 0)
                            {
                                gameNotRun = 1;
                            }
                        }
                    }
                }
            }
            else if (league.StateId == 7)
            {
                var todaysGames = await _context.Schedules.Where(x => x.GameDay == league.Day).ToListAsync();
                if (todaysGames.Count != 0)
                {
                    foreach (var game in todaysGames)
                    {
                        var gameResult = await _context.GameResults.FirstOrDefaultAsync(x => x.GameId == game.Id && x.LeagueId == leagueId);
                        if (gameResult != null)
                        {
                            if (gameResult.Completed == 0)
                            {
                                gameNotRun = 1;
                            }
                        }
                        else
                        {
                            gameNotRun = 1;
                        }
                    }
                }
            }
            else if (league.StateId == 8 || league.StateId == 9 || league.StateId == 10 || league.StateId == 11)
            {
                var todaysGames = await _context.SchedulesPlayoffs.Where(x => x.GameDay == (league.Day) && x.LeagueId == leagueId).ToListAsync();
                if (todaysGames.Count != 0)
                {
                    foreach (var game in todaysGames)
                    {
                        var gameResult = await _context.PlayoffResults.FirstOrDefaultAsync(x => x.GameId == game.Id && x.LeagueId == leagueId);
                        if (gameResult != null)
                        {
                            if (gameResult.Completed == 0)
                            {
                                gameNotRun = 1;
                            }
                        }
                        else
                        {
                            gameNotRun = 1;
                        }
                    }
                }
            }

            if (gameNotRun == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> ChangeDay(int day, int leagueId)
        {
            var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == leagueId);
            league.Day = day;
            _context.Update(league);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> BeginPlayoffs(int leagueId)
        {
            // Change the League State Id to 8
            await UpdateLeagueState(8, leagueId);

            var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == leagueId);
            // Need to check the injuries and update appropriately
            await DailyInjuriesUpdate(league.StateId, league.Day, league.Id);

            // Create the PlayOff Series for Round 1
            // Get the standings and set up the lists
            var leagueStandings = await _context.Standings.Where(x => x.LeagueId == leagueId).OrderByDescending(x => x.Wins).ToListAsync();
            var teams = await _context.Teams.ToListAsync();

            int westTeams = 0;
            int eastTeams = 0;
            List<int> eastTeamsList = new List<int>();
            List<int> westTeamsList = new List<int>();

            foreach (var ls in leagueStandings)
            {
                var team = teams.FirstOrDefault(x => x.TeamId == ls.TeamId && x.LeagueId == leagueId);
                if ((team.Division == 1 || team.Division == 2 || team.Division == 3) && eastTeams < 8)
                {
                    // East
                    eastTeamsList.Add(ls.TeamId);
                    eastTeams++;
                }
                else if ((team.Division == 4 || team.Division == 5 || team.Division == 6) && westTeams < 8)
                {
                    // West
                    westTeamsList.Add(ls.TeamId);
                    westTeams++;
                }
                else if (westTeams == 8 && eastTeams == 8)
                {
                    break;
                }
            }

            // Create the object and add to DB for East
            for (int i = 1; i < 5; i++)
            {
                int homeTeamId = 0;
                int awayTeamId = 0;

                if (i == 1)
                {
                    homeTeamId = eastTeamsList[0];
                    awayTeamId = eastTeamsList[7];
                }
                else if (i == 2)
                {
                    homeTeamId = eastTeamsList[1];
                    awayTeamId = eastTeamsList[6];
                }
                else if (i == 3)
                {
                    homeTeamId = eastTeamsList[2];
                    awayTeamId = eastTeamsList[5];
                }
                else if (i == 4)
                {
                    homeTeamId = eastTeamsList[3];
                    awayTeamId = eastTeamsList[4];
                }

                PlayoffSeries ps = new PlayoffSeries
                {
                    Round = 1,
                    HomeTeamId = homeTeamId,
                    AwayTeamId = awayTeamId,
                    HomeWins = 0,
                    AwayWins = 0,
                    Conference = 1,
                    LeagueId = leagueId
                };
                await _context.AddAsync(ps);
            }

            for (int i = 1; i < 5; i++)
            {
                int homeTeamId = 0;
                int awayTeamId = 0;

                if (i == 1)
                {
                    homeTeamId = westTeamsList[0];
                    awayTeamId = westTeamsList[7];
                }
                else if (i == 2)
                {
                    homeTeamId = westTeamsList[1];
                    awayTeamId = westTeamsList[6];
                }
                else if (i == 3)
                {
                    homeTeamId = westTeamsList[2];
                    awayTeamId = westTeamsList[5];
                }
                else if (i == 4)
                {
                    homeTeamId = westTeamsList[3];
                    awayTeamId = westTeamsList[4];
                }

                PlayoffSeries ps = new PlayoffSeries
                {
                    Round = 1,
                    HomeTeamId = homeTeamId,
                    AwayTeamId = awayTeamId,
                    HomeWins = 0,
                    AwayWins = 0,
                    Conference = 2,
                    LeagueId = leagueId
                };
                await _context.AddAsync(ps);
            }

            // Save the PlayOffSeries
            await _context.SaveChangesAsync();

            // Now get list of all PlayOff series for Round 1
            var allSeries = await _context.PlayoffSerieses.Where(x => x.Round == 1 && x.LeagueId == leagueId).ToListAsync();

            if (allSeries != null)
            {
                foreach (var series in allSeries)
                {
                    SchedulesPlayoff sched = new SchedulesPlayoff
                    {
                        AwayTeamId = series.AwayTeamId,
                        HomeTeamId = series.HomeTeamId,
                        SeriesId = series.Id,
                        GameDay = 218,
                        LeagueId = leagueId
                    };
                    await _context.AddAsync(sched);
                }
            }
            // Save all changes and return
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> BeginConferenceSemis(int leagueId)
        {
            // Need to check to see if the previous round has been completed
            var seriesFinished = await _context.PlayoffSerieses.Where(x => (x.AwayWins == 4 || x.HomeWins == 4) && x.LeagueId == leagueId).ToListAsync();

            if (seriesFinished.Count == 8)
            {
                // Change the League State Id to 9
                await UpdateLeagueState(9, leagueId);
                var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == leagueId);
                league.Day = 235;
                _context.Update(league);
                await _context.SaveChangesAsync();

                // Create the PlayOff Series for Round 2 - Semis
                // Get the standings and set up the lists
                var leagueStandings = await _context.Standings.Where(x => x.LeagueId == leagueId).OrderByDescending(x => x.Wins).ToListAsync();

                // First 4 will be east
                var series1 = seriesFinished[0];
                var series2 = seriesFinished[1];
                var series3 = seriesFinished[2];
                var series4 = seriesFinished[3];
                var series5 = seriesFinished[4];
                var series6 = seriesFinished[5];
                var series7 = seriesFinished[6];
                var series8 = seriesFinished[7];

                // Create the object and add to DB for East
                for (int i = 1; i < 3; i++)
                {
                    int homeTeamId = 0;
                    int awayTeamId = 0;
                    int teamOneId = 0;
                    int teamTwoId = 0;

                    if (i == 1)
                    {
                        if (series1.HomeWins == 4)
                        {
                            teamOneId = series1.HomeTeamId;
                        }
                        else
                        {
                            teamOneId = series1.AwayTeamId;
                        }

                        if (series4.HomeWins == 4)
                        {
                            teamTwoId = series4.HomeTeamId;
                        }
                        else
                        {
                            teamTwoId = series4.AwayTeamId;
                        }

                        // Need to determine who should get home court
                        var teamOneStandings = await _context.Standings.FirstOrDefaultAsync(x => x.TeamId == teamOneId && x.LeagueId == leagueId);
                        var teamTwoStandings = await _context.Standings.FirstOrDefaultAsync(x => x.TeamId == teamTwoId && x.LeagueId == leagueId);

                        if (teamOneStandings.Wins >= teamTwoStandings.Wins)
                        {
                            homeTeamId = teamOneId;
                            awayTeamId = teamTwoId;
                        }
                        else
                        {
                            homeTeamId = teamTwoId;
                            awayTeamId = teamOneId;
                        }

                        PlayoffSeries ps = new PlayoffSeries
                        {
                            Round = 2,
                            HomeTeamId = homeTeamId,
                            AwayTeamId = awayTeamId,
                            HomeWins = 0,
                            AwayWins = 0,
                            Conference = 1,
                            LeagueId = leagueId
                        };
                        await _context.AddAsync(ps);
                    }
                    else if (i == 2)
                    {
                        if (series2.HomeWins == 4)
                        {
                            teamOneId = series2.HomeTeamId;
                        }
                        else
                        {
                            teamOneId = series2.AwayTeamId;
                        }

                        if (series3.HomeWins == 4)
                        {
                            teamTwoId = series3.HomeTeamId;
                        }
                        else
                        {
                            teamTwoId = series3.AwayTeamId;
                        }

                        // Need to determine who should get home court
                        var teamOneStandings = await _context.Standings.FirstOrDefaultAsync(x => x.TeamId == teamOneId && x.LeagueId == leagueId);
                        var teamTwoStandings = await _context.Standings.FirstOrDefaultAsync(x => x.TeamId == teamTwoId && x.LeagueId == leagueId);

                        if (teamOneStandings.Wins >= teamTwoStandings.Wins)
                        {
                            homeTeamId = teamOneId;
                            awayTeamId = teamTwoId;
                        }
                        else
                        {
                            homeTeamId = teamTwoId;
                            awayTeamId = teamOneId;
                        }

                        PlayoffSeries ps = new PlayoffSeries
                        {
                            Round = 2,
                            HomeTeamId = homeTeamId,
                            AwayTeamId = awayTeamId,
                            HomeWins = 0,
                            AwayWins = 0,
                            Conference = 1,
                            LeagueId = leagueId
                        };
                        await _context.AddAsync(ps);
                    }
                }

                for (int i = 1; i < 3; i++)
                {
                    int homeTeamId = 0;
                    int awayTeamId = 0;
                    int teamOneId = 0;
                    int teamTwoId = 0;

                    if (i == 1)
                    {
                        if (series5.HomeWins == 4)
                        {
                            teamOneId = series5.HomeTeamId;
                        }
                        else
                        {
                            teamOneId = series5.AwayTeamId;
                        }

                        if (series8.HomeWins == 4)
                        {
                            teamTwoId = series8.HomeTeamId;
                        }
                        else
                        {
                            teamTwoId = series8.AwayTeamId;
                        }

                        // Need to determine who should get home court
                        var teamOneStandings = await _context.Standings.FirstOrDefaultAsync(x => x.TeamId == teamOneId && x.LeagueId == leagueId);
                        var teamTwoStandings = await _context.Standings.FirstOrDefaultAsync(x => x.TeamId == teamTwoId && x.LeagueId == leagueId);

                        if (teamOneStandings.Wins >= teamTwoStandings.Wins)
                        {
                            homeTeamId = teamOneId;
                            awayTeamId = teamTwoId;
                        }
                        else
                        {
                            homeTeamId = teamTwoId;
                            awayTeamId = teamOneId;
                        }

                        PlayoffSeries ps = new PlayoffSeries
                        {
                            Round = 2,
                            HomeTeamId = homeTeamId,
                            AwayTeamId = awayTeamId,
                            HomeWins = 0,
                            AwayWins = 0,
                            Conference = 2,
                            LeagueId = leagueId
                        };
                        await _context.AddAsync(ps);
                    }
                    else if (i == 2)
                    {
                        if (series6.HomeWins == 4)
                        {
                            teamOneId = series6.HomeTeamId;
                        }
                        else
                        {
                            teamOneId = series6.AwayTeamId;
                        }

                        if (series7.HomeWins == 4)
                        {
                            teamTwoId = series7.HomeTeamId;
                        }
                        else
                        {
                            teamTwoId = series7.AwayTeamId;
                        }

                        // Need to determine who should get home court
                        var teamOneStandings = await _context.Standings.FirstOrDefaultAsync(x => x.TeamId == teamOneId && x.LeagueId == leagueId);
                        var teamTwoStandings = await _context.Standings.FirstOrDefaultAsync(x => x.TeamId == teamTwoId && x.LeagueId == leagueId);

                        if (teamOneStandings.Wins >= teamTwoStandings.Wins)
                        {
                            homeTeamId = teamOneId;
                            awayTeamId = teamTwoId;
                        }
                        else
                        {
                            homeTeamId = teamTwoId;
                            awayTeamId = teamOneId;
                        }

                        PlayoffSeries ps = new PlayoffSeries
                        {
                            Round = 2,
                            HomeTeamId = homeTeamId,
                            AwayTeamId = awayTeamId,
                            HomeWins = 0,
                            AwayWins = 0,
                            Conference = 2,
                            LeagueId = leagueId
                        };
                        await _context.AddAsync(ps);
                    }
                }
            }

            // Save the PlayOffSeries
            await _context.SaveChangesAsync();

            // Now get list of all PlayOff series for Round 2
            var allSeries = await _context.PlayoffSerieses.Where(x => x.Round == 2 && x.LeagueId == leagueId).ToListAsync();

            if (allSeries != null)
            {
                foreach (var series in allSeries)
                {
                    SchedulesPlayoff sched = new SchedulesPlayoff
                    {
                        AwayTeamId = series.AwayTeamId,
                        HomeTeamId = series.HomeTeamId,
                        SeriesId = series.Id,
                        GameDay = 235,
                        LeagueId = leagueId
                    };
                    await _context.AddAsync(sched);
                }
            }
            // Save all changes and return
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> BeginConferenceFinals(int leagueId)
        {
            // Need to check to see if the previous round has been completed
            var seriesFinished = await _context.PlayoffSerieses.Where(x => (x.AwayWins == 4 || x.HomeWins == 4) && x.Round == 2 && x.LeagueId == leagueId).ToListAsync();

            if (seriesFinished.Count == 4)
            {
                // Change the League State Id to 10
                await UpdateLeagueState(10, leagueId);
                var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == leagueId);
                league.Day = 250;
                _context.Update(league);
                await _context.SaveChangesAsync();

                // Create the PlayOff Series for Round 3 - Conference Finals
                // Get the standings and set up the lists
                var leagueStandings = await _context.Standings.Where(x => x.LeagueId == leagueId).OrderByDescending(x => x.Wins).ToListAsync();

                // First 2 will be east
                var series1 = seriesFinished[0];
                var series2 = seriesFinished[1];
                var series3 = seriesFinished[2];
                var series4 = seriesFinished[3];

                // Create the object and add to DB for East
                int homeTeamId = 0;
                int awayTeamId = 0;
                int teamOneId = 0;
                int teamTwoId = 0;

                if (series1.HomeWins == 4)
                {
                    teamOneId = series1.HomeTeamId;
                }
                else
                {
                    teamOneId = series1.AwayTeamId;
                }

                if (series2.HomeWins == 4)
                {
                    teamTwoId = series2.HomeTeamId;
                }
                else
                {
                    teamTwoId = series2.AwayTeamId;
                }

                // Need to determine who should get home court
                var teamOneStandings = await _context.Standings.FirstOrDefaultAsync(x => x.TeamId == teamOneId && x.LeagueId == leagueId);
                var teamTwoStandings = await _context.Standings.FirstOrDefaultAsync(x => x.TeamId == teamTwoId && x.LeagueId == leagueId);

                if (teamOneStandings.Wins >= teamTwoStandings.Wins)
                {
                    homeTeamId = teamOneId;
                    awayTeamId = teamTwoId;
                }
                else
                {
                    homeTeamId = teamTwoId;
                    awayTeamId = teamOneId;
                }

                PlayoffSeries ps = new PlayoffSeries
                {
                    Round = 3,
                    HomeTeamId = homeTeamId,
                    AwayTeamId = awayTeamId,
                    HomeWins = 0,
                    AwayWins = 0,
                    Conference = 1,
                    LeagueId = leagueId
                };
                await _context.AddAsync(ps);

                homeTeamId = 0;
                awayTeamId = 0;
                teamOneId = 0;
                teamTwoId = 0;

                if (series3.HomeWins == 4)
                {
                    teamOneId = series3.HomeTeamId;
                }
                else
                {
                    teamOneId = series3.AwayTeamId;
                }

                if (series4.HomeWins == 4)
                {
                    teamTwoId = series4.HomeTeamId;
                }
                else
                {
                    teamTwoId = series4.AwayTeamId;
                }

                // Need to determine who should get home court
                teamOneStandings = await _context.Standings.FirstOrDefaultAsync(x => x.TeamId == teamOneId && x.LeagueId == leagueId);
                teamTwoStandings = await _context.Standings.FirstOrDefaultAsync(x => x.TeamId == teamTwoId && x.LeagueId == leagueId);

                if (teamOneStandings.Wins >= teamTwoStandings.Wins)
                {
                    homeTeamId = teamOneId;
                    awayTeamId = teamTwoId;
                }
                else
                {
                    homeTeamId = teamTwoId;
                    awayTeamId = teamOneId;
                }

                PlayoffSeries psTwo = new PlayoffSeries
                {
                    Round = 3,
                    HomeTeamId = homeTeamId,
                    AwayTeamId = awayTeamId,
                    HomeWins = 0,
                    AwayWins = 0,
                    Conference = 2,
                    LeagueId = leagueId
                };
                await _context.AddAsync(psTwo);
            }

            // Save the PlayOffSeries
            await _context.SaveChangesAsync();

            // Now get list of all PlayOff series for Round 2
            var allSeries = await _context.PlayoffSerieses.Where(x => x.Round == 3 && x.LeagueId == leagueId).ToListAsync();

            if (allSeries != null)
            {
                foreach (var series in allSeries)
                {
                    SchedulesPlayoff sched = new SchedulesPlayoff
                    {
                        AwayTeamId = series.AwayTeamId,
                        HomeTeamId = series.HomeTeamId,
                        SeriesId = series.Id,
                        GameDay = 250,
                        LeagueId = leagueId
                    };
                    await _context.AddAsync(sched);
                }
            }
            // Save all changes and return
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> BeginFinals(int leagueId)
        {
            // Need to check to see if the previous round has been completed
            var seriesFinished = await _context.PlayoffSerieses.Where(x => (x.AwayWins == 4 || x.HomeWins == 4) && x.Round == 3 && x.LeagueId == leagueId).ToListAsync();

            if (seriesFinished.Count == 2)
            {
                // Change the League State Id to 10
                await UpdateLeagueState(11, leagueId);
                var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == leagueId);
                league.Day = 267;
                _context.Update(league);
                await _context.SaveChangesAsync();

                // Create the PlayOff Series for Round 3 - Conference Finals
                // Get the standings and set up the lists
                var leagueStandings = await _context.Standings.Where(x => x.LeagueId == leagueId).OrderByDescending(x => x.Wins).ToListAsync();

                // First 2 will be east
                var series1 = seriesFinished[0];
                var series2 = seriesFinished[1];

                // Create the object and add to DB for East
                int homeTeamId = 0;
                int awayTeamId = 0;
                int teamOneId = 0;
                int teamTwoId = 0;

                if (series1.HomeWins == 4)
                {
                    teamOneId = series1.HomeTeamId;
                }
                else
                {
                    teamOneId = series1.AwayTeamId;
                }

                if (series2.HomeWins == 4)
                {
                    teamTwoId = series2.HomeTeamId;
                }
                else
                {
                    teamTwoId = series2.AwayTeamId;
                }

                // Need to determine who should get home court
                var teamOneStandings = await _context.Standings.FirstOrDefaultAsync(x => x.TeamId == teamOneId && x.LeagueId == leagueId);
                var teamTwoStandings = await _context.Standings.FirstOrDefaultAsync(x => x.TeamId == teamTwoId && x.LeagueId == leagueId);

                if (teamOneStandings.Wins >= teamTwoStandings.Wins)
                {
                    homeTeamId = teamOneId;
                    awayTeamId = teamTwoId;
                }
                else
                {
                    homeTeamId = teamTwoId;
                    awayTeamId = teamOneId;
                }

                PlayoffSeries ps = new PlayoffSeries
                {
                    Round = 4,
                    HomeTeamId = homeTeamId,
                    AwayTeamId = awayTeamId,
                    HomeWins = 0,
                    AwayWins = 0,
                    Conference = 0,
                    LeagueId = leagueId
                };
                await _context.AddAsync(ps);
            }

            // Save the PlayOffSeries
            await _context.SaveChangesAsync();

            // Now get list of all PlayOff series for Round 2
            var allSeries = await _context.PlayoffSerieses.Where(x => x.Round == 4 && x.LeagueId == leagueId).ToListAsync();

            if (allSeries != null)
            {
                foreach (var series in allSeries)
                {
                    SchedulesPlayoff sched = new SchedulesPlayoff
                    {
                        AwayTeamId = series.AwayTeamId,
                        HomeTeamId = series.HomeTeamId,
                        SeriesId = series.Id,
                        GameDay = 267,
                        LeagueId = leagueId
                    };
                    await _context.AddAsync(sched);
                }
            }
            // Save all changes and return
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> GenerateAutoPickOrder(int leagueId)
        {
            // League league = 
            var autoCount = await _context.AutoPickOrders.Where(x => x.LeagueId == leagueId).LongCountAsync();
            if (autoCount == 0)
            {
                var playerRatings = await _context.PlayerRatings.Where(x => x.LeagueId == leagueId).ToListAsync();
                foreach (var player in playerRatings)
                {
                    int twoRating = (int)player.TwoRating / 3;
                    int threeRating = (int)player.ThreeRating / 2;
                    int ftRating = (int)player.FTRating / 4;
                    int orebRating = player.ORebRating;
                    int drebRating = player.DRebRating;
                    int astRating = player.PassAssistRating;
                    int ast2Rating = player.AssitRating / 3;
                    int stealRating = player.StealRating;
                    int blockRating = player.BlockRating;
                    int orpm = (int)player.ORPMRating;
                    int drpm = (int)player.DRPMRating;
                    int staminaRating = 100 - player.StaminaRating;
                    int foulRating = 100 - player.FoulingRating;

                    // Need to get the players age
                    var p = await _context.Players.FirstOrDefaultAsync(x => x.PlayerId == player.PlayerId);

                    int firstScore = twoRating + threeRating + ftRating + orebRating + drebRating + ast2Rating + astRating + stealRating + blockRating + staminaRating + orpm + drpm + foulRating;
                    int finalScore = (firstScore - (ftRating) + ((100 - p.Age) * 5));

                    AutoPickOrder apo = new AutoPickOrder
                    {
                        PlayerId = player.PlayerId,
                        Score = finalScore,
                        LeagueId = leagueId
                    };
                    await _context.AddAsync(apo);
                }
            }
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<RolloverStatus> GetRolloverStatus(int leagueId)
        {
            var ros = await _context.RolloverStatuses.FirstOrDefaultAsync(x => x.LeagueID == leagueId);
            return ros;
        }

        public async Task<IEnumerable<VotesDto>> GetAllNBATeams(int leagueId)
        {
            List<VotesDto> allFirstTeam = new List<VotesDto>();
            List<VotesDto> allSecondTeam = new List<VotesDto>();
            List<VotesDto> allThirdTeam = new List<VotesDto>();
            List<VotesDto> allTeams = new List<VotesDto>();
            List<int> addedPlayers = new List<int>();
            var votes = await _context.MvpVoting.Where(x => x.LeagueId == leagueId).OrderByDescending(x => x.Votes).ToListAsync();

            // Point Guard
            int pgCount = 0;
            foreach (var vote in votes)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.PlayerId == vote.PlayerId && x.LeagueId == leagueId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == vote.PlayerId && x.LeagueId == leagueId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.TeamId == playerTeam.TeamId && x.LeagueId == leagueId);

                if (player.PGPosition == 1)
                {
                    var added = addedPlayers.Contains(player.PlayerId);

                    if (!added)
                    {
                        VotesDto dto = new VotesDto
                        {
                            PlayerId = vote.PlayerId,
                            PlayerName = player.FirstName + " " + player.Surname,
                            TeamName = team.ShortCode,
                            Votes = vote.Votes
                        };

                        if (pgCount == 0)
                        {
                            allFirstTeam.Add(dto);
                        }

                        if (pgCount == 1)
                        {
                            allSecondTeam.Add(dto);
                        }

                        if (pgCount == 2)
                        {
                            allThirdTeam.Add(dto);
                        }
                        addedPlayers.Add(dto.PlayerId);
                        pgCount++;

                        if (pgCount == 3)
                        {
                            break;
                        }
                    }
                }
            }

            // Shooting Guard
            int sgCount = 0;
            foreach (var vote in votes)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.PlayerId == vote.PlayerId && x.LeagueId == leagueId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == vote.PlayerId && x.LeagueId == leagueId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.TeamId == playerTeam.TeamId && x.LeagueId == leagueId);

                if (player.SGPosition == 1)
                {
                    var added = addedPlayers.Contains(player.PlayerId);

                    if (!added)
                    {
                        VotesDto dto = new VotesDto
                        {
                            PlayerId = vote.PlayerId,
                            PlayerName = player.FirstName + " " + player.Surname,
                            TeamName = team.ShortCode,
                            Votes = vote.Votes
                        };

                        if (sgCount == 0)
                        {
                            allFirstTeam.Add(dto);
                        }

                        if (sgCount == 1)
                        {
                            allSecondTeam.Add(dto);
                        }

                        if (sgCount == 2)
                        {
                            allThirdTeam.Add(dto);
                        }
                        addedPlayers.Add(dto.PlayerId);
                        sgCount++;

                        if (sgCount == 3)
                        {
                            break;
                        }
                    }
                }
            }

            // Small Forward
            int sfCount = 0;
            foreach (var vote in votes)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.PlayerId == vote.PlayerId && x.LeagueId == leagueId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == vote.PlayerId && x.LeagueId == leagueId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.TeamId == playerTeam.TeamId && x.LeagueId == leagueId);

                if (player.SFPosition == 1)
                {
                    var added = addedPlayers.Contains(player.PlayerId);

                    if (!added)
                    {
                        VotesDto dto = new VotesDto
                        {
                            PlayerId = vote.PlayerId,
                            PlayerName = player.FirstName + " " + player.Surname,
                            TeamName = team.ShortCode,
                            Votes = vote.Votes
                        };

                        if (sfCount == 0)
                        {
                            allFirstTeam.Add(dto);
                        }

                        if (sfCount == 1)
                        {
                            allSecondTeam.Add(dto);
                        }

                        if (sfCount == 2)
                        {
                            allThirdTeam.Add(dto);
                        }
                        addedPlayers.Add(dto.PlayerId);
                        sfCount++;

                        if (sfCount == 3)
                        {
                            break;
                        }
                    }
                }
            }

            // Power Forward
            int pfCount = 0;
            foreach (var vote in votes)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.PlayerId == vote.PlayerId && x.LeagueId == leagueId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == vote.PlayerId && x.LeagueId == leagueId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.TeamId == playerTeam.TeamId && x.LeagueId == leagueId);

                if (player.PFPosition == 1)
                {
                    var added = addedPlayers.Contains(player.PlayerId);

                    if (!added)
                    {
                        VotesDto dto = new VotesDto
                        {
                            PlayerId = vote.PlayerId,
                            PlayerName = player.FirstName + " " + player.Surname,
                            TeamName = team.ShortCode,
                            Votes = vote.Votes
                        };

                        if (pfCount == 0)
                        {
                            allFirstTeam.Add(dto);
                        }

                        if (pfCount == 1)
                        {
                            allSecondTeam.Add(dto);
                        }

                        if (pfCount == 2)
                        {
                            allThirdTeam.Add(dto);
                        }
                        addedPlayers.Add(dto.PlayerId);
                        pfCount++;

                        if (pfCount == 3)
                        {
                            break;
                        }
                    }
                }
            }

            // Centre
            int cCount = 0;
            foreach (var vote in votes)
            {
                var player = await _context.Players.FirstOrDefaultAsync(x => x.PlayerId == vote.PlayerId && x.LeagueId == leagueId);
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == vote.PlayerId && x.LeagueId == leagueId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.TeamId == playerTeam.TeamId && x.LeagueId == leagueId);

                if (player.CPosition == 1)
                {
                    var added = addedPlayers.Contains(player.PlayerId);

                    if (!added)
                    {
                        VotesDto dto = new VotesDto
                        {
                            PlayerId = vote.PlayerId,
                            PlayerName = player.FirstName + " " + player.Surname,
                            TeamName = team.ShortCode,
                            Votes = vote.Votes
                        };

                        if (cCount == 0)
                        {
                            allFirstTeam.Add(dto);
                        }

                        if (cCount == 1)
                        {
                            allSecondTeam.Add(dto);
                        }

                        if (cCount == 2)
                        {
                            allThirdTeam.Add(dto);
                        }
                        addedPlayers.Add(dto.PlayerId);
                        cCount++;

                        if (cCount == 3)
                        {
                            break;
                        }
                    }
                }
            }

            // Now to add the 3 lists together in apprporiate order
            foreach (var item in allFirstTeam)
            {
                allTeams.Add(item);
            }

            foreach (var item in allSecondTeam)
            {
                allTeams.Add(item);
            }

            foreach (var item in allThirdTeam)
            {
                allTeams.Add(item);
            }
            return allTeams;
        }

        public async Task<bool> SaveSeasonResults(int leagueId)
        {
            var ros = await _context.RolloverStatuses.FirstOrDefaultAsync(x => x.LeagueID == leagueId);
            var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == leagueId);


            if (ros.RollOverStatus == 0)
            {
                // Get the champion and runner up
                var series = await _context.PlayoffSerieses.FirstOrDefaultAsync(x => x.Round == 4 && x.LeagueId == leagueId);

                int championId = 0;
                int runnerupId = 0;
                if (series.HomeWins == 4)
                {
                    championId = series.HomeTeamId;
                    runnerupId = series.AwayTeamId;
                }
                else
                {
                    championId = series.AwayTeamId;
                    runnerupId = series.HomeTeamId;
                }

                // Get the award winner details
                var mvp = await _context.MvpVoting.Where(x => x.LeagueId == leagueId).OrderByDescending(x => x.Votes).FirstOrDefaultAsync();
                var mvpPlayerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == mvp.PlayerId && x.LeagueId == leagueId);
                var mvpPlayer = await _context.Players.FirstOrDefaultAsync(x => x.PlayerId == mvp.PlayerId && x.LeagueId == leagueId);

                var dpoy = await _context.DpoyVoting.Where(x => x.LeagueId == leagueId).OrderByDescending(x => x.Votes).FirstOrDefaultAsync();
                var dpoyPlayerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == dpoy.PlayerId && x.LeagueId == leagueId);
                var dpoyPlayer = await _context.Players.FirstOrDefaultAsync(x => x.PlayerId == dpoy.PlayerId && x.LeagueId == leagueId);

                var sixth = await _context.SixthManVoting.Where(x => x.LeagueId == leagueId).OrderByDescending(x => x.Votes).FirstOrDefaultAsync();
                var sixthPlayerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == sixth.PlayerId && x.LeagueId == leagueId);
                var sixthPlayer = await _context.Players.FirstOrDefaultAsync(x => x.PlayerId == sixth.PlayerId && x.LeagueId == leagueId);

                // All-NBA Teams
                var allNbaTeams = await GetAllNBATeams(leagueId);
                int counter = 0;
                string firstTeamPGName = "";
                int firstTeamPGTeamId = 0;
                string firstTeamSGName = "";
                int firstTeamSGTeamId = 0;
                string firstTeamSFName = "";
                int firstTeamSFTeamId = 0;
                string firstTeamPFName = "";
                int firstTeamPFTeamId = 0;
                string firstTeamCName = "";
                int firstTeamCTeamId = 0;
                string secondTeamPGName = "";
                int secondTeamPGTeamId = 0;
                string secondTeamSGName = "";
                int secondTeamSGTeamId = 0;
                string secondTeamSFName = "";
                int secondTeamSFTeamId = 0;
                string secondTeamPFName = "";
                int secondTeamPFTeamId = 0;
                string secondTeamCName = "";
                int secondTeamCTeamId = 0;
                string thirdTeamPGName = "";
                int thirdTeamPGTeamId = 0;
                string thirdTeamSGName = "";
                int thirdTeamSGTeamId = 0;
                string thirdTeamSFName = "";
                int thirdTeamSFTeamId = 0;
                string thirdTeamPFName = "";
                int thirdTeamPFTeamId = 0;
                string thirdTeamCName = "";
                int thirdTeamCTeamId = 0;

                foreach (var allnba in allNbaTeams)
                {
                    counter++;
                    switch (counter)
                    {
                        case 1:
                            firstTeamPGName = allnba.PlayerName;
                            var firstPGTeam = await _context.Teams.FirstOrDefaultAsync(x => x.LeagueId == leagueId && x.ShortCode == allnba.TeamName);
                            firstTeamPGTeamId = firstPGTeam.TeamId;
                            break;
                        case 2:
                            firstTeamSGName = allnba.PlayerName;
                            var firstSGTeam = await _context.Teams.FirstOrDefaultAsync(x => x.LeagueId == leagueId && x.ShortCode == allnba.TeamName);
                            firstTeamSGTeamId = firstSGTeam.TeamId;
                            break;
                        case 3:
                            firstTeamSFName = allnba.PlayerName;
                            var firstSFTeam = await _context.Teams.FirstOrDefaultAsync(x => x.LeagueId == leagueId && x.ShortCode == allnba.TeamName);
                            firstTeamSFTeamId = firstSFTeam.TeamId;
                            break;
                        case 4:
                            firstTeamPFName = allnba.PlayerName;
                            var firstPFTeam = await _context.Teams.FirstOrDefaultAsync(x => x.LeagueId == leagueId && x.ShortCode == allnba.TeamName);
                            firstTeamPFTeamId = firstPFTeam.TeamId;
                            break;
                        case 5:
                            firstTeamCName = allnba.PlayerName;
                            var firstCTeam = await _context.Teams.FirstOrDefaultAsync(x => x.LeagueId == leagueId && x.ShortCode == allnba.TeamName);
                            firstTeamCTeamId = firstCTeam.TeamId;
                            break;
                        case 6:
                            secondTeamPGName = allnba.PlayerName;
                            var secondPGTeam = await _context.Teams.FirstOrDefaultAsync(x => x.LeagueId == leagueId && x.ShortCode == allnba.TeamName);
                            secondTeamPGTeamId = secondPGTeam.TeamId;
                            break;
                        case 7:
                            secondTeamSGName = allnba.PlayerName;
                            var secondSGTeam = await _context.Teams.FirstOrDefaultAsync(x => x.LeagueId == leagueId && x.ShortCode == allnba.TeamName);
                            secondTeamSGTeamId = secondSGTeam.TeamId;
                            break;
                        case 8:
                            secondTeamSFName = allnba.PlayerName;
                            var secondSFTeam = await _context.Teams.FirstOrDefaultAsync(x => x.LeagueId == leagueId && x.ShortCode == allnba.TeamName);
                            secondTeamSFTeamId = secondSFTeam.TeamId;
                            break;
                        case 9:
                            secondTeamPFName = allnba.PlayerName;
                            var secondPFTeam = await _context.Teams.FirstOrDefaultAsync(x => x.LeagueId == leagueId && x.ShortCode == allnba.TeamName);
                            secondTeamPFTeamId = secondPFTeam.TeamId;
                            break;
                        case 10:
                            secondTeamCName = allnba.PlayerName;
                            var secondCTeam = await _context.Teams.FirstOrDefaultAsync(x => x.LeagueId == leagueId && x.ShortCode == allnba.TeamName);
                            secondTeamCTeamId = secondCTeam.TeamId;
                            break;
                        case 11:
                            thirdTeamPGName = allnba.PlayerName;
                            var thirdPGTeam = await _context.Teams.FirstOrDefaultAsync(x => x.LeagueId == leagueId && x.ShortCode == allnba.TeamName);
                            thirdTeamPGTeamId = thirdPGTeam.TeamId;
                            break;
                        case 12:
                            thirdTeamSGName = allnba.PlayerName;
                            var thirdSGTeam = await _context.Teams.FirstOrDefaultAsync(x => x.LeagueId == leagueId && x.ShortCode == allnba.TeamName);
                            thirdTeamSGTeamId = thirdSGTeam.TeamId;
                            break;
                        case 13:
                            thirdTeamSFName = allnba.PlayerName;
                            var thirdSFTeam = await _context.Teams.FirstOrDefaultAsync(x => x.LeagueId == leagueId && x.ShortCode == allnba.TeamName);
                            thirdTeamSFTeamId = thirdSFTeam.TeamId;
                            break;
                        case 14:
                            thirdTeamPFName = allnba.PlayerName;
                            var thirdPFTeam = await _context.Teams.FirstOrDefaultAsync(x => x.LeagueId == leagueId && x.ShortCode == allnba.TeamName);
                            thirdTeamPFTeamId = thirdPFTeam.TeamId;
                            break;
                        case 15:
                            thirdTeamCName = allnba.PlayerName;
                            var thirdCTeam = await _context.Teams.FirstOrDefaultAsync(x => x.LeagueId == leagueId && x.ShortCode == allnba.TeamName);
                            thirdTeamCTeamId = thirdCTeam.TeamId;
                            break;
                        default:
                            break;
                    }
                }

                // League History
                LeagueHistory history = new LeagueHistory
                {
                    LeagueId = leagueId,
                    SeasonId = league.Year,
                    ChampionsId = championId,
                    RunnerUpId = runnerupId,
                    DpoyPlayer = dpoyPlayer.FirstName + " " + dpoyPlayer.Surname,
                    DpoyTeamId = dpoyPlayerTeam.TeamId,
                    MVPPlayer = mvpPlayer.FirstName + " " + mvpPlayer.Surname,
                    MvpTeamId = mvpPlayerTeam.TeamId,
                    SixthPlayer = sixthPlayer.FirstName + " " + sixthPlayer.Surname,
                    SixthTeamId = sixthPlayerTeam.TeamId,
                    FirstTeamPGPlayer = firstTeamPGName,
                    FirstTeamPGTeamId = firstTeamPGTeamId,
                    FirstTeamSGPlayer = firstTeamSGName,
                    FirstTeamSGTeamId = firstTeamSGTeamId,
                    FirstTeamSFPlayer = firstTeamSFName,
                    FirstTeamSFTeamId = firstTeamSFTeamId,
                    FirstTeamPFPlayer = firstTeamPFName,
                    FirstTeamPFTeamId = firstTeamPFTeamId,
                    FirstTeamCPlayer = firstTeamCName,
                    FirstTeamCTeamId = firstTeamCTeamId,
                    SecondTeamPGPlayer = secondTeamPGName,
                    SecondTeamPGTeamId = secondTeamPGTeamId,
                    SecondTeamSGPlayer = secondTeamSGName,
                    SecondTeamSGTeamId = secondTeamSGTeamId,
                    SecondTeamSFPlayer = secondTeamSFName,
                    SecondTeamSFTeamId = secondTeamSFTeamId,
                    SecondTeamPFPlayer = secondTeamPFName,
                    SecondTeamPFTeamId = secondTeamPFTeamId,
                    SecondTeamCPlayer = secondTeamCName,
                    SecondTeamCTeamId = secondTeamCTeamId,
                    ThirdTeamPGPlayer = thirdTeamPGName,
                    ThirdTeamPGTeamId = thirdTeamPGTeamId,
                    ThirdTeamSGPlayer = thirdTeamSGName,
                    ThirdTeamSGTeamId = thirdTeamSGTeamId,
                    ThirdTeamSFPlayer = thirdTeamSFName,
                    ThirdTeamSFTeamId = thirdTeamSFTeamId,
                    ThirdTeamPFPlayer = thirdTeamPFName,
                    ThirdTeamPFTeamId = thirdTeamPFTeamId,
                    ThirdTeamCPlayer = thirdTeamCName,
                    ThirdTeamCTeamId = thirdTeamCTeamId
                };
                await _context.AddAsync(history);
                ros.RollOverStatus = 1;
                _context.Update(ros);
                await _context.SaveChangesAsync();
            }

            if (ros.RollOverStatus == 1)
            {
                // Player Stats
                var seasonId = new SqlParameter("@seasonId", league.Year);
                var lid = new SqlParameter("@leagueId", league.Id);
                await _context.Database.ExecuteSqlRawAsync("exec spRolloverPlayerStatsSeason @leagueId, @seasonId", lid, seasonId);

                // Player Stats move into career
                await _context.Database.ExecuteSqlRawAsync("exec spRolloverPlayerStatsPlayoffs @leagueId, @seasonId", lid, seasonId);
                
                ros.RollOverStatus = 2;
                _context.Update(ros);
                await _context.SaveChangesAsync();
            }

            if (ros.RollOverStatus == 2)
            {
                var teams = await _context.Teams.Where(x => x.LeagueId == leagueId).ToListAsync();

                foreach (var team in teams)
                {
                    var record = await _context.Standings.FirstOrDefaultAsync(x => x.TeamId == team.TeamId && x.LeagueId == leagueId);

                    // Team History
                    TeamHistory history = new TeamHistory
                    {
                        LeagueId = leagueId,
                        SeasonId = league.Year,
                        TeamId = team.TeamId,
                        Wins = record.Wins,
                        Losses = record.Losses
                    };
                    await _context.AddAsync(history);
                }
                await _context.SaveChangesAsync();
                ros.RollOverStatus = 3;
                _context.Update(ros);
                await _context.SaveChangesAsync();
            }

            if (ros.RollOverStatus == 3)
            {
                // Playoff Series History
                var serieses = await _context.PlayoffSerieses.Where(x => x.LeagueId == leagueId).ToListAsync();
                foreach (var series in serieses)
                {
                    PlayoffSeriesHistory history = new PlayoffSeriesHistory
                    {
                        LeagueId = leagueId,
                        SeasonId = league.Year,
                        Round = series.Round,
                        HomeTeamId = series.HomeTeamId,
                        AwayTeamId = series.AwayTeamId,
                        HomeWins = series.HomeWins,
                        AwayWins = series.AwayWins,
                        Conference = series.Conference
                    };
                    await _context.AddAsync(history);
                }
                await _context.SaveChangesAsync();
                ros.RollOverStatus = 4;
                _context.Update(ros);
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<bool> RolloverDeletePlayerData(int leagueId)
        {
            var ros = await _context.RolloverStatuses.FirstOrDefaultAsync(x => x.LeagueID == leagueId);

            if (ros.RollOverStatus == 4) {
                // Need to handle updated injuries over the season
                var injuries = await _context.PlayerInjuries.Where(x => x.LeagueId == leagueId).ToListAsync();
                foreach (var injury in injuries)
                {
                    if(injury.TimeMissed > 10)
                    {
                        injury.TimeMissed = injury.TimeMissed - 10;
                        _context.Update(injury);
                    }
                    else
                    {
                        _context.Remove(injury);
                    }
                }
                await _context.SaveChangesAsync();

                var playerStatsToRemove = await _context.PlayerStats.Where(x => x.LeagueId == leagueId).ToListAsync();
                foreach (var stats in playerStatsToRemove)
                {
                    _context.PlayerStats.Remove(stats);
                }
                await _context.SaveChangesAsync();

                ros.RollOverStatus = 5;
                _context.Update(ros);
                await _context.SaveChangesAsync();
            }

            if (ros.RollOverStatus == 5) {
                var playerStatsToRemove = await _context.PlayerStatsPlayoffs.Where(x => x.LeagueId == leagueId).ToListAsync();
                foreach (var stats in playerStatsToRemove)
                {
                    _context.PlayerStatsPlayoffs.Remove(stats);
                }
                await _context.SaveChangesAsync();

                ros.RollOverStatus = 6;
                _context.Update(ros);
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<bool> RolloverDeletePlayByPlaySeason(int leagueId)
        {
            var ros = await _context.RolloverStatuses.FirstOrDefaultAsync(x => x.LeagueID == leagueId);

            if (ros.RollOverStatus == 6) 
            {
                var playbyplays = await _context.PlayByPlays.Where(x => x.LeagueId == leagueId).ToListAsync();
                int count = playbyplays.Count;

                foreach (var pbp in playbyplays)
                {
                    _context.PlayByPlays.Remove(pbp);
                }
                await _context.SaveChangesAsync();

                ros.RollOverStatus = 7;
                _context.Update(ros);
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<bool> RolloverDeletePlayByPlayPlayoffs(int leagueId)
        {
            var ros = await _context.RolloverStatuses.FirstOrDefaultAsync(x => x.LeagueID == leagueId);
            if (ros.RollOverStatus == 7)
            {
                var playbyplays = await _context.PlayByPlayPlayoffs.Where(x => x.LeagueId == leagueId).ToListAsync();
                int count = playbyplays.Count;

                foreach (var pbp in playbyplays)
                {
                    _context.PlayByPlayPlayoffs.Remove(pbp);
                }
                await _context.SaveChangesAsync();

                ros.RollOverStatus = 8;
                _context.Update(ros);
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<bool> RolloverDeletePlayoffSerieses(int leagueId)
        {
            var ros = await _context.RolloverStatuses.FirstOrDefaultAsync(x => x.LeagueID == leagueId);
            if (ros.RollOverStatus == 8)
            {
                var serieses = await _context.PlayoffSerieses.Where(x => x.LeagueId == leagueId).ToListAsync();
                foreach (var ps in serieses)
                {
                    _context.PlayoffSerieses.Remove(ps);
                }
                await _context.SaveChangesAsync();

                ros.RollOverStatus = 9;
                _context.Update(ros);
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<bool> RolloverDeleteGameResults(int leagueId)
        {
            var ros = await _context.RolloverStatuses.FirstOrDefaultAsync(x => x.LeagueID == leagueId);
            if (ros.RollOverStatus == 9)
            {
                var results = await _context.GameResults.Where(x => x.LeagueId == leagueId).ToListAsync();
                foreach (var gr in results)
                {
                    _context.GameResults.Remove(gr);
                }
                await _context.SaveChangesAsync();

                ros.RollOverStatus = 10;
                _context.Update(ros);
                await _context.SaveChangesAsync();
            }

            if (ros.RollOverStatus == 10)
            {
                var results = await _context.PlayoffResults.Where(x => x.LeagueId == leagueId).ToListAsync();
                foreach (var pr in results)
                {
                    _context.PlayoffResults.Remove(pr);
                }
                await _context.SaveChangesAsync();

                ros.RollOverStatus = 11;
                _context.Update(ros);
                await _context.SaveChangesAsync();
            }

            if (ros.RollOverStatus == 11)
            {
                var results = await _context.PreseasonGameResults.Where(x => x.LeagueId == leagueId).ToListAsync();
                foreach (var pr in results)
                {
                    _context.PreseasonGameResults.Remove(pr);
                }
                await _context.SaveChangesAsync();

                ros.RollOverStatus = 12;
                _context.Update(ros);
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<bool> RolloverDeleteBoxScores(int leagueId)
        {
            var ros = await _context.RolloverStatuses.FirstOrDefaultAsync(x => x.LeagueID == leagueId);
            if (ros.RollOverStatus == 12)
            {
                var boxScores = await _context.GameBoxScores.Where(x => x.LeagueId == leagueId).ToListAsync();
                foreach (var bs in boxScores)
                {
                    _context.GameBoxScores.Remove(bs);
                }
                await _context.SaveChangesAsync();

                ros.RollOverStatus = 13;
                _context.Update(ros);
                await _context.SaveChangesAsync();
            }

            if (ros.RollOverStatus == 13)
            {
                var boxScores = await _context.PlayoffBoxScores.Where(x => x.LeagueId == leagueId).ToListAsync();
                foreach (var bs in boxScores)
                {
                    _context.PlayoffBoxScores.Remove(bs);
                }
                await _context.SaveChangesAsync();

                ros.RollOverStatus = 14;
                _context.Update(ros);
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<bool> RolloverDeleteAwards(int leagueId)
        {
            var ros = await _context.RolloverStatuses.FirstOrDefaultAsync(x => x.LeagueID == leagueId);
            if (ros.RollOverStatus == 14)
            {
                var mvps = await _context.MvpVoting.Where(x => x.LeagueId == leagueId).ToListAsync();
                foreach (var mvp in mvps)
                {
                    _context.MvpVoting.Remove(mvp);
                }
                await _context.SaveChangesAsync();

                ros.RollOverStatus = 15;
                _context.Update(ros);
                await _context.SaveChangesAsync();
            }

            if (ros.RollOverStatus == 15)
            {
                var dpoys = await _context.DpoyVoting.Where(x => x.LeagueId == leagueId).ToListAsync();
                foreach (var dpoy in dpoys)
                {
                    _context.DpoyVoting.Remove(dpoy);
                }
                await _context.SaveChangesAsync();

                ros.RollOverStatus = 16;
                _context.Update(ros);
                await _context.SaveChangesAsync();
            }

            if (ros.RollOverStatus == 16)
            {
                var sixths = await _context.SixthManVoting.Where(x => x.LeagueId == leagueId).ToListAsync();
                foreach (var six in sixths)
                {
                    _context.SixthManVoting.Remove(six);
                }
                await _context.SaveChangesAsync();

                ros.RollOverStatus = 17;
                _context.Update(ros);
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<bool> RolloverLoadNextDraftPicks(int leagueId)
        {
            var ros = await _context.RolloverStatuses.FirstOrDefaultAsync(x => x.LeagueID == leagueId);
            if (ros.RollOverStatus == 17) {
                var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == leagueId);

                int year = 0;
                switch (league.Year)
                {
                    case 1314:
                        year = 1718;
                        break;
                    case 1415:
                        year = 1819;
                        break;
                    case 1516:
                        year = 1920;
                        break;
                    case 1617:
                        year = 2021;
                        break;
                    case 1718:
                        year = 2122;
                        break;
                    case 1819:
                        year = 2223;
                        break;
                    case 1920:
                        year = 2324;
                        break;
                    case 2021:
                        year = 2425;
                        break;
                    default:
                        break;
                }

                // (Year, Round, OriginalTeam, CurrentTeam, LeagueId
                var teams = await _context.Teams.Where(x => x.LeagueId == leagueId).ToListAsync();
                foreach (var team in teams)
                {
                    // Now we add the first and second round pick for each team
                    TeamDraftPick first = new TeamDraftPick
                    {
                        Year = year,
                        Round = 1,
                        OriginalTeam = team.TeamId,
                        CurrentTeam = team.TeamId,
                        LeagueId = leagueId
                    };

                    TeamDraftPick second = new TeamDraftPick
                    {
                        Year = year,
                        Round = 2,
                        OriginalTeam = team.TeamId,
                        CurrentTeam = team.TeamId,
                        LeagueId = leagueId
                    };
                    await _context.AddAsync(first);
                    await _context.AddAsync(second);
                }
                await _context.SaveChangesAsync();

                ros.RollOverStatus = 18;
                _context.Update(ros);
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<bool> RolloverTeamSettings(int leagueId)
        {
            var ros = await _context.RolloverStatuses.FirstOrDefaultAsync(x => x.LeagueID == leagueId);
            if (ros.RollOverStatus == 18) 
            {
                // depth chart
                var depthCharts = await _context.DepthCharts.Where(x => x.LeagueId == leagueId).ToListAsync();
                foreach (var dc in depthCharts)
                {
                    _context.DepthCharts.Remove(dc);
                }
                await _context.SaveChangesAsync();

                ros.RollOverStatus = 19;
                _context.Update(ros);
                await _context.SaveChangesAsync();
            }

            if (ros.RollOverStatus == 19) 
            {
                // go to players
                var coachSettings = await _context.CoachSettings.Where(x => x.LeagueId == leagueId).ToListAsync();
                foreach (var cs in coachSettings)
                {
                    cs.GoToPlayerOne = 0;
                    cs.GoToPlayerTwo = 0;
                    cs.GoToPlayerThree = 0;
                    _context.Update(cs);
                }
                 await _context.SaveChangesAsync();

                ros.RollOverStatus = 20;
                _context.Update(ros);
                await _context.SaveChangesAsync();
            }

            if (ros.RollOverStatus == 20) 
            {
                // reset team strategies
                var teamSettings = await _context.TeamStrategies.Where(x => x.LeagueId == leagueId).ToListAsync();
                foreach (var ts in teamSettings)
                {
                    ts.OffensiveStrategyId = 1;
                    ts.DefensiveStrategyId = 1;
                    _context.Update(ts);
                }
                await _context.SaveChangesAsync();

                ros.RollOverStatus = 21;
                _context.Update(ros);
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<bool> RolloverDeleteMessagesAndOffers(int leagueId)
        {
            var ros = await _context.RolloverStatuses.FirstOrDefaultAsync(x => x.LeagueID == leagueId);
            if (ros.RollOverStatus == 21) {
                var messages = await _context.InboxMessages.Where(x => x.LeagueId == leagueId).ToListAsync();
                foreach (var msg in messages)
                {
                    _context.Update(msg);
                }
                await _context.SaveChangesAsync();

                ros.RollOverStatus = 22;
                _context.Update(ros);
                await _context.SaveChangesAsync();
            }

            if (ros.RollOverStatus == 22)
            {
                var trades = await _context.Trades.Where(x => x.LeagueId == leagueId).ToListAsync();
                foreach (var trade in trades)
                {
                    _context.Update(trade);
                }
                await _context.SaveChangesAsync();

                ros.RollOverStatus = 23;
                _context.Update(ros);
                await _context.SaveChangesAsync();
            }

            if (ros.RollOverStatus == 23)
            {
                var offers = await _context.ContractOffers.Where(x => x.LeagueId == leagueId).ToListAsync();
                foreach (var off in offers)
                {
                    _context.Update(off);
                }
                await _context.SaveChangesAsync();

                ros.RollOverStatus = 24;
                _context.Update(ros);
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<bool> RolloverRetiredPlayers(int leagueId)
        {
            var ros = await _context.RolloverStatuses.FirstOrDefaultAsync(x => x.LeagueID == leagueId);
            if (ros.RollOverStatus == 24)
            {
                var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == leagueId);
                
                // Need to load the retired players into the Retired Players table
                var retiringPlayers = await _context.RetiringPlayers.Where(x => x.SeasonId == league.Year).ToListAsync();

                foreach (var rp in retiringPlayers)
                {
                    var playerDetails = await _context.Players.FirstOrDefaultAsync(x => x.LeagueId == leagueId && x.PlayerId == rp.PlayerId);

                    string positions = "";
                    if (playerDetails.PGPosition == 1) {
                        positions = "PG ";
                    }

                    if (playerDetails.SGPosition == 1) {
                        positions = positions + "SG ";
                    }

                    if (playerDetails.SFPosition == 1) {
                        positions = positions + "SF ";
                    }

                    if (playerDetails.PFPosition == 1) {
                        positions = positions + "PF ";
                    }

                    if (playerDetails.CPosition == 1) {
                        positions = positions + "C";
                    }

                    RetiredPlayer player = new RetiredPlayer
                    {
                        Name = playerDetails.FirstName + " " + playerDetails.Surname,
                        Year = league.Year,
                        PlayerId = rp.PlayerId,
                        Positions = positions,
                        LeagueId = leagueId
                    };
                    await _context.AddAsync(player);

                    // remove player from Rosters
                    var rosterRecord = await _context.Rosters.FirstOrDefaultAsync(x => x.LeagueId == leagueId && x.PlayerId == rp.PlayerId);
                    if (rosterRecord != null) {
                        _context.Remove(rosterRecord);
                    }

                    // Remove player from PlayerTeams
                    var ptRecord = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.LeagueId == leagueId && x.PlayerId == rp.PlayerId);
                    _context.Remove(ptRecord);

                    // Remove player from PlayerContracts
                    var pcRecord = await _context.PlayerContracts.FirstOrDefaultAsync(x => x.LeagueId == leagueId && x.PlayerId == rp.PlayerId);
                    if (pcRecord != null) {
                        _context.Remove(pcRecord);
                    }
                }
                await _context.SaveChangesAsync();

                ros.RollOverStatus = 25;
                _context.Update(ros);
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<bool> RolloverUpdateContractsAndPlayers(int leagueId)
        {
            var ros = await _context.RolloverStatuses.FirstOrDefaultAsync(x => x.LeagueID == leagueId);
            if (ros.RollOverStatus == 25) 
            {
                // get all player contracts for the league
                var contracts = await _context.PlayerContracts.Where(x => x.LeagueId == leagueId).ToListAsync();
                foreach (var contract in contracts)
                {
                    // Need to check player options here
                    if (contract.YearTwo == 0 && contract.YearOne != 0 && contract.PlayerOption == 1)
                    {
                        // Player has an option
                        Random r = new Random();
                        int result = r.Next(101);

                        if (result <= 66) {
                            // Player has taken the option
                            contract.PlayerOption = 0;
                        } else {
                            // Player has rejected the option
                            // Remove the player from the roster
                            var rosterRecord = await _context.Rosters.FirstOrDefaultAsync(x => x.LeagueId == leagueId && x.PlayerId == contract.PlayerId);
                            _context.Remove(rosterRecord);

                            // Change the players player team to be 31
                            var ptRecord = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.LeagueId == leagueId && x.PlayerId == contract.PlayerId);
                            _context.Remove(ptRecord);

                            // Delete the contract
                            _context.Remove(contract);
                        }
                    }
                    
                    // Update the contract
                    contract.YearFive = 0;
                    contract.GuranteedFive = 0;
                    contract.YearFour = contract.YearFive;
                    contract.GuranteedFour = contract.GuranteedFive;
                    contract.YearThree = contract.YearFour;
                    contract.GuranteedThree = contract.GuranteedFour;
                    contract.YearTwo = contract.YearThree;
                    contract.GuranteedTwo = contract.GuranteedThree;
                    contract.YearOne = contract.YearTwo;
                    contract.GuranteedOne = contract.GuranteedTwo;

                    // Now need to check if the contract has expired
                    if (contract.YearOne == 0)
                    {
                        // Remove the player from the roster
                        var rosterRecord = await _context.Rosters.FirstOrDefaultAsync(x => x.LeagueId == leagueId && x.PlayerId == contract.PlayerId);
                        _context.Remove(rosterRecord);

                        // Change the players player team to be 31
                        var ptRecord = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.LeagueId == leagueId && x.PlayerId == contract.PlayerId);
                        _context.Remove(ptRecord);

                        // Delete the contract
                        _context.Remove(contract);
                    }
                }
                await _context.SaveChangesAsync();

                // Now need to update all teams salary caps
                var teams = await _context.Teams.Where(x => x.LeagueId == leagueId).ToListAsync();
                foreach (var team in teams)
                {
                    int capAmount = 0;
                    var cap = await _context.TeamSalaryCaps.FirstOrDefaultAsync(x => x.TeamId == team.TeamId && x.LeagueId == leagueId);
                    var playersOnTeam = await _context.PlayerTeams.Where(x => x.TeamId == team.TeamId && x.LeagueId == leagueId).ToListAsync();
                    foreach (var p in playersOnTeam)
                    {
                        var contract = await _context.PlayerContracts.FirstOrDefaultAsync(x => x.LeagueId == leagueId && x.PlayerId == p.PlayerId);
                        capAmount = capAmount + contract.YearOne;
                    }
                    cap.CurrentCapAmount = capAmount;
                    _context.Update(cap);
                }
                await _context.SaveChangesAsync();

                ros.RollOverStatus = 26;
                _context.Update(ros);
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<bool> RolloverDeletePlayerDetailsData(int leagueId)
        {
            var ros = await _context.RolloverStatuses.FirstOrDefaultAsync(x => x.LeagueID == leagueId);
            if (ros.RollOverStatus == 26) 
            {
                // Delete player data
                // Players
                var players = await _context.Players.Where(x => x.LeagueId == leagueId).ToListAsync();
                foreach (var player in players)
                {
                    _context.Remove(player);
                }
                await _context.SaveChangesAsync();

                // PlayerRatings
                var playerRatings = await _context.PlayerRatings.Where(x => x.LeagueId == leagueId).ToListAsync();
                foreach (var pr in playerRatings)
                {
                    _context.Remove(pr);
                }
                await _context.SaveChangesAsync();

                // PlayerGradings
                var playerGradings = await _context.PlayerGradings.Where(x => x.LeagueId == leagueId).ToListAsync();
                foreach (var pg in playerGradings)
                {
                    _context.Remove(pg);
                }
                await _context.SaveChangesAsync();

                // PlayerTendancies
                var tendancies = await _context.PlayerTendancies.Where(x => x.LeagueId == leagueId).ToListAsync();

                foreach (var t in tendancies)
                {
                    _context.Remove(t);
                }
                await _context.SaveChangesAsync();

                ros.RollOverStatus = 27;
                _context.Update(ros);
                await _context.SaveChangesAsync();
            }           
            return true;
        }

        public async Task<bool> RolloverUpdateSeason(int leagueId)
        {
            var ros = await _context.RolloverStatuses.FirstOrDefaultAsync(x => x.LeagueID == leagueId);
            if (ros.RollOverStatus == 27)
            {
                var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == leagueId);
                
                int year = 0;
                switch (league.Year)
                {
                    case 1314:
                        year = 1415;
                        break;
                    case 1415:
                        year = 1516;
                        break;
                    case 1516:
                        year = 1617;
                        break;
                    case 1617:
                        year = 1718;
                        break;
                    case 1718:
                        year = 1819;
                        break;
                    case 1819:
                        year = 2920;
                        break;
                    case 1920:
                        year = 2021;
                        break;
                    case 2021:
                        year = 2425;
                        break;
                    default:
                        break;
                }
                league.Year = year;
                _context.Update(league);
                await _context.SaveChangesAsync();

                ros.RollOverStatus = 28;
                _context.Update(ros);
                await _context.SaveChangesAsync();
            }
            return true;
        }
        
        public async Task<bool> RolloverAddPlayerData(int leagueId)
        {
            var ros = await _context.RolloverStatuses.FirstOrDefaultAsync(x => x.LeagueID == leagueId);
            if (ros.RollOverStatus == 28)
            {
                var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == leagueId);
                var seasonId = new SqlParameter("@seasonId", league.Year);
                var lid = new SqlParameter("@leagueId", league.Id);
                await _context.Database.ExecuteSqlRawAsync("exec spLoadNewPlayerData @leagueId, @seasonId", lid, seasonId);
                
                ros.RollOverStatus = 29;
                _context.Update(ros);
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<bool> RolloverSetPlayerTeams(int leagueId)
        {
            var ros = await _context.RolloverStatuses.FirstOrDefaultAsync(x => x.LeagueID == leagueId);
            if (ros.RollOverStatus == 29)
            {
                var players = await _context.Players.Where(x => x.LeagueId == leagueId).ToListAsync();
                foreach (var p in players)
                {
                    var pt = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.LeagueId == leagueId && p.PlayerId == x.PlayerId);
                    if (pt == null) {
                        PlayerTeam newPt = new PlayerTeam
                        {
                            PlayerId = p.PlayerId,
                            TeamId = 31,
                            LeagueId = leagueId
                        };
                        await _context.AddAsync(newPt);
                    }
                }
                await _context.SaveChangesAsync();

                ros.RollOverStatus = 30;
                _context.Update(ros);
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<bool> RolloverFinishUp(int leagueId)
        {
            var ros = await _context.RolloverStatuses.FirstOrDefaultAsync(x => x.LeagueID == leagueId);
            if (ros.RollOverStatus == 30)
            {
                var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == leagueId);
                league.StateId = 13;
                league.Day = 0;
                ros.RollOverStatus = 0;
            }
            return await _context.SaveChangesAsync() > 1;
        }

        public async Task<bool> EndSeason(int leagueId)
        {
            var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == leagueId);
            // Update Player Career Stats
            var playerStats = await _context.PlayerStats.Where(x => x.LeagueId == leagueId).ToListAsync();

            foreach (var ps in playerStats)
            {
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == ps.PlayerId && x.LeagueId == leagueId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.TeamId == playerTeam.TeamId && x.LeagueId == leagueId);
                CareerPlayerStat stats = new CareerPlayerStat
                {
                    PlayerId = ps.PlayerId,
                    SeasonId = league.Year,
                    Team = team.TeamId,
                    GamesPlayed = ps.GamesPlayed,
                    Minutes = ps.Minutes,
                    Points = ps.Points,
                    Rebounds = ps.Rebounds,
                    Assists = ps.Assists,
                    Steals = ps.Steals,
                    Blocks = ps.Blocks,
                    FieldGoalsMade = ps.FieldGoalsMade,
                    FieldGoalsAttempted = ps.FieldGoalsAttempted,
                    ThreeFieldGoalsMade = ps.ThreeFieldGoalsMade,
                    ThreeFieldGoalsAttempted = ps.ThreeFieldGoalsAttempted,
                    FreeThrowsMade = ps.FreeThrowsMade,
                    FreeThrowsAttempted = ps.FreeThrowsAttempted,
                    ORebs = ps.ORebs,
                    DRebs = ps.DRebs,
                    Turnovers = ps.Turnovers,
                    Fouls = ps.Fouls,
                    Ppg = ps.Ppg,
                    Apg = ps.Apg,
                    Rpg = ps.Rpg,
                    Spg = ps.Spg,
                    Bpg = ps.Bpg,
                    Mpg = ps.Mpg,
                    Tpg = ps.Tpg,
                    Fpg = ps.Fpg,
                    LeagueId = leagueId
                };
                await _context.AddAsync(stats);
            }
            await _context.SaveChangesAsync();

            // Update Player Career Playoff Stats
            var playerStatsPlayoffs = await _context.PlayerStatsPlayoffs.Where(x => x.LeagueId == leagueId).ToListAsync();

            foreach (var ps in playerStatsPlayoffs)
            {
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == ps.PlayerId && x.LeagueId == leagueId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.TeamId == playerTeam.TeamId && x.LeagueId == leagueId);
                CareerPlayerStatsPlayoff stats = new CareerPlayerStatsPlayoff
                {
                    PlayerId = ps.PlayerId,
                    SeasonId = league.Year,
                    Team = team.TeamId,
                    GamesPlayed = ps.GamesPlayed,
                    Minutes = ps.Minutes,
                    Points = ps.Points,
                    Rebounds = ps.Rebounds,
                    Assists = ps.Assists,
                    Steals = ps.Steals,
                    Blocks = ps.Blocks,
                    FieldGoalsMade = ps.FieldGoalsMade,
                    FieldGoalsAttempted = ps.FieldGoalsAttempted,
                    ThreeFieldGoalsMade = ps.ThreeFieldGoalsMade,
                    ThreeFieldGoalsAttempted = ps.ThreeFieldGoalsAttempted,
                    FreeThrowsMade = ps.FreeThrowsMade,
                    FreeThrowsAttempted = ps.FreeThrowsAttempted,
                    ORebs = ps.ORebs,
                    DRebs = ps.DRebs,
                    Turnovers = ps.Turnovers,
                    Fouls = ps.Fouls,
                    Ppg = ps.Ppg,
                    Apg = ps.Apg,
                    Rpg = ps.Rpg,
                    Spg = ps.Spg,
                    Bpg = ps.Bpg,
                    Mpg = ps.Mpg,
                    Tpg = ps.Tpg,
                    Fpg = ps.Fpg,
                    LeagueId = leagueId
                };
                await _context.AddAsync(stats);
            }
            await _context.SaveChangesAsync();

            // Now need to update the draft picks for the next additional season
            var teams = await _context.Teams.Where(x => x.LeagueId == leagueId).ToListAsync();
            int newSeasonForPicks = league.Year + 6;

            foreach (var team in teams)
            {
                for (int j = 1; j < 3; j++)
                {
                    TeamDraftPick dp = new TeamDraftPick
                    {
                        Year = newSeasonForPicks,
                        Round = j,
                        OriginalTeam = team.TeamId,
                        CurrentTeam = team.TeamId,
                        LeagueId = leagueId
                    };
                    await _context.AddAsync(dp);
                }
            }
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RunTeamDraftPicks(int leagueId)
        {
            var teams = await _context.Teams.Where(x => x.LeagueId == leagueId).ToListAsync();
            var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == leagueId);

            foreach (var team in teams)
            {
                int start = league.Year + 1;
                for (int i = start; i < start + 6; i++)
                {
                    for (int j = 1; j < 3; j++)
                    {
                        // Now need to create the Team Draft Pick object
                        TeamDraftPick dp = new TeamDraftPick
                        {
                            Year = i,
                            Round = j,
                            OriginalTeam = team.TeamId,
                            CurrentTeam = team.TeamId,
                            LeagueId = leagueId
                        };
                        await _context.AddAsync(dp);
                    }
                }
            }
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> GenerateInitialContracts(int leagueId)
        {
            var initialDraftPicks = await _context.InitialDrafts.Where(x => x.LeagueId == leagueId).ToListAsync();
            foreach (var dp in initialDraftPicks)
            {
                int years = 0;
                int amount = 0;

                // int yearOne = 

                switch (dp.Round)
                {
                    case 1:
                        if (dp.Pick > 0 && dp.Pick <= 10)
                        {
                            years = 5;
                            amount = 25000000;
                        }
                        else if (dp.Pick > 10 && dp.Pick <= 20)
                        {
                            years = 4;
                            amount = 23750000;
                        }
                        else if (dp.Pick > 20 && dp.Pick <= 30)
                        {
                            years = 3;
                            amount = 22500000;
                        }
                        break;
                    case 2:
                        if (dp.Pick > 0 && dp.Pick <= 10)
                        {
                            years = 4;
                            amount = 18750000;
                        }
                        else if (dp.Pick > 10 && dp.Pick <= 20)
                        {
                            years = 3;
                            amount = 16250000;
                        }
                        else if (dp.Pick > 20 && dp.Pick <= 30)
                        {
                            years = 2;
                            amount = 16250000;
                        }
                        break;
                    case 3:
                        if (dp.Pick > 0 && dp.Pick <= 10)
                        {
                            years = 3;
                            amount = 15000000;
                        }
                        else if (dp.Pick > 10 && dp.Pick <= 20)
                        {
                            years = 2;
                            amount = 13750000;
                        }
                        else if (dp.Pick > 20 && dp.Pick <= 30)
                        {
                            years = 1;
                            amount = 13750000;
                        }
                        break;
                    case 4:
                        if (dp.Pick > 0 && dp.Pick <= 10)
                        {
                            years = 3;
                            amount = 12500000;
                        }
                        else if (dp.Pick > 10 && dp.Pick <= 20)
                        {
                            years = 2;
                            amount = 11250000;
                        }
                        else if (dp.Pick > 20 && dp.Pick <= 30)
                        {
                            years = 2;
                            amount = 11250000;
                        }
                        break;
                    case 5:
                        if (dp.Pick > 0 && dp.Pick <= 10)
                        {
                            years = 3;
                            amount = 10000000;
                        }
                        else if (dp.Pick > 10 && dp.Pick <= 20)
                        {
                            years = 2;
                            amount = 8750000;
                        }
                        else if (dp.Pick > 20 && dp.Pick <= 30)
                        {
                            years = 1;
                            amount = 8750000;
                        }
                        break;
                    case 6:
                        if (dp.Pick > 0 && dp.Pick <= 10)
                        {
                            years = 2;
                            amount = 6250000;
                        }
                        else if (dp.Pick > 10 && dp.Pick <= 20)
                        {
                            years = 2;
                            amount = 6250000;
                        }
                        else if (dp.Pick > 20 && dp.Pick <= 30)
                        {
                            years = 2;
                            amount = 6250000;
                        }
                        break;
                    case 7:
                        if (dp.Pick > 0 && dp.Pick <= 10)
                        {
                            years = 1;
                            amount = 6250000;
                        }
                        else if (dp.Pick > 10 && dp.Pick <= 20)
                        {
                            years = 1;
                            amount = 6250000;
                        }
                        else if (dp.Pick > 20 && dp.Pick <= 30)
                        {
                            years = 1;
                            amount = 6250000;
                        }
                        break;
                    case 8:
                        if (dp.Pick > 0 && dp.Pick <= 10)
                        {
                            years = 2;
                            amount = 5000000;
                        }
                        else if (dp.Pick > 10 && dp.Pick <= 20)
                        {
                            years = 2;
                            amount = 5000000;
                        }
                        else if (dp.Pick > 20 && dp.Pick <= 30)
                        {
                            years = 1;
                            amount = 5000000;
                        }
                        break;
                    case 9:
                        if (dp.Pick > 0 && dp.Pick <= 10)
                        {
                            years = 2;
                            amount = 3750000;
                        }
                        else if (dp.Pick > 10 && dp.Pick <= 20)
                        {
                            years = 1;
                            amount = 3750000;
                        }
                        else if (dp.Pick > 20 && dp.Pick <= 30)
                        {
                            years = 1;
                            amount = 3750000;
                        }
                        break;
                    case 10:
                        if (dp.Pick > 0 && dp.Pick <= 10)
                        {
                            years = 2;
                            amount = 2500000;
                        }
                        else if (dp.Pick > 10 && dp.Pick <= 20)
                        {
                            years = 2;
                            amount = 2500000;
                        }
                        else if (dp.Pick > 20 && dp.Pick <= 30)
                        {
                            years = 1;
                            amount = 2500000;
                        }
                        break;
                    case 11:
                        if (dp.Pick > 0 && dp.Pick <= 10)
                        {
                            years = 1;
                            amount = 2500000;
                        }
                        else if (dp.Pick > 10 && dp.Pick <= 20)
                        {
                            years = 2;
                            amount = 1250000;
                        }
                        else if (dp.Pick > 20 && dp.Pick <= 30)
                        {
                            years = 2;
                            amount = 1000000;
                        }
                        break;
                    case 12:
                        if (dp.Pick > 0 && dp.Pick <= 10)
                        {
                            years = 1;
                            amount = 1250000;
                        }
                        else if (dp.Pick > 10 && dp.Pick <= 20)
                        {
                            years = 1;
                            amount = 1250000;
                        }
                        else if (dp.Pick > 20 && dp.Pick <= 30)
                        {
                            years = 1;
                            amount = 1250000;
                        }
                        break;
                    case 13:
                        if (dp.Pick > 0 && dp.Pick <= 10)
                        {
                            years = 1;
                            amount = 1000000;
                        }
                        else if (dp.Pick > 10 && dp.Pick <= 20)
                        {
                            years = 1;
                            amount = 1000000;
                        }
                        else if (dp.Pick > 20 && dp.Pick <= 30)
                        {
                            years = 1;
                            amount = 1000000;
                        }
                        break;
                    default:
                        break;
                }

                InitialDraftContract pc = new InitialDraftContract
                {
                    Round = dp.Round,
                    Pick = dp.Pick,
                    Years = years,
                    SalaryAmount = amount,
                };

                await _context.AddAsync(pc);
            }
            return await _context.SaveChangesAsync() > 1;
        }

        public async Task<IEnumerable<CurrentDayGamesDto>> GetGamesForRreset(int leagueId)
        {
            var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == leagueId);
            var todaysGames = await _context.Schedules.Where(x => x.GameDay == league.Day).ToListAsync();

            List<CurrentDayGamesDto> nextGamesList = new List<CurrentDayGamesDto>();
            foreach (var game in todaysGames)
            {
                var awayTeam = await _context.Teams.FirstOrDefaultAsync(x => x.TeamId == game.AwayTeamId && x.LeagueId == leagueId);
                var homeTeam = await _context.Teams.FirstOrDefaultAsync(x => x.TeamId == game.HomeTeamId && x.LeagueId == leagueId);
                var gameResult = await _context.GameResults.FirstOrDefaultAsync(x => x.GameId == game.Id && x.LeagueId == leagueId);

                int awayScore = 0;
                int homeScore = 0;
                int completed = 0;

                if (gameResult != null)
                {
                    awayScore = gameResult.AwayScore;
                    homeScore = gameResult.HomeScore;
                    completed = gameResult.Completed;
                }

                CurrentDayGamesDto ng = new CurrentDayGamesDto
                {
                    Id = game.Id,
                    AwayTeamId = awayTeam.TeamId,
                    AwayTeamName = awayTeam.Teamname + " " + awayTeam.Mascot,
                    HomeTeamId = homeTeam.TeamId,
                    HomeTeamName = homeTeam.Teamname + " " + homeTeam.Mascot,
                    Day = league.Day + 1,
                    awayScore = awayScore,
                    homeScore = homeScore,
                    Completed = completed
                };

                nextGamesList.Add(ng);
            }

            return nextGamesList;
        }

        public async Task<bool> ResetGame(int gameId, int leagueId)
        {
            var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == leagueId);
            if (league.StateId == 7)
            {
                var gameresult = await _context.GameResults.FirstOrDefaultAsync(x => x.GameId == gameId && x.LeagueId == leagueId);
                var boxScores = await _context.GameBoxScores.Where(x => x.GameId == gameId && x.LeagueId == leagueId).ToListAsync();
                var playByPlays = await _context.PlayByPlays.Where(x => x.GameId == gameId && x.LeagueId == leagueId).ToListAsync();

                // _context.GameResults.Remove(gameresult);
                gameresult.AwayScore = 0;
                gameresult.Completed = 0;
                gameresult.HomeScore = 0;
                gameresult.WinningTeamId = 0;
                _context.GameResults.Update(gameresult);
                await _context.SaveChangesAsync();

                foreach (var bs in boxScores)
                {
                    _context.GameBoxScores.Remove(bs);
                }

                foreach (var pbp in playByPlays)
                {
                    _context.PlayByPlays.Remove(pbp);
                }
                return await _context.SaveChangesAsync() > 0;
            }
            return true;
        }

        public async Task<bool> SaveSeasonHistoricalRecords(int leagueId)
        {
            // Team Records
            var teams = await _context.Teams.Where(x => x.LeagueId == leagueId).ToListAsync();
            var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == leagueId);

            foreach (var team in teams)
            {
                // Get standings record for the team
                var teamStanding = await _context.Standings.FirstOrDefaultAsync(x => x.TeamId == team.TeamId && x.LeagueId == leagueId);

                // Need to determine if the team made the playoffs or not
                var lottery = 1;
                var firstRound = 0;
                var secondRound = 0;
                var confFinals = 0;
                var finals = 0;
                var champion = 0;
                var playoffWins = 0;
                var playoffLossess = 0;

                var ps = await _context.PlayoffSerieses.Where(x => (x.AwayTeamId == team.TeamId || x.HomeTeamId == team.TeamId) && x.LeagueId == leagueId).ToListAsync();
                if (ps != null)
                {
                    // Team made the players
                    lottery = 0;
                    firstRound = 1;
                    if (ps.Count > 1)
                    {
                        secondRound = 1;
                    }

                    if (ps.Count > 2)
                    {
                        confFinals = 1;
                    }

                    if (ps.Count > 3)
                    {
                        finals = 1;

                        // Need to check the if the team won the title
                        var fs = await _context.PlayoffSerieses.Where(x => x.LeagueId == leagueId).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                        if (fs.AwayTeamId == team.TeamId)
                        {
                            if (fs.AwayWins == 4)
                            {
                                champion = 1;
                            }
                        }
                        else if (fs.HomeTeamId == team.TeamId)
                        {
                            if (fs.HomeWins == 4)
                            {
                                champion = 1;
                            }
                        }
                    }

                    foreach (var series in ps)
                    {
                        if (series.AwayTeamId == team.TeamId)
                        {
                            playoffWins = playoffWins + series.AwayWins;
                            playoffLossess = playoffLossess + series.HomeWins;
                        }
                        else if (series.HomeTeamId == team.TeamId)
                        {
                            playoffWins = playoffWins + series.HomeWins;
                            playoffLossess = playoffLossess + series.AwayWins;
                        }
                    }
                }

                HistoricalTeamRecord htr = new HistoricalTeamRecord
                {
                    TeamId = team.TeamId,
                    SeasonId = league.Year,
                    Wins = teamStanding.Wins,
                    Losses = teamStanding.Losses,
                    Lottery = lottery,
                    FirstRound = firstRound,
                    SecondRound = secondRound,
                    ConfFinals = confFinals,
                    Finals = finals,
                    Champion = champion,
                    PlayoffWins = playoffWins,
                    PlayoffLosses = playoffLossess,
                    LeagueId = leagueId
                };
                await _context.AddAsync(htr);
            }
            await _context.SaveChangesAsync();

            // Awards
            var mvpVotes = await _context.MvpVoting.Where(x => x.LeagueId == leagueId).OrderByDescending(x => x.Votes).FirstOrDefaultAsync();
            var dpoyVotes = await _context.DpoyVoting.Where(x => x.LeagueId == leagueId).OrderByDescending(x => x.Votes).FirstOrDefaultAsync();
            var sixthVotes = await _context.SixthManVoting.Where(x => x.LeagueId == leagueId).OrderByDescending(x => x.Votes).FirstOrDefaultAsync();

            var mvpPlayer = await _context.Players.FirstOrDefaultAsync(x => x.PlayerId == mvpVotes.PlayerId && x.LeagueId == leagueId);
            var dpoyPlayer = await _context.Players.FirstOrDefaultAsync(x => x.PlayerId == dpoyVotes.PlayerId && x.LeagueId == leagueId);
            var sixthPlayer = await _context.Players.FirstOrDefaultAsync(x => x.PlayerId == sixthVotes.PlayerId && x.LeagueId == leagueId);

            var mvpPlayerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == mvpVotes.PlayerId && x.LeagueId == leagueId);
            var mvpTeam = await _context.Teams.FirstOrDefaultAsync(x => x.TeamId == mvpPlayerTeam.TeamId && x.LeagueId == leagueId);
            var dpoyPlayerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == dpoyVotes.PlayerId && x.LeagueId == leagueId);
            var dpoyTeam = await _context.Teams.FirstOrDefaultAsync(x => x.TeamId == dpoyPlayerTeam.TeamId && x.LeagueId == leagueId);
            var sixthPlayerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == sixthVotes.PlayerId && x.LeagueId == leagueId);
            var sixthTeam = await _context.Teams.FirstOrDefaultAsync(x => x.TeamId == sixthPlayerTeam.TeamId && x.LeagueId == leagueId);

            AwardWinner mvpWinner = new AwardWinner
            {
                SeasonId = league.Year,
                PlayerId = mvpVotes.PlayerId,
                PlayerName = mvpPlayer.FirstName + " " + mvpPlayer.Surname,
                Team = mvpTeam.Teamname + " " + mvpTeam.Mascot,
                Mvp = 1,
                Dpoy = 0,
                Sixth = 0,
                LeagueId = leagueId
            };
            await _context.AddAsync(mvpWinner);

            AwardWinner dpoyWinner = new AwardWinner
            {
                SeasonId = league.Year,
                PlayerId = dpoyVotes.PlayerId,
                PlayerName = dpoyPlayer.FirstName + " " + dpoyPlayer.Surname,
                Team = dpoyTeam.Teamname + " " + dpoyTeam.Mascot,
                Mvp = 0,
                Dpoy = 1,
                Sixth = 0,
                LeagueId = leagueId
            };
            await _context.AddAsync(dpoyWinner);

            AwardWinner sixthWinner = new AwardWinner
            {
                SeasonId = league.Year,
                PlayerId = sixthVotes.PlayerId,
                PlayerName = sixthPlayer.FirstName + " " + sixthPlayer.Surname,
                Team = sixthTeam.Teamname + " " + sixthTeam.Mascot,
                Mvp = 0,
                Dpoy = 0,
                Sixth = 1,
                LeagueId = leagueId
            };
            await _context.AddAsync(sixthWinner);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RolloverSeasonCareerStats(int leagueId)
        {
            var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == leagueId);
            var playerStats = await _context.PlayerStats.Where(x => x.LeagueId == leagueId).ToListAsync();
            var playerStatsPlayoffs = await _context.PlayerStatsPlayoffs.Where(x => x.LeagueId == leagueId).ToListAsync();

            // Player Stats
            foreach (var ps in playerStats)
            {
                var pt = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == ps.PlayerId && x.LeagueId == leagueId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.TeamId == pt.TeamId && x.LeagueId == leagueId);

                CareerPlayerStat stats = new CareerPlayerStat
                {
                    PlayerId = ps.PlayerId,
                    Team = team.TeamId,
                    SeasonId = league.Year,
                    GamesPlayed = ps.GamesPlayed,
                    Minutes = ps.Minutes,
                    Points = ps.Points,
                    Rebounds = ps.Rebounds,
                    Assists = ps.Assists,
                    Steals = ps.Steals,
                    Blocks = ps.Blocks,
                    FieldGoalsMade = ps.FieldGoalsMade,
                    FieldGoalsAttempted = ps.FieldGoalsAttempted,
                    ThreeFieldGoalsAttempted = ps.ThreeFieldGoalsAttempted,
                    ThreeFieldGoalsMade = ps.ThreeFieldGoalsMade,
                    FreeThrowsAttempted = ps.FreeThrowsAttempted,
                    FreeThrowsMade = ps.FreeThrowsMade,
                    ORebs = ps.ORebs,
                    DRebs = ps.DRebs,
                    Turnovers = ps.Turnovers,
                    Fouls = ps.Fouls,
                    Ppg = ps.Ppg, //ps.Points / ps.GamesPlayed,
                    Apg = ps.Apg, //ps.Assists / ps.GamesPlayed,
                    Rpg = ps.Rpg, //ps.Rebounds / ps.GamesPlayed,
                    Spg = ps.Spg,
                    Bpg = ps.Bpg,
                    Mpg = ps.Mpg,
                    Fpg = ps.Fpg,
                    Tpg = ps.Tpg,
                    LeagueId = leagueId
                };
                await _context.AddAsync(stats);
                _context.PlayerStats.Remove(ps);
            }

            // Player Stats Playoffs
            foreach (var ps in playerStatsPlayoffs)
            {
                var pt = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == ps.PlayerId && x.LeagueId == leagueId);
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.TeamId == pt.TeamId && x.LeagueId == leagueId);

                CareerPlayerStatsPlayoff stats = new CareerPlayerStatsPlayoff
                {
                    PlayerId = ps.PlayerId,
                    Team = team.TeamId,
                    SeasonId = league.Year,
                    GamesPlayed = ps.GamesPlayed,
                    Minutes = ps.Minutes,
                    Points = ps.Points,
                    Rebounds = ps.Rebounds,
                    Assists = ps.Assists,
                    Steals = ps.Steals,
                    Blocks = ps.Blocks,
                    FieldGoalsMade = ps.FieldGoalsMade,
                    FieldGoalsAttempted = ps.FieldGoalsAttempted,
                    ThreeFieldGoalsAttempted = ps.ThreeFieldGoalsAttempted,
                    ThreeFieldGoalsMade = ps.ThreeFieldGoalsMade,
                    FreeThrowsAttempted = ps.FreeThrowsAttempted,
                    FreeThrowsMade = ps.FreeThrowsMade,
                    ORebs = ps.ORebs,
                    DRebs = ps.DRebs,
                    Turnovers = ps.Turnovers,
                    Fouls = ps.Fouls,
                    Ppg = ps.Ppg, //ps.Points / ps.GamesPlayed,
                    Apg = ps.Apg, //ps.Assists / ps.GamesPlayed,
                    Rpg = ps.Rpg, //ps.Rebounds / ps.GamesPlayed,
                    Spg = ps.Spg,
                    Bpg = ps.Bpg,
                    Mpg = ps.Mpg,
                    Fpg = ps.Fpg,
                    Tpg = ps.Tpg,
                    LeagueId = leagueId
                };
                await _context.AddAsync(stats);
                _context.PlayerStatsPlayoffs.Remove(ps);
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateTeamSalaries(int leagueId)
        {
            var teams = await _context.Teams.Where(x => x.LeagueId == leagueId).ToListAsync();

            foreach (var team in teams)
            {
                var teamSalaryCap = await _context.TeamSalaryCaps.FirstOrDefaultAsync(x => x.TeamId == team.TeamId && x.LeagueId == leagueId);
                var teamRoster = await _context.Rosters.Where(x => x.TeamId == team.TeamId && x.LeagueId == leagueId).ToListAsync();

                int salaryAmount = 0;
                foreach (var roster in teamRoster)
                {
                    var playerContract = await _context.PlayerContracts.FirstOrDefaultAsync(x => x.PlayerId == roster.PlayerId && x.LeagueId == leagueId);
                    salaryAmount = salaryAmount + playerContract.YearOne;
                }

                var waivedContracts = await _context.WaivedPlayerContracts.Where(x => x.TeamId == team.TeamId && x.LeagueId == leagueId).ToListAsync();
                foreach (var contract in waivedContracts)
                {
                    salaryAmount = salaryAmount + contract.YearOne;
                }

                // Now save the updated cap value
                teamSalaryCap.CurrentCapAmount = salaryAmount;
                _context.TeamSalaryCaps.Update(teamSalaryCap);
            }
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> GenerateDraftLottery(int leagueId)
        {
            var existingPicks = await _context.DraftPicks.Where(x => x.LeagueId == leagueId).ToListAsync();
            SqlParameter lid = new SqlParameter("@lid", leagueId);
            if (existingPicks != null)
            {
                string sql = "delete from DraftPicks where LeagueId = @lid";
                await _context.Database.ExecuteSqlRawAsync(sql, lid);
            }
            string sqldr = "delete from DraftRankings where LeagueId = @lid";
            await _context.Database.ExecuteSqlRawAsync(sqldr, lid);

            List<int> teamIds = new List<int>();
            var standings = await _context.Standings.Where(x => x.LeagueId == leagueId).OrderBy(x => x.Wins).ToListAsync();

            for (int i = 0; i < 14; i++)
            {
                var standingRecord = standings[i];
                var teamId = standingRecord.TeamId;

                int pickNumber = i + 1;
                int chances = 0;
                switch (pickNumber)
                {
                    case 1:
                        chances = 140;
                        break;
                    case 2:
                        chances = 140;
                        break;
                    case 3:
                        chances = 140;
                        break;
                    case 4:
                        chances = 125;
                        break;
                    case 5:
                        chances = 105;
                        break;
                    case 6:
                        chances = 90;
                        break;
                    case 7:
                        chances = 75;
                        break;
                    case 8:
                        chances = 60;
                        break;
                    case 9:
                        chances = 45;
                        break;
                    case 10:
                        chances = 30;
                        break;
                    case 11:
                        chances = 20;
                        break;
                    case 12:
                        chances = 15;
                        break;
                    case 13:
                        chances = 10;
                        break;
                    case 14:
                        chances = 5;
                        break;
                    default:
                        break;
                }

                for (int j = 0; j < chances; j++)
                {
                    teamIds.Add(teamId);
                }
            }

            // Now need to shuffle the list
            int n = teamIds.Count;
            Random rng = new Random();

            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                int value = teamIds[k];
                teamIds[k] = teamIds[n];
                teamIds[n] = value;
            }

            // Now need to get the order down to 14
            List<int> draftLotteryOrder = new List<int>();
            for (int i = 0; i < 14; i++)
            {
                int teamId = teamIds[0];
                draftLotteryOrder.Add(teamId);
                teamIds.Remove(teamId);
            }

            // Now need to add the non-lottery picks
            for (int i = 14; i < 31; i++)
            {
                var standingRecord = standings[i];
                var teamId = standingRecord.TeamId;
                draftLotteryOrder.Add(teamId);
            }

            // // Now need to go through and save the draft picks
            // int pick = 1;
            for (int i = 1; i < 3; i++)
            {
                if (i == 1)
                {
                    int pickNumer = 1;
                    foreach (var selection in draftLotteryOrder)
                    {
                        var properPick = await _context.TeamDraftPicks.FirstOrDefaultAsync(x => x.Round == i && x.OriginalTeam == selection && x.LeagueId == leagueId);
                        DraftPick draftPick = new DraftPick
                        {
                            Round = 1,
                            Pick = pickNumer,
                            TeamId = properPick.CurrentTeam,
                            PlayerId = 0,
                            LeagueId = leagueId
                        };
                        await _context.DraftPicks.AddAsync(draftPick);
                        pickNumer++;

                        if (pickNumer == 31)
                        {
                            pickNumer = 1;
                        }
                    }
                }
                else
                {
                    int pickNumer = 1;
                    foreach (var selection in standings)
                    {
                        var properPick = await _context.TeamDraftPicks.FirstOrDefaultAsync(x => x.Round == i && x.OriginalTeam == selection.TeamId && x.LeagueId == leagueId);
                        DraftPick draftPick = new DraftPick
                        {
                            Round = 2,
                            Pick = pickNumer,
                            TeamId = properPick.CurrentTeam,
                            PlayerId = 0,
                            LeagueId = leagueId
                        };
                        await _context.DraftPicks.AddAsync(draftPick);

                        pickNumer++;

                        if (pickNumer == 31)
                        {
                            pickNumer = 1;
                        }
                    }
                }
            }
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ContractUpdates(int leagueId)
        {
            // Now to update all player contracts
            var allContracts = await _context.PlayerContracts.Where(x => x.LeagueId == leagueId).ToListAsync();

            foreach (var contract in allContracts)
            {
                int years = GetActualContractYears(contract);

                if (years == 5)
                {
                    contract.YearOne = contract.YearTwo;
                    contract.GuranteedOne = contract.GuranteedTwo;
                    contract.YearTwo = contract.YearThree;
                    contract.GuranteedTwo = contract.GuranteedThree;
                    contract.YearThree = contract.YearFour;
                    contract.GuranteedThree = contract.GuranteedFour;
                    contract.YearFour = contract.YearFive;
                    contract.GuranteedFour = contract.GuranteedFive;
                }
                else if (years == 4)
                {
                    contract.YearOne = contract.YearTwo;
                    contract.GuranteedOne = contract.GuranteedTwo;
                    contract.YearTwo = contract.YearThree;
                    contract.GuranteedTwo = contract.GuranteedThree;
                    contract.YearThree = contract.YearFour;
                    contract.GuranteedThree = contract.GuranteedFour;
                }
                else if (years == 3)
                {
                    contract.YearOne = contract.YearTwo;
                    contract.GuranteedOne = contract.GuranteedTwo;
                    contract.YearTwo = contract.YearThree;
                    contract.GuranteedTwo = contract.GuranteedThree;
                }
                else if (years == 2)
                {
                    contract.YearOne = contract.YearTwo;
                    contract.GuranteedOne = contract.GuranteedTwo;
                }
                else if (years == 1)
                {
                    contract.YearOne = 0;
                    contract.GuranteedOne = 0;
                    contract.PlayerOption = 0;
                    contract.TeamOption = 0;

                    // Contract is now a free agent and should be picked up after
                }
                _context.PlayerContracts.Update(contract);
            }
            await _context.SaveChangesAsync();

            var zeroLengthContracts = await _context.PlayerContracts.Where(x => x.YearOne == 0 && x.LeagueId == leagueId).ToListAsync();
            if (zeroLengthContracts != null)
            {
                foreach (var contract in zeroLengthContracts)
                {
                    int playerId = contract.PlayerId;

                    // delete the contract
                    _context.PlayerContracts.Remove(contract);

                    // check for roster record
                    var rosterRecord = await _context.Rosters.FirstOrDefaultAsync(x => x.PlayerId == playerId && x.LeagueId == leagueId);
                    _context.Rosters.Remove(rosterRecord);

                    // Get and update the PlayerTeam
                    var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == playerId && x.LeagueId == leagueId);
                    playerTeam.TeamId = 0;
                    _context.PlayerTeams.Update(playerTeam);
                }
                await _context.SaveChangesAsync();
            }

            // Need to update Waived Contracts!
            var waivedContracts = await _context.WaivedPlayerContracts.Where(x => x.LeagueId == leagueId).ToListAsync();

            foreach (var contract in waivedContracts)
            {
                int years = GetWaivedContractYears(contract);

                if (years == 5)
                {
                    contract.YearOne = contract.YearTwo;
                    contract.YearTwo = contract.YearThree;
                    contract.YearThree = contract.YearFour;
                    contract.YearFour = contract.YearFive;
                }
                else if (years == 4)
                {
                    contract.YearOne = contract.YearTwo;
                    contract.YearTwo = contract.YearThree;
                    contract.YearThree = contract.YearFour;
                }
                else if (years == 3)
                {
                    contract.YearOne = contract.YearTwo;
                    contract.YearTwo = contract.YearThree;
                }
                else if (years == 2)
                {
                    contract.YearOne = contract.YearTwo;
                }
                else if (years == 1)
                {
                    contract.YearOne = 0;
                }
                _context.WaivedPlayerContracts.Update(contract);
            }
            await _context.SaveChangesAsync();

            var zeroContracts = await _context.WaivedPlayerContracts.Where(x => x.YearOne == 0 && x.LeagueId == leagueId).ToListAsync();
            if (zeroContracts != null)
            {
                foreach (var contract in zeroContracts)
                {
                    int playerId = contract.PlayerId;

                    // delete the contract
                    _context.WaivedPlayerContracts.Remove(contract);
                }
                await _context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> DeletePreseasonData(int leagueId)
        {
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE PreseasonGameResults");
            // return await _context.SaveChangesAsync() > 0;
            return false;
        }

        public async Task<bool> DeletePlayoffData(int leagueId)
        {
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE PlayByPlaysPlayoffs");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE PlayerStatsPlayoffs");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE PlayoffBoxScores");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE PlayoffResults");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE PlayoffSeries");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE SchedulePlayoffs");
            // return await _context.SaveChangesAsync() > 0;
            return false;
        }

        public async Task<bool> DeleteTeamSettings(int leagueId)
        {
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE CoachSettings");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE DepthCharts");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE TeamStrategies");
            // return await _context.SaveChangesAsync() > 0;
            return false;
        }

        public async Task<bool> DeleteAwardsData(int leagueId)
        {
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE DpoyVoting");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE MvpVoting");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE SixthManVoting");
            // return await _context.SaveChangesAsync() > 0;
            return false;
        }

        public async Task<bool> DeleteOtherSeasonData(int leagueId)
        {
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE FreeAgencyDecisions");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE InboxMessages");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE TradeMessages");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Trades");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Transactions");
            // return await _context.SaveChangesAsync() > 0;
            return false;
        }

        public async Task<bool> DeleteSeasonData(int leagueId)
        {
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE GameBoxScores");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE GameResults");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE PlayerStats");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE PlayByPlays");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE MidLevelExceptions");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE BiAnnualExceptions");
            // return await _context.SaveChangesAsync() > 0;
            return false;
        }

        public async Task<bool> ResetStandings(int leagueId)
        {
            var ros = await _context.RolloverStatuses.FirstOrDefaultAsync(x => x.LeagueID == leagueId);
            if (ros.RollOverStatus == 4)
            {
                var standings = await _context.Standings.Where(x => x.LeagueId == leagueId).ToListAsync();

                foreach (var s in standings)
                {
                    s.ConfLosses = 0;
                    s.ConfWins = 0;
                    s.GamesPlayed = 0;
                    s.HomeLosses = 0;
                    s.HomeWins = 0;
                    s.Losses = 0;
                    s.RoadLosses = 0;
                    s.RoadWins = 0;
                    s.Wins = 0;

                    _context.Standings.Update(s);
                }
            }
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RolloverLeague(int leagueId)
        {
            var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == leagueId);
            league.Year = league.Year + 1;
            league.StateId = 14;
            _context.Leagues.Update(league);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ResetLeague(int leagueId)
        {
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE GameBoxScores");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE GameResults");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE PlayerStats");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE PlayByPlays");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE FreeAgencyDecisions");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE InboxMessages");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE TradeMessages");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Trades");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Transactions");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE DpoyVoting");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE MvpVoting");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE SixthManVoting");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE CoachSettings");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE DepthCharts");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE TeamStrategies");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE PlayByPlayPlayoffs");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE PlayerStatsPlayoffs");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE PlayoffBoxScores");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE PlayoffResults");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE PlayoffSerieses");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE SchedulesPlayoffs");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE PreseasonGameResults");

            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE MidLevelExceptions");

            // // Now the extras for a complete reset to fresh league
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE AutoPickOrders");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE AwardWinners");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE CareerPlayerStats");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE CareerPlayerStatsPlayoffs");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE ContractOffers");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE DraftPicks");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE DraftRankings");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE DraftTrackers");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE GlobalChats");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE HistoricalTeamRecords");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE InitialDrafts");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE PlayerCareerStats");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE PlayerCareerStatsPlayoffs");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE PlayerContracts");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE PlayerGradings");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE PlayerInjuries");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE PlayerRatings");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Players");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE PlayerTeams");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE PlayerTendancies");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Rosters");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE TeamDraftPicks");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Users");
            // await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE WaivedPlayerContracts");

            // // Update the league
            // var league = await _context.Leagues.FirstOrDefaultAsync();
            // league.Day = 0;
            // league.StateId = 1;
            // league.Year = 1;
            // _context.Leagues.Update(league);

            // var teams = await _context.Teams.ToListAsync();
            // foreach (var team in teams)
            // {
            //     team.UserId = 0;
            //     _context.Teams.Update(team);

            //     // Need to update the Bi-Annual Exception
            //     BiAnnualException bae = new BiAnnualException
            //     {
            //         TeamId = team.Id,
            //         AmountRemaining = 3382000,
            //         YearUsed = 0
            //     };
            //     await _context.BiAnnualExceptions.AddAsync(bae);

            //     // Need to then create the new MLE recrods
            //     MidLevelException mle = new MidLevelException
            //     {
            //         TeamId = team.Id,
            //         AmountRemaining = 8641000,
            //         AmountUsed = 0
            //     };
            //     await _context.MidLevelExceptions.AddAsync(mle);
            // }

            // var teamSalarycaps = await _context.TeamSalaryCaps.ToListAsync();
            // foreach (var caps in teamSalarycaps)
            // {
            //     caps.CurrentCapAmount = 0;
            //     _context.TeamSalaryCaps.Update(caps);
            // }

            // var standings = await _context.Standings.ToListAsync();
            // foreach (var stand in standings)
            // {
            //     stand.ConfLosses = 0;
            //     stand.ConfWins = 0;
            //     stand.GamesPlayed = 0;
            //     stand.HomeLosses = 0;
            //     stand.HomeWins = 0;
            //     stand.Losses = 0;
            //     stand.RoadLosses = 0;
            //     stand.RoadWins = 0;
            //     stand.Wins = 0;

            //     _context.Standings.Update(stand);
            // }
            // return await _context.SaveChangesAsync() > 0;
            return false;
        }

        public async Task<bool> GenerateInitialSalaryCaps(int leagueId)
        {
            var teams = await _context.Teams.Where(x => x.LeagueId == leagueId).ToListAsync();
            foreach (var team in teams)
            {
                int teamSalary = 0;
                var contracts = await _context.PlayerContracts.Where(x => x.TeamId == team.TeamId && x.LeagueId == leagueId).ToListAsync();
                foreach (var contract in contracts)
                {
                    teamSalary = teamSalary + contract.YearOne;
                }

                // Now update the TeamSalary Record
                var salaryCap = await _context.TeamSalaryCaps.FirstOrDefaultAsync(x => x.TeamId == team.TeamId && x.LeagueId == leagueId);
                salaryCap.CurrentCapAmount = teamSalary;
                _context.TeamSalaryCaps.Update(salaryCap);
            }
            return await _context.SaveChangesAsync() > 0;
        }
    }
}