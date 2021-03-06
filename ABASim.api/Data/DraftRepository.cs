using System.Linq;
using System.Threading.Tasks;
using ABASim.api.Dtos;
using ABASim.api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;

namespace ABASim.api.Data
{
    public class DraftRepository : IDraftRepository
    {
        private readonly DataContext _context;
        public DraftRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> AddDraftRanking(AddDraftRankingDto draftRanking)
        {
            var teamsDraftRankings = await _context.DraftRankings.Where(x => x.TeamId == draftRanking.TeamId && x.LeagueId == draftRanking.LeagueId).ToListAsync();
            DraftRanking newRanking = new DraftRanking
            {
                TeamId = draftRanking.TeamId,
                PlayerId = draftRanking.PlayerId,
                Rank = teamsDraftRankings.Count + 1,
                LeagueId = draftRanking.LeagueId
            };
            await _context.AddAsync(newRanking);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<DraftPlayerDto>> GetDraftBoardForTeamId(int teamId, int leagueId)
        {
            List<DraftPlayerDto> draftboardPlayers = new List<DraftPlayerDto>();
            var draftRankings = await _context.DraftRankings.Where(x => x.TeamId == teamId && x.LeagueId == leagueId).OrderBy(x => x.Rank).ToListAsync();

            foreach (var player in draftRankings)
            {
                var playerRecord = await _context.Players.FirstOrDefaultAsync(x => x.PlayerId == player.PlayerId && x.LeagueId == leagueId);
                var playerGrades = await _context.PlayerGradings.FirstOrDefaultAsync(x => x.PlayerId == player.PlayerId && x.LeagueId == leagueId);

                DraftPlayerDto newPlayer = new DraftPlayerDto();
                newPlayer.PlayerId = playerRecord.PlayerId;
                newPlayer.BlockGrade = playerGrades.BlockGrade;
                newPlayer.CPosition = playerRecord.CPosition;
                newPlayer.Age = playerRecord.Age;
                newPlayer.DRebGrade = playerGrades.DRebGrade;
                newPlayer.FirstName = playerRecord.FirstName;
                newPlayer.FTGrade = playerGrades.FTGrade;
                newPlayer.HandlingGrade = playerGrades.HandlingGrade;
                newPlayer.IntangiblesGrade = playerGrades.IntangiblesGrade;
                newPlayer.ORebGrade = playerGrades.ORebGrade;
                newPlayer.PassingGrade = playerGrades.PassingGrade;
                newPlayer.PFPosition = playerRecord.PFPosition;
                newPlayer.PGPosition = playerRecord.PGPosition;
                newPlayer.SFPosition = playerRecord.SFPosition;
                newPlayer.SGPosition = playerRecord.SGPosition;
                newPlayer.StaminaGrade = playerGrades.StaminaGrade;
                newPlayer.StealGrade = playerGrades.StealGrade;
                newPlayer.Surname = playerRecord.Surname;
                newPlayer.ThreeGrade = playerGrades.ThreeGrade;
                newPlayer.TwoGrade = playerGrades.TwoGrade;

                draftboardPlayers.Add(newPlayer);
            }
            return draftboardPlayers;
        }

        public async Task<bool> RemoveDraftRanking(RemoveDraftRankingDto draftRanking)
        {
            var draftRankingRecord = await _context.DraftRankings.FirstOrDefaultAsync(x => x.TeamId == draftRanking.TeamId && x.PlayerId == draftRanking.PlayerId && x.LeagueId == draftRanking.LeagueId);
            _context.Remove(draftRankingRecord);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> MovePlayerRankingUp(AddDraftRankingDto ranking)
        {
            List<DraftPlayerDto> draftboardPlayers = new List<DraftPlayerDto>();
            var draftRankings = await _context.DraftRankings.Where(x => x.TeamId == ranking.TeamId && x.LeagueId == ranking.LeagueId).OrderBy(x => x.Rank).ToListAsync();
            var index = draftRankings.FindIndex(x => x.PlayerId == ranking.PlayerId);
            var rankingToMoveUp = draftRankings.ElementAt(index);
            var rankingToMoveDown = draftRankings.ElementAt(index - 1); // This will need to change to be rank lookup

            rankingToMoveUp.Rank = rankingToMoveUp.Rank - 1;
            rankingToMoveDown.Rank = rankingToMoveDown.Rank + 1;

            _context.Update(rankingToMoveUp);
            _context.Update(rankingToMoveDown);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> MovePlayerRankingDown(AddDraftRankingDto ranking)
        {
            List<DraftPlayerDto> draftboardPlayers = new List<DraftPlayerDto>();
            var draftRankings = await _context.DraftRankings.Where(x => x.TeamId == ranking.TeamId && x.LeagueId == ranking.LeagueId).OrderBy(x => x.Rank).ToListAsync();
            var index = draftRankings.FindIndex(x => x.PlayerId == ranking.PlayerId);
            var rankingToMoveDown = draftRankings.ElementAt(index);
            var rankingToMoveUp = draftRankings.ElementAt(index + 1);

            rankingToMoveUp.Rank = rankingToMoveUp.Rank - 1;
            rankingToMoveDown.Rank = rankingToMoveDown.Rank + 1;

            _context.Update(rankingToMoveUp);
            _context.Update(rankingToMoveDown);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> BeginInitialDraft(int leagueId)
        {
            // Get UTC time
            DateTime dt = DateTime.UtcNow.ToUniversalTime();
            dt.AddMinutes(10);

            string dateAndTime = dt.ToString("MM/dd/yyyy HH:mm:ss");

            var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == leagueId);
            if (league.StateId == 14) {
                var draftTracker = await _context.DraftTrackers.FirstOrDefaultAsync(x => x.LeagueId == leagueId);
                draftTracker.Round = 1;
                draftTracker.Pick = 1;
                draftTracker.DateTimeOfLastPick = dateAndTime;
                _context.Update(draftTracker);
            } else {
                DraftTracker draftTracker = new DraftTracker()
                {
                    Round = 1,
                    Pick = 1,
                    DateTimeOfLastPick = dateAndTime,
                    LeagueId = leagueId
                };
                await _context.AddAsync(draftTracker);
            }

            // Now the draft record is saved - now need to update the league state            
            if (league.StateId != 14) {
                league.StateId = 4;
            }
            league.Day = 3;
            _context.Update(league);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<DraftTracker> GetDraftTracker(int leagueId)
        {
            var tracker = await _context.DraftTrackers.FirstOrDefaultAsync(x => x.LeagueId == leagueId);
            return tracker;
        }

        public async Task<IEnumerable<InitialDraft>> GetInitialDraftPicks(int leagueId)
        {
            var initialDraftPicks = await _context.InitialDrafts.Where(x => x.LeagueId == leagueId).ToListAsync();
            return initialDraftPicks;
        }

        public async Task<bool> MakeDraftPick(InitialDraftPicksDto draftPick)
        {
            var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == draftPick.LeagueId);
            if (league.StateId == 3 || league.StateId == 4 || league.StateId == 5)
            {
                var draftSelection = await _context.InitialDrafts.FirstOrDefaultAsync(x => x.Pick == draftPick.Pick && x.Round == draftPick.Round && x.LeagueId == draftPick.LeagueId);
                draftSelection.PlayerId = draftPick.PlayerId;
                _context.Update(draftSelection);

                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == draftPick.PlayerId && x.LeagueId == draftPick.LeagueId);
                playerTeam.TeamId = draftPick.TeamId;
                _context.Update(playerTeam);

                var teamRoser = new Roster
                {
                    PlayerId = draftPick.PlayerId,
                    TeamId = draftPick.TeamId,
                    LeagueId = draftPick.LeagueId
                };
                await _context.AddAsync(teamRoser);

                var tracker = await _context.DraftTrackers.FirstOrDefaultAsync(x => x.LeagueId == draftPick.LeagueId);

                // Now need to add the InitialDraftPick Contract
                var contractDetails = await _context.InitialDraftContracts.FirstOrDefaultAsync(x => x.Round == tracker.Round && x.Pick == tracker.Pick);
                int yearOne = 0;
                int yearTwo = 0;
                int yearThree = 0;
                int yearFour = 0;
                int yearFive = 0;

                int gOne = 0;
                int gTwo = 0;
                int gThree = 0;
                int gFour = 0;
                int gFive = 0;

                if (contractDetails.Years == 1)
                {
                    yearOne = contractDetails.SalaryAmount;
                    gOne = 1;
                }
                else if (contractDetails.Years == 2)
                {
                    yearOne = contractDetails.SalaryAmount;
                    gOne = 1;
                    yearTwo = contractDetails.SalaryAmount;
                    gTwo = 1;
                }
                else if (contractDetails.Years == 3)
                {
                    yearOne = contractDetails.SalaryAmount;
                    gOne = 1;
                    yearTwo = contractDetails.SalaryAmount;
                    gTwo = 1;
                    yearThree = contractDetails.SalaryAmount;
                    gThree = 1;
                }
                else if (contractDetails.Years == 4)
                {
                    yearOne = contractDetails.SalaryAmount;
                    gOne = 1;
                    yearTwo = contractDetails.SalaryAmount;
                    gTwo = 1;
                    yearThree = contractDetails.SalaryAmount;
                    gThree = 1;
                    yearFour = contractDetails.SalaryAmount;
                    gFour = 1;
                }
                else if (contractDetails.Years == 5)
                {
                    yearOne = contractDetails.SalaryAmount;
                    gOne = 1;
                    yearTwo = contractDetails.SalaryAmount;
                    gTwo = 1;
                    yearThree = contractDetails.SalaryAmount;
                    gThree = 1;
                    yearFour = contractDetails.SalaryAmount;
                    gFour = 1;
                    yearFive = contractDetails.SalaryAmount;
                    gFive = 1;
                }

                PlayerContract pc = new PlayerContract
                {
                    PlayerId = draftPick.PlayerId,
                    TeamId = draftPick.TeamId,
                    YearOne = yearOne,
                    GuranteedOne = gOne,
                    YearTwo = yearTwo,
                    GuranteedTwo = gTwo,
                    YearThree = yearThree,
                    GuranteedThree = gThree,
                    YearFour = yearFour,
                    GuranteedFour = gFour,
                    YearFive = yearFive,
                    GuranteedFive = gFive,
                    LeagueId = draftPick.LeagueId,
                    TeamOption = 1,
                    PlayerOption = 0
                };
                await _context.AddAsync(pc);


                if (tracker.Pick < 30)
                {
                    tracker.Pick++;
                }
                else
                {
                    if (tracker.Round < 13)
                    {
                        tracker.Pick = 1;
                        tracker.Round++;
                    }
                    else if (tracker.Round == 13 && tracker.Pick == 30)
                    {
                        // Draft is finished
                        var leagueState = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == draftPick.LeagueId);
                        leagueState.StateId = 5;
                        leagueState.Day = 5;
                        _context.Update(leagueState);

                        // Make all undrafted players a free agent
                        var undraftedPlayers = await _context.PlayerTeams.Where(x => x.TeamId == 31 && x.LeagueId == draftPick.LeagueId).ToListAsync();
                        foreach (var up in undraftedPlayers)
                        {
                            up.TeamId = 0;
                            _context.PlayerTeams.Update(up);
                        }

                        // Update all teams salary caps
                        var teams = await _context.Teams.Where(x => x.LeagueId == draftPick.LeagueId).ToListAsync();
                        foreach (var team in teams)
                        {
                            int teamSalary = 0;
                            var contracts = await _context.PlayerContracts.Where(x => x.TeamId == team.TeamId && x.LeagueId == draftPick.LeagueId).ToListAsync();
                            foreach (var contract in contracts)
                            {
                                teamSalary = teamSalary + contract.YearOne;
                            }

                            // Now update the TeamSalary Record
                            var salaryCap = await _context.TeamSalaryCaps.FirstOrDefaultAsync(x => x.TeamId == team.TeamId && x.LeagueId == draftPick.LeagueId);
                            salaryCap.CurrentCapAmount = teamSalary;
                            _context.TeamSalaryCaps.Update(salaryCap);
                        }
                    }
                }
                DateTime dt = DateTime.UtcNow.ToUniversalTime();
                dt = dt.AddMinutes(6);

                string dateAndTime = dt.ToString("MM/dd/yyyy HH:mm:ss");
                tracker.DateTimeOfLastPick = dateAndTime;

                _context.Update(tracker);

                // Need to remove from draftboards
                var draftboards = await _context.DraftRankings.Where(x => x.PlayerId == draftPick.PlayerId && x.LeagueId == draftPick.LeagueId).ToListAsync();
                foreach (var db in draftboards)
                {
                    // Need to remove the record and update all rankings
                    var teamDB = await _context.DraftRankings.Where(x => x.TeamId == db.TeamId && x.LeagueId == draftPick.LeagueId).ToListAsync();

                    var recordToRemove = teamDB.Find(x => x.PlayerId == db.PlayerId);
                    var rank = recordToRemove.Rank;

                    foreach (var record in teamDB)
                    {
                        if (record.Rank > rank)
                        {
                            record.Rank--;
                            _context.Update(record);
                        }
                    }
                    _context.Remove(recordToRemove);
                }

                // Need to move from the autopick board
                var autopickRecord = await _context.AutoPickOrders.FirstOrDefaultAsync(x => x.PlayerId == draftPick.PlayerId && x.LeagueId == draftPick.LeagueId);
                _context.Remove(autopickRecord);
            }
            else if (league.StateId == 14)
            {
                var draftSelection = await _context.DraftPicks.FirstOrDefaultAsync(x => x.Pick == draftPick.Pick && x.Round == draftPick.Round && x.LeagueId == draftPick.LeagueId);
                draftSelection.PlayerId = draftPick.PlayerId;
                _context.Update(draftSelection);

                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == draftPick.PlayerId && x.LeagueId == draftPick.LeagueId);
                playerTeam.TeamId = draftPick.TeamId;
                _context.Update(playerTeam);

                var teamRoser = new Roster
                {
                    PlayerId = draftPick.PlayerId,
                    TeamId = draftPick.TeamId,
                    LeagueId = draftPick.LeagueId
                };
                await _context.AddAsync(teamRoser);

                var tracker = await _context.DraftTrackers.FirstOrDefaultAsync(x => x.LeagueId == draftPick.LeagueId);

                // Now need to add the InitialDraftPick Contract
                if (tracker.Round == 1)
                {
                    var contractDetails = await _context.RegularDraftContracts.FirstOrDefaultAsync(x => x.Round == tracker.Round && x.Pick == tracker.Pick);
                    int yearOne = 0;
                    int yearTwo = 0;
                    int yearThree = 0;
                    int yearFour = 0;
                    int yearFive = 0;

                    int gOne = 0;
                    int gTwo = 0;
                    int gThree = 0;
                    int gFour = 0;
                    int gFive = 0;

                    if (contractDetails.Years == 1)
                    {
                        yearOne = contractDetails.SalaryAmount;
                        gOne = 1;
                    }
                    else if (contractDetails.Years == 2)
                    {
                        yearOne = contractDetails.SalaryAmount;
                        gOne = 1;
                        yearTwo = contractDetails.SalaryAmount;
                        gTwo = 1;
                    }
                    else if (contractDetails.Years == 3)
                    {
                        yearOne = contractDetails.SalaryAmount;
                        gOne = 1;
                        yearTwo = contractDetails.SalaryAmount;
                        gTwo = 1;
                        yearThree = contractDetails.SalaryAmount;
                        gThree = 1;
                    }
                    else if (contractDetails.Years == 4)
                    {
                        yearOne = contractDetails.SalaryAmount;
                        gOne = 1;
                        yearTwo = contractDetails.SalaryAmount;
                        gTwo = 1;
                        yearThree = contractDetails.SalaryAmount;
                        gThree = 1;
                        yearFour = contractDetails.SalaryAmount;
                        gFour = 1;
                    }
                    else if (contractDetails.Years == 5)
                    {
                        yearOne = contractDetails.SalaryAmount;
                        gOne = 1;
                        yearTwo = contractDetails.SalaryAmount;
                        gTwo = 1;
                        yearThree = contractDetails.SalaryAmount;
                        gThree = 1;
                        yearFour = contractDetails.SalaryAmount;
                        gFour = 1;
                        yearFive = contractDetails.SalaryAmount;
                        gFive = 1;
                    }

                    PlayerContract pc = new PlayerContract
                    {
                        PlayerId = draftPick.PlayerId,
                        TeamId = draftPick.TeamId,
                        YearOne = yearOne,
                        GuranteedOne = gOne,
                        YearTwo = yearTwo,
                        GuranteedTwo = gTwo,
                        YearThree = yearThree,
                        GuranteedThree = gThree,
                        YearFour = yearFour,
                        GuranteedFour = gFour,
                        YearFive = yearFive,
                        GuranteedFive = gFive,
                        LeagueId = draftPick.LeagueId,
                        TeamOption = 1,
                        PlayerOption = 0
                    };
                    await _context.AddAsync(pc);
                }
                else
                {
                    if (draftPick.Pick <= 15)
                    {
                        PlayerContract pc = new PlayerContract
                        {
                            PlayerId = draftPick.PlayerId,
                            TeamId = draftPick.TeamId,
                            YearOne = 1500000,
                            GuranteedOne = 0,
                            YearTwo = 1500000,
                            GuranteedTwo = 0,
                            YearThree = 0,
                            GuranteedThree = 0,
                            YearFour = 0,
                            GuranteedFour = 0,
                            YearFive = 0,
                            GuranteedFive = 0,
                            LeagueId = draftPick.LeagueId,
                            TeamOption = 0,
                            PlayerOption = 0
                        };
                        await _context.AddAsync(pc);
                    }
                    else
                    {
                        PlayerContract pc = new PlayerContract
                        {
                            PlayerId = draftPick.PlayerId,
                            TeamId = draftPick.TeamId,
                            YearOne = 1000000,
                            GuranteedOne = 0,
                            YearTwo = 1000000,
                            GuranteedTwo = 0,
                            YearThree = 0,
                            GuranteedThree = 0,
                            YearFour = 0,
                            GuranteedFour = 0,
                            YearFive = 0,
                            GuranteedFive = 0,
                            LeagueId = draftPick.LeagueId,
                            TeamOption = 0,
                            PlayerOption = 0
                        };
                        await _context.AddAsync(pc);
                    }
                }

                if (tracker.Pick < 30)
                {
                    tracker.Pick++;
                }
                else
                {
                    if (tracker.Round < 2)
                    {
                        tracker.Pick = 1;
                        tracker.Round++;
                    }
                    else if (tracker.Round == 2 && tracker.Pick == 30)
                    {
                        // Draft is finished
                        var leagueState = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == draftPick.LeagueId);
                        leagueState.StateId = 15;
                        leagueState.Day = 5;
                        _context.Update(leagueState);

                        // Make all undrafted players a free agent
                        var undraftedPlayers = await _context.PlayerTeams.Where(x => x.TeamId == 31 && x.LeagueId == draftPick.LeagueId).ToListAsync();
                        foreach (var up in undraftedPlayers)
                        {
                            up.TeamId = 0;
                            _context.PlayerTeams.Update(up);
                        }

                        // Update all teams salary caps
                        var teams = await _context.Teams.Where(x => x.LeagueId == draftPick.LeagueId).ToListAsync();
                        foreach (var team in teams)
                        {
                            int teamSalary = 0;
                            var contracts = await _context.PlayerContracts.Where(x => x.TeamId == team.TeamId && x.LeagueId == draftPick.LeagueId).ToListAsync();
                            foreach (var contract in contracts)
                            {
                                teamSalary = teamSalary + contract.YearOne;
                            }

                            // Now update the TeamSalary Record
                            var salaryCap = await _context.TeamSalaryCaps.FirstOrDefaultAsync(x => x.TeamId == team.TeamId && x.LeagueId == draftPick.LeagueId);
                            salaryCap.CurrentCapAmount = teamSalary;
                            _context.TeamSalaryCaps.Update(salaryCap);

                            // Now delete all IncomingDraftPlayerRecords
                            var incomingDraftPlayers = await _context.IncomingDraftPlayers.Where(x => x.LeagueId == draftPick.LeagueId).ToListAsync();
                            foreach (var idp in incomingDraftPlayers)
                            {
                                _context.Remove(idp);
                            }   
                        }
                    }
                }
                DateTime dt = DateTime.UtcNow.ToUniversalTime();
                dt = dt.AddMinutes(6);

                string dateAndTime = dt.ToString("MM/dd/yyyy HH:mm:ss");
                tracker.DateTimeOfLastPick = dateAndTime;

                _context.Update(tracker);

                // Need to remove from draftboards
                var draftboards = await _context.DraftRankings.Where(x => x.PlayerId == draftPick.PlayerId && x.LeagueId == draftPick.LeagueId).ToListAsync();
                foreach (var db in draftboards)
                {
                    // Need to remove the record and update all rankings
                    var teamDB = await _context.DraftRankings.Where(x => x.TeamId == db.TeamId && x.LeagueId == draftPick.LeagueId).ToListAsync();

                    var recordToRemove = teamDB.Find(x => x.PlayerId == db.PlayerId);
                    var rank = recordToRemove.Rank;

                    foreach (var record in teamDB)
                    {
                        if (record.Rank > rank)
                        {
                            record.Rank--;
                            _context.Update(record);
                        }
                    }
                    _context.Remove(recordToRemove);
                }

                // Need to move from the autopick board
                var autopickRecord = await _context.AutoPickOrders.FirstOrDefaultAsync(x => x.PlayerId == draftPick.PlayerId && x.LeagueId == draftPick.LeagueId);
                _context.Remove(autopickRecord);
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> MakeAutoPick(InitialDraftPicksDto draftPick)
        {
            var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == draftPick.LeagueId);
            if (league.StateId == 4) {
                var draftSelection = await _context.InitialDrafts.FirstOrDefaultAsync(x => x.Pick == draftPick.Pick && x.Round == draftPick.Round && x.LeagueId == draftPick.LeagueId);
                var teamId = draftSelection.TeamId;

                // Need to check whether the team has set a draft board
                var draftboard = await _context.DraftRankings.Where(x => x.TeamId == teamId && x.LeagueId == draftPick.LeagueId).OrderBy(x => x.Rank).ToListAsync();

                if (draftboard.Count > 0)
                {
                    foreach (var db in draftboard)
                    {
                        var playerTeamForPlayerId = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == db.PlayerId && x.LeagueId == draftPick.LeagueId);

                        if (playerTeamForPlayerId.TeamId == 31)
                        {
                            // Then this is the player that we will draft
                            draftPick.TeamId = teamId;
                            draftPick.PlayerId = playerTeamForPlayerId.PlayerId;
                            return await this.MakeDraftPick(draftPick);
                        }
                    }
                }
                else
                {
                    // Now need to get the auto pick order
                    var autopick = await _context.AutoPickOrders.OrderByDescending(x => x.Score).FirstOrDefaultAsync(x => x.LeagueId == draftPick.LeagueId);
                    draftPick.TeamId = teamId;
                    draftPick.PlayerId = autopick.PlayerId;
                    return await this.MakeDraftPick(draftPick);
                }
            } else if (league.StateId == 14) {
                var draftSelection = await _context.DraftPicks.FirstOrDefaultAsync(x => x.Pick == draftPick.Pick && x.Round == draftPick.Round && x.LeagueId == draftPick.LeagueId);
                var teamId = draftSelection.TeamId;

                // Need to check whether the team has set a draft board
                var draftboard = await _context.DraftRankings.Where(x => x.TeamId == teamId && x.LeagueId == draftPick.LeagueId).OrderBy(x => x.Rank).ToListAsync();

                if (draftboard.Count > 0)
                {
                    foreach (var db in draftboard)
                    {
                        var playerTeamForPlayerId = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == db.PlayerId && x.LeagueId == draftPick.LeagueId);

                        if (playerTeamForPlayerId.TeamId == 31)
                        {
                            // Then this is the player that we will draft
                            draftPick.TeamId = teamId;
                            draftPick.PlayerId = playerTeamForPlayerId.PlayerId;
                            return await this.MakeDraftPick(draftPick);
                        }
                    }
                }
                else
                {
                    // Now need to get the auto pick order
                    var autopick = await _context.AutoPickOrders.OrderByDescending(x => x.Score).FirstOrDefaultAsync(x => x.LeagueId == draftPick.LeagueId);
                    draftPick.TeamId = teamId;
                    draftPick.PlayerId = autopick.PlayerId;
                    return await this.MakeDraftPick(draftPick);
                }
            }
            return false;
        }

        public async Task<IEnumerable<DraftPickDto>> GetInitialDraftPicksForPage(int round, int leagueId)
        {
            List<DraftPickDto> draftPicks = new List<DraftPickDto>();
            var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == leagueId);

            if (league.StateId == 14)
            {
                var dps = await _context.DraftPicks.Where(x => x.Round == round && x.LeagueId == leagueId).OrderBy(x => x.Pick).ToListAsync();
                var teams = await _context.Teams.Where(x => x.LeagueId == leagueId).ToListAsync();

                foreach (var dp in dps)
                {
                    var currentTeam = teams.FirstOrDefault(x => x.TeamId == dp.TeamId && x.LeagueId == leagueId);

                    var player = await _context.Players.FirstOrDefaultAsync(x => x.PlayerId == dp.PlayerId && x.LeagueId == leagueId);
                    var playerName = "";
                    if (player != null)
                    {
                        playerName = player.FirstName + " " + player.Surname;
                    }

                    DraftPickDto dto = new DraftPickDto
                    {
                        Round = dp.Round,
                        Pick = dp.Pick,
                        TeamId = dp.TeamId,
                        TeamName = currentTeam.Teamname + " " + currentTeam.Mascot,
                        PlayerId = dp.PlayerId,
                        PlayerName = playerName
                    };
                    draftPicks.Add(dto);
                }
            }
            else
            {
                var initalDraftPicks = await _context.InitialDrafts.Where(x => x.Round == round && x.LeagueId == leagueId).OrderBy(x => x.Pick).ToListAsync();
                var teams = await _context.Teams.Where(x => x.LeagueId == leagueId).ToListAsync();

                foreach (var dp in initalDraftPicks)
                {
                    var currentTeam = teams.FirstOrDefault(x => x.TeamId == dp.TeamId && x.LeagueId == leagueId);

                    var player = await _context.Players.FirstOrDefaultAsync(x => x.PlayerId == dp.PlayerId && x.LeagueId == leagueId);
                    var playerName = "";
                    if (player != null)
                    {
                        playerName = player.FirstName + " " + player.Surname;
                    }

                    DraftPickDto dto = new DraftPickDto
                    {
                        Round = dp.Round,
                        Pick = dp.Pick,
                        TeamId = dp.TeamId,
                        TeamName = currentTeam.Teamname + " " + currentTeam.Mascot,
                        PlayerId = dp.PlayerId,
                        PlayerName = playerName
                    };
                    draftPicks.Add(dto);
                }
            }
            return draftPicks;
        }

        public async Task<DashboardDraftPickDto> GetDashboardDraftPick(int pick, int leagueId)
        {
            DashboardDraftPickDto pickDto = new DashboardDraftPickDto();

            // Get the draft tracker
            var draftTracker = await _context.DraftTrackers.FirstOrDefaultAsync(x => x.LeagueId == leagueId);

            // TODO NEED TO ADD IN END OF ROUND AND DRAFT CHECKS
            int pickNumber = draftTracker.Pick;
            int roundNumber = draftTracker.Round;

            if (pick == 0)
            {
                // current pick
                pickDto.Pick = pickNumber;
            }
            else if (pick == 1)
            {
                // next pick
                if (pickNumber == 30)
                {
                    if (roundNumber != 13)
                    {
                        roundNumber = roundNumber + 1;
                    }
                    else
                    {
                        roundNumber = 0;
                        pick = 0;
                    }

                    pickDto.Pick = 0;
                }
                else
                {
                    pickDto.Pick = pickNumber + 1;
                }
            }
            else if (pick == -1)
            {
                // previous pick
                if (pickNumber == 1)
                {
                    roundNumber = roundNumber - 1;
                    pickDto.Pick = 30;
                }
                else
                {
                    pickDto.Pick = draftTracker.Pick - 1;
                }
            }

            // Now need to get the team for that pick
            var id_pick = await _context.InitialDrafts.FirstOrDefaultAsync(x => x.Round == roundNumber && x.Pick == pickDto.Pick && x.LeagueId == leagueId);

            // Now need the team
            if (roundNumber != 0)
            {
                var team = await _context.Teams.FirstOrDefaultAsync(x => x.TeamId == id_pick.TeamId && x.LeagueId == leagueId);
                pickDto.TeamMascot = team.Mascot;
            }
            else
            {
                pickDto.TeamMascot = "";
            }

            if (pick == -1)
            {
                if (id_pick == null)
                {
                    pickDto.PlayerName = "";
                }
                else
                {
                    var player = await _context.Players.FirstOrDefaultAsync(x => x.PlayerId == id_pick.PlayerId && x.LeagueId == leagueId);
                    pickDto.PlayerName = player.FirstName + " " + player.Surname;
                }
            }
            else
            {
                pickDto.PlayerName = "";
            }
            return pickDto;
        }

        public async Task<InitialDraft> GetCurrentInitialDraftPick(int leagueId)
        {
            var tracker = await _context.DraftTrackers.FirstOrDefaultAsync(x => x.LeagueId == leagueId);
            var cp = await _context.InitialDrafts.FirstOrDefaultAsync(x => x.Pick == tracker.Pick && x.Round == tracker.Round && x.LeagueId == leagueId);
            return cp;
        }

        public async Task<IEnumerable<RegularDraftContract>> GetRegularDraftSalaryDetails()
        {
            List<RegularDraftContract> contracts = new List<RegularDraftContract>();
            var salaries = await _context.RegularDraftContracts.ToListAsync();

            foreach (var s in salaries)
            {
                contracts.Add(s);
            }

            // 2nd Round
            for (int i = 1; i<31; i++)
            {
                int salary = 0;
                if (i < 16) {
                    salary = 1500000;
                } else {
                    salary = 1000000;
                }
                RegularDraftContract c = new RegularDraftContract
                {
                    Round = 2,
                    Pick = i,
                    Years = 2,
                    SalaryAmount = salary,
                    TeamOption = 1
                };
                contracts.Add(c);
            }
            return contracts;
        }

        public async Task<IEnumerable<InitialPickSalaryDto>> GetInitialDraftSalaryDetails()
        {
            List<InitialPickSalaryDto> details = new List<InitialPickSalaryDto>();
            for (int i = 1; i < 14; i++)
            {
                var info1 = await _context.InitialDraftContracts.FirstOrDefaultAsync(x => x.Round == i && x.Pick == 5);
                var info2 = await _context.InitialDraftContracts.FirstOrDefaultAsync(x => x.Round == i && x.Pick == 15);
                var info3 = await _context.InitialDraftContracts.FirstOrDefaultAsync(x => x.Round == i && x.Pick == 25);

                InitialPickSalaryDto dto1 = new InitialPickSalaryDto
                {
                    Round = i,
                    Pick = 5,
                    Salary = info1.SalaryAmount,
                    Years = info1.Years
                };
                details.Add(dto1);

                InitialPickSalaryDto dto2 = new InitialPickSalaryDto
                {
                    Round = i,
                    Pick = 15,
                    Salary = info2.SalaryAmount,
                    Years = info2.Years
                };
                details.Add(dto2);

                InitialPickSalaryDto dto3 = new InitialPickSalaryDto
                {
                    Round = i,
                    Pick = 25,
                    Salary = info3.SalaryAmount,
                    Years = info3.Years
                };
                details.Add(dto3);
            }
            return details;
        }
    }
}