using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ABASim.api.Dtos;
using ABASim.api.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ABASim.api.Data
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly DataContext _context;
        public PlayerRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DraftPlayerDto>> DraftPoolFilterByPosition(int pos, int leagueId)
        {
            List<DraftPlayerDto> draftPool = new List<DraftPlayerDto>();
            List<Player> players = new List<Player>();
            // Get players
            if (pos == 1)
            {
                players = await _context.Players.Where(x => x.PGPosition == 1 && x.LeagueId == leagueId).OrderBy(x => x.Surname).ToListAsync();
            }
            else if (pos == 2)
            {
                players = await _context.Players.Where(x => x.SGPosition == 1 && x.LeagueId == leagueId).OrderBy(x => x.Surname).ToListAsync();
            }
            else if (pos == 3)
            {
                players = await _context.Players.Where(x => x.SFPosition == 1 && x.LeagueId == leagueId).OrderBy(x => x.Surname).ToListAsync();
            }
            else if (pos == 4)
            {
                players = await _context.Players.Where(x => x.PFPosition == 1 && x.LeagueId == leagueId).OrderBy(x => x.Surname).ToListAsync();
            }
            else if (pos == 5)
            {
                players = await _context.Players.Where(x => x.CPosition == 1 && x.LeagueId == leagueId).OrderBy(x => x.Surname).ToListAsync();
            }

            int total = players.Count;
            foreach (var player in players)
            {
                // NEED TO CHECK WHETHER THE PLAYER HAS BEEN DRAFTED
                var playerTeamForPlayerId = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == player.PlayerId && x.LeagueId == leagueId);

                if (playerTeamForPlayerId.TeamId == 31)
                {
                    var playerGrade = await _context.PlayerGradings.FirstOrDefaultAsync(x => x.PlayerId == player.PlayerId && x.LeagueId == leagueId);

                    // Now create the Dto
                    DraftPlayerDto newPlayer = new DraftPlayerDto();
                    newPlayer.PlayerId = player.PlayerId;
                    newPlayer.BlockGrade = playerGrade.BlockGrade;
                    newPlayer.CPosition = player.CPosition;
                    newPlayer.Age = player.Age;
                    newPlayer.DRebGrade = playerGrade.DRebGrade;
                    newPlayer.FirstName = player.FirstName;
                    newPlayer.FTGrade = playerGrade.FTGrade;
                    newPlayer.HandlingGrade = playerGrade.HandlingGrade;
                    newPlayer.IntangiblesGrade = playerGrade.IntangiblesGrade;
                    newPlayer.ORebGrade = playerGrade.ORebGrade;
                    newPlayer.PassingGrade = playerGrade.PassingGrade;
                    newPlayer.PFPosition = player.PFPosition;
                    newPlayer.PGPosition = player.PGPosition;
                    newPlayer.SFPosition = player.SFPosition;
                    newPlayer.SGPosition = player.SGPosition;
                    newPlayer.StaminaGrade = playerGrade.StaminaGrade;
                    newPlayer.StealGrade = playerGrade.StealGrade;
                    newPlayer.Surname = player.Surname;
                    newPlayer.ThreeGrade = playerGrade.ThreeGrade;
                    newPlayer.TwoGrade = playerGrade.TwoGrade;

                    draftPool.Add(newPlayer);
                }
            }
            return draftPool;
        }

        public async Task<IEnumerable<Player>> FilterByPosition(int filter, int leagueId)
        {
            List<Player> players = new List<Player>();

            if (filter == 1)
            {
                players = await _context.Players.Where(x => x.PGPosition == 1 && x.LeagueId == leagueId).OrderBy(x => x.Surname).ToListAsync();
            }
            else if (filter == 2)
            {
                players = await _context.Players.Where(x => x.SGPosition == 1 && x.LeagueId == leagueId).OrderBy(x => x.Surname).ToListAsync();
            }
            else if (filter == 3)
            {
                players = await _context.Players.Where(x => x.SFPosition == 1 && x.LeagueId == leagueId).OrderBy(x => x.Surname).ToListAsync();
            }
            else if (filter == 4)
            {
                players = await _context.Players.Where(x => x.PFPosition == 1 && x.LeagueId == leagueId).OrderBy(x => x.Surname).ToListAsync();
            }
            else if (filter == 5)
            {
                players = await _context.Players.Where(x => x.CPosition == 1 && x.LeagueId == leagueId).OrderBy(x => x.Surname).ToListAsync();
            }
            return players;
        }

        public async Task<IEnumerable<DraftPlayerDto>> FilterInitialDraftPlayerPool(string value, int leagueId)
        {
            List<DraftPlayerDto> draftPool = new List<DraftPlayerDto>();

            var query = String.Format("SELECT * FROM Players where (Surname like '%" + value + "%' or FirstName like '%" + value + "%') and LeagueId = " + leagueId);
            var players = await _context.Players.FromSqlRaw(query).ToListAsync();

            for (int i = 0; i < players.Count; i++)
            {
                var player = players[i];
                // NEED TO CHECK WHETHER THE PLAYER HAS BEEN DRAFTED
                var playerTeamForPlayerId = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == player.PlayerId && x.LeagueId == leagueId);

                if (playerTeamForPlayerId.TeamId == 31)
                {
                    var playerGrade = await _context.PlayerGradings.FirstOrDefaultAsync(x => x.PlayerId == player.PlayerId && x.LeagueId == leagueId);

                    // Now create the Dto
                    DraftPlayerDto newPlayer = new DraftPlayerDto();
                    newPlayer.PlayerId = player.PlayerId;
                    newPlayer.BlockGrade = playerGrade.BlockGrade;
                    newPlayer.CPosition = player.CPosition;
                    newPlayer.Age = player.Age;
                    newPlayer.DRebGrade = playerGrade.DRebGrade;
                    newPlayer.FirstName = player.FirstName;
                    newPlayer.FTGrade = playerGrade.FTGrade;
                    newPlayer.HandlingGrade = playerGrade.HandlingGrade;
                    newPlayer.IntangiblesGrade = playerGrade.IntangiblesGrade;
                    newPlayer.ORebGrade = playerGrade.ORebGrade;
                    newPlayer.PassingGrade = playerGrade.PassingGrade;
                    newPlayer.PFPosition = player.PFPosition;
                    newPlayer.PGPosition = player.PGPosition;
                    newPlayer.SFPosition = player.SFPosition;
                    newPlayer.SGPosition = player.SGPosition;
                    newPlayer.StaminaGrade = playerGrade.StaminaGrade;
                    newPlayer.StealGrade = playerGrade.StealGrade;
                    newPlayer.Surname = player.Surname;
                    newPlayer.ThreeGrade = playerGrade.ThreeGrade;
                    newPlayer.TwoGrade = playerGrade.TwoGrade;

                    draftPool.Add(newPlayer);
                }
            }
            return draftPool;
        }

        public async Task<IEnumerable<Player>> FilterPlayers(string value)
        {
            List<Player> players = new List<Player>();
            var query = String.Format("SELECT * FROM Players where Surname like '%" + value + "%' or FirstName like '%" + value + "%'");
            players = await _context.Players.FromSqlRaw(query).ToListAsync();
            return players;
        }

        public async Task<IEnumerable<Player>> GetAllPlayers(int leagueId)
        {
            var players = await _context.Players.Where(x => x.LeagueId == leagueId).ToListAsync();
            return players;
        }

        public async Task<IEnumerable<CareerStatsDto>> GetCareerStats(int playerId, int leagueId)
        {
            List<CareerStatsDto> careerStats = new List<CareerStatsDto>();
            var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == playerId && x.LeagueId == leagueId);
            var team = await _context.Teams.FirstOrDefaultAsync(x => x.TeamId == playerTeam.TeamId && x.LeagueId == leagueId);
            var currentSeasonPlayerStats = await _context.PlayerStats.FirstOrDefaultAsync(x => x.PlayerId == playerId && x.LeagueId == leagueId);
            var currentPlayoffStats = await _context.PlayerStatsPlayoffs.FirstOrDefaultAsync(x => x.PlayerId == playerId && x.LeagueId == leagueId);

            var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == leagueId);
            var pastSeasonStats = await _context.PlayerCareerStats.Where(x => x.PlayerId == playerId && x.LeagueId == leagueId).ToListAsync();
            var pastSeasonPlayoffStats = await _context.PlayerCareerStatsPlayoffs.Where(x => x.PlayerId == playerId && x.LeagueId == leagueId).ToListAsync();

            foreach (var season in pastSeasonStats)
            {
                var matchingPlayoffs = pastSeasonPlayoffStats.Find(x => x.SeasonId == season.SeasonId);

                if (matchingPlayoffs == null)
                {
                    CareerStatsDto csdto = new CareerStatsDto
                    {
                        PlayerId = season.PlayerId,
                        SeasonId = season.SeasonId,
                        TeamName = season.Team,
                        GamesStats = season.GamesPlayed,
                        MinutesStats = season.Minutes,
                        FgmStats = season.FieldGoalsMade,
                        FgaStats = season.FieldGoalsAttempted,
                        ThreeFgmStats = season.ThreeFieldGoalsMade,
                        ThreeFgaStats = season.ThreeFieldGoalsAttempted,
                        FtmStats = season.FreeThrowsMade,
                        FtaStats = season.FreeThrowsAttempted,
                        OrebsStats = season.ORebs,
                        DrebsStats = season.DRebs,
                        AstStats = season.Assists,
                        StlStats = season.Steals,
                        BlkStats = season.Blocks,
                        FlsStats = season.Fouls,
                        ToStats = season.Turnovers,
                        PtsStats = season.Points,
                        PlayoffGamesStats = matchingPlayoffs.GamesPlayed,
                        PlayoffMinutesStats = matchingPlayoffs.Minutes,
                        PlayoffFgmStats = matchingPlayoffs.FieldGoalsMade,
                        PlayoffFgaStats = matchingPlayoffs.FieldGoalsMade,
                        PlayoffThreeFgmStats = matchingPlayoffs.ThreeFieldGoalsMade,
                        PlayoffThreeFgaStats = matchingPlayoffs.ThreeFieldGoalsAttempted,
                        PlayoffFtmStats = matchingPlayoffs.FreeThrowsMade,
                        PlayoffFtaStats = matchingPlayoffs.FreeThrowsAttempted,
                        PlayoffOrebsStats = matchingPlayoffs.ORebs,
                        PlayoffDrebsStats = matchingPlayoffs.DRebs,
                        PlayoffAstStats = matchingPlayoffs.Assists,
                        PlayoffStlStats = matchingPlayoffs.Steals,
                        PlayoffBlkStats = matchingPlayoffs.Blocks,
                        PlayoffFlsStats = matchingPlayoffs.Fouls,
                        PlayoffToStats = matchingPlayoffs.Turnovers,
                        PlayoffPtsStats = matchingPlayoffs.Points
                    };
                    careerStats.Add(csdto);
                }
                else
                {
                    CareerStatsDto csdto = new CareerStatsDto
                    {
                        PlayerId = season.PlayerId,
                        SeasonId = season.SeasonId,
                        TeamName = season.Team,
                        GamesStats = season.GamesPlayed,
                        MinutesStats = season.Minutes,
                        FgmStats = season.FieldGoalsMade,
                        FgaStats = season.FieldGoalsAttempted,
                        ThreeFgmStats = season.ThreeFieldGoalsMade,
                        ThreeFgaStats = season.ThreeFieldGoalsAttempted,
                        FtmStats = season.FreeThrowsMade,
                        FtaStats = season.FreeThrowsAttempted,
                        OrebsStats = season.ORebs,
                        DrebsStats = season.DRebs,
                        AstStats = season.Assists,
                        StlStats = season.Steals,
                        BlkStats = season.Blocks,
                        FlsStats = season.Fouls,
                        ToStats = season.Turnovers,
                        PtsStats = season.Points,
                        PlayoffGamesStats = 0,
                        PlayoffMinutesStats = 0,
                        PlayoffFgmStats = 0,
                        PlayoffFgaStats = 0,
                        PlayoffThreeFgmStats = 0,
                        PlayoffThreeFgaStats = 0,
                        PlayoffFtmStats = 0,
                        PlayoffFtaStats = 0,
                        PlayoffOrebsStats = 0,
                        PlayoffDrebsStats = 0,
                        PlayoffAstStats = 0,
                        PlayoffStlStats = 0,
                        PlayoffBlkStats = 0,
                        PlayoffFlsStats = 0,
                        PlayoffToStats = 0,
                        PlayoffPtsStats = 0
                    };
                    careerStats.Add(csdto);
                }
            }

            // Now need to add the current season
            if (currentSeasonPlayerStats != null)
            {
                if (currentPlayoffStats == null)
                {
                    CareerStatsDto csdto = new CareerStatsDto
                    {
                        PlayerId = currentSeasonPlayerStats.PlayerId,
                        SeasonId = league.Year,
                        TeamName = team.Mascot,
                        GamesStats = currentSeasonPlayerStats.GamesPlayed,
                        MinutesStats = currentSeasonPlayerStats.Minutes,
                        FgmStats = currentSeasonPlayerStats.FieldGoalsMade,
                        FgaStats = currentSeasonPlayerStats.FieldGoalsAttempted,
                        ThreeFgmStats = currentSeasonPlayerStats.ThreeFieldGoalsMade,
                        ThreeFgaStats = currentSeasonPlayerStats.ThreeFieldGoalsAttempted,
                        FtmStats = currentSeasonPlayerStats.FreeThrowsMade,
                        FtaStats = currentSeasonPlayerStats.FreeThrowsAttempted,
                        OrebsStats = currentSeasonPlayerStats.ORebs,
                        DrebsStats = currentSeasonPlayerStats.DRebs,
                        AstStats = currentSeasonPlayerStats.Assists,
                        StlStats = currentSeasonPlayerStats.Steals,
                        BlkStats = currentSeasonPlayerStats.Blocks,
                        FlsStats = currentSeasonPlayerStats.Fouls,
                        ToStats = currentSeasonPlayerStats.Turnovers,
                        PtsStats = currentSeasonPlayerStats.Points,
                        PlayoffGamesStats = 0,
                        PlayoffMinutesStats = 0,
                        PlayoffFgmStats = 0,
                        PlayoffFgaStats = 0,
                        PlayoffThreeFgmStats = 0,
                        PlayoffThreeFgaStats = 0,
                        PlayoffFtmStats = 0,
                        PlayoffFtaStats = 0,
                        PlayoffOrebsStats = 0,
                        PlayoffDrebsStats = 0,
                        PlayoffAstStats = 0,
                        PlayoffStlStats = 0,
                        PlayoffBlkStats = 0,
                        PlayoffFlsStats = 0,
                        PlayoffToStats = 0,
                        PlayoffPtsStats = 0
                    };
                    careerStats.Add(csdto);
                }
                else
                {
                    CareerStatsDto csdto = new CareerStatsDto
                    {
                        PlayerId = currentSeasonPlayerStats.PlayerId,
                        SeasonId = league.Year,
                        TeamName = team.Mascot,
                        GamesStats = currentSeasonPlayerStats.GamesPlayed,
                        MinutesStats = currentSeasonPlayerStats.Minutes,
                        FgmStats = currentSeasonPlayerStats.FieldGoalsMade,
                        FgaStats = currentSeasonPlayerStats.FieldGoalsAttempted,
                        ThreeFgmStats = currentSeasonPlayerStats.ThreeFieldGoalsMade,
                        ThreeFgaStats = currentSeasonPlayerStats.ThreeFieldGoalsAttempted,
                        FtmStats = currentSeasonPlayerStats.FreeThrowsMade,
                        FtaStats = currentSeasonPlayerStats.FreeThrowsAttempted,
                        OrebsStats = currentSeasonPlayerStats.ORebs,
                        DrebsStats = currentSeasonPlayerStats.DRebs,
                        AstStats = currentSeasonPlayerStats.Assists,
                        StlStats = currentSeasonPlayerStats.Steals,
                        BlkStats = currentSeasonPlayerStats.Blocks,
                        FlsStats = currentSeasonPlayerStats.Fouls,
                        ToStats = currentSeasonPlayerStats.Turnovers,
                        PtsStats = currentSeasonPlayerStats.Points,
                        PlayoffGamesStats = currentPlayoffStats.GamesPlayed,
                        PlayoffMinutesStats = currentPlayoffStats.Minutes,
                        PlayoffFgmStats = currentPlayoffStats.FieldGoalsMade,
                        PlayoffFgaStats = currentPlayoffStats.FieldGoalsAttempted,
                        PlayoffThreeFgmStats = currentPlayoffStats.ThreeFieldGoalsMade,
                        PlayoffThreeFgaStats = currentPlayoffStats.ThreeFieldGoalsAttempted,
                        PlayoffFtmStats = currentPlayoffStats.FreeThrowsMade,
                        PlayoffFtaStats = currentPlayoffStats.FreeThrowsAttempted,
                        PlayoffOrebsStats = currentPlayoffStats.ORebs,
                        PlayoffDrebsStats = currentPlayoffStats.DRebs,
                        PlayoffAstStats = currentPlayoffStats.Assists,
                        PlayoffStlStats = currentPlayoffStats.Steals,
                        PlayoffBlkStats = currentPlayoffStats.Blocks,
                        PlayoffFlsStats = currentPlayoffStats.Fouls,
                        PlayoffToStats = currentPlayoffStats.Turnovers,
                        PlayoffPtsStats = currentPlayoffStats.Points
                    };
                    careerStats.Add(csdto);
                }
            }
            return careerStats;
        }

        public async Task<CompletePlayerDto> GetCompletePlayer(int playerId, int leagueId)
        {
            var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == leagueId);
            var playerDetails = await _context.Players.FirstOrDefaultAsync(x => x.PlayerId == playerId && x.LeagueId == leagueId);
            var playerRatings = await _context.PlayerRatings.FirstOrDefaultAsync(x => x.PlayerId == playerId && x.LeagueId == leagueId);
            var playerTendancies = await _context.PlayerTendancies.FirstOrDefaultAsync(x => x.PlayerId == playerId && x.LeagueId == leagueId);
            var playerGrades = await _context.PlayerGradings.FirstOrDefaultAsync(x => x.PlayerId == playerId && x.LeagueId == leagueId);
            var playerStats = await _context.PlayerStats.FirstOrDefaultAsync(x => x.PlayerId == playerId && x.LeagueId == leagueId);

            // need to get the players team
            var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == playerId && x.LeagueId == leagueId);
            var team = await _context.Teams.FirstOrDefaultAsync(x => x.TeamId == playerTeam.TeamId && x.LeagueId == leagueId);
            string teamname = "Free Agent";

            if (team != null)
            {
                teamname = team.Teamname + " " + team.Mascot;
            }

            if (playerStats != null)
            {
                PlayerStatsPlayoff psp = new PlayerStatsPlayoff();
                if (league.StateId > 8 || (league.StateId == 8 && league.StateId > 0))
                {
                    psp = await _context.PlayerStatsPlayoffs.FirstOrDefaultAsync(x => x.PlayerId == playerId && x.LeagueId == leagueId);

                    if (psp == null)
                    {
                        psp = new PlayerStatsPlayoff
                        {
                            PlayerId = playerId,
                            GamesPlayed = 0,
                            Minutes = 0,
                            Points = 0,
                            Rebounds = 0,
                            Assists = 0,
                            Steals = 0,
                            Blocks = 0,
                            FieldGoalsMade = 0,
                            FieldGoalsAttempted = 0,
                            ThreeFieldGoalsMade = 0,
                            ThreeFieldGoalsAttempted = 0,
                            FreeThrowsMade = 0,
                            FreeThrowsAttempted = 0,
                            ORebs = 0,
                            DRebs = 0,
                            Turnovers = 0,
                            Fouls = 0,
                            Ppg = 0,
                            Apg = 0,
                            Rpg = 0,
                            Spg = 0,
                            Bpg = 0,
                            Mpg = 0,
                            Fpg = 0,
                            Tpg = 0
                        };
                    }

                    CompletePlayerDto player = new CompletePlayerDto
                    {
                        PlayerId = playerId,
                        FirstName = playerDetails.FirstName,
                        Surname = playerDetails.Surname,
                        PGPosition = playerDetails.PGPosition,
                        SGPosition = playerDetails.SGPosition,
                        SFPosition = playerDetails.SFPosition,
                        PFPosition = playerDetails.PFPosition,
                        CPosition = playerDetails.CPosition,
                        Age = playerDetails.Age,
                        TwoGrade = playerGrades.TwoGrade,
                        ThreeGrade = playerGrades.ThreeGrade,
                        FTGrade = playerGrades.FTGrade,
                        ORebGrade = playerGrades.ORebGrade,
                        DRebGrade = playerGrades.DRebGrade,
                        StealGrade = playerGrades.StealGrade,
                        BlockGrade = playerGrades.BlockGrade,
                        StaminaGrade = playerGrades.StaminaGrade,
                        HandlingGrade = playerGrades.HandlingGrade,
                        TwoPointTendancy = playerTendancies.TwoPointTendancy,
                        ThreePointTendancy = playerTendancies.ThreePointTendancy,
                        PassTendancy = playerTendancies.PassTendancy,
                        FouledTendancy = playerTendancies.FouledTendancy,
                        TurnoverTendancy = playerTendancies.TurnoverTendancy,
                        TwoRating = playerRatings.TwoRating,
                        ThreeRating = playerRatings.ThreeRating,
                        FtRating = playerRatings.FTRating,
                        OrebRating = playerRatings.ORebRating,
                        DrebRating = playerRatings.DRebRating,
                        AssistRating = playerRatings.AssitRating,
                        PassAssistRating = playerRatings.PassAssistRating,
                        StealRating = playerRatings.StealRating,
                        BlockRating = playerRatings.BlockRating,
                        UsageRating = playerRatings.UsageRating,
                        StaminaRating = playerRatings.StaminaRating,
                        OrpmRating = playerRatings.ORPMRating,
                        DrpmRating = playerRatings.DRPMRating,
                        FoulingRating = playerRatings.FoulingRating,
                        PassingGrade = playerGrades.PassingGrade,
                        IntangiblesGrade = playerGrades.IntangiblesGrade,
                        TeamName = teamname,
                        GamesStats = playerStats.GamesPlayed,
                        MinutesStats = playerStats.Minutes,
                        FgmStats = playerStats.FieldGoalsMade,
                        FgaStats = playerStats.FieldGoalsAttempted,
                        ThreeFgmStats = playerStats.ThreeFieldGoalsMade,
                        ThreeFgaStats = playerStats.ThreeFieldGoalsAttempted,
                        FtmStats = playerStats.FreeThrowsMade,
                        FtaStats = playerStats.FreeThrowsAttempted,
                        OrebsStats = playerStats.ORebs,
                        DrebsStats = playerStats.DRebs,
                        AstStats = playerStats.Assists,
                        StlStats = playerStats.Steals,
                        BlkStats = playerStats.Blocks,
                        FlsStats = playerStats.Fouls,
                        ToStats = playerStats.Turnovers,
                        PtsStats = playerStats.Points,
                        PlayoffGamesStats = psp.GamesPlayed,
                        PlayoffMinutesStats = psp.Minutes,
                        PlayoffFgmStats = psp.FieldGoalsMade,
                        PlayoffFgaStats = psp.FieldGoalsAttempted,
                        PlayoffThreeFgmStats = psp.ThreeFieldGoalsMade,
                        PlayoffThreeFgaStats = psp.ThreeFieldGoalsAttempted,
                        PlayoffFtmStats = psp.FreeThrowsMade,
                        PlayoffFtaStats = psp.FreeThrowsAttempted,
                        PlayoffOrebsStats = psp.ORebs,
                        PlayoffDrebsStats = psp.DRebs,
                        PlayoffAstStats = psp.Assists,
                        PlayoffStlStats = psp.Steals,
                        PlayoffBlkStats = psp.Blocks,
                        PlayoffFlsStats = psp.Steals,
                        PlayoffToStats = psp.Turnovers,
                        PlayoffPtsStats = psp.Points
                    };
                    return player;
                }
                else
                {
                    CompletePlayerDto player = new CompletePlayerDto
                    {
                        PlayerId = playerId,
                        FirstName = playerDetails.FirstName,
                        Surname = playerDetails.Surname,
                        PGPosition = playerDetails.PGPosition,
                        SGPosition = playerDetails.SGPosition,
                        SFPosition = playerDetails.SFPosition,
                        PFPosition = playerDetails.PFPosition,
                        CPosition = playerDetails.CPosition,
                        Age = playerDetails.Age,
                        TwoGrade = playerGrades.TwoGrade,
                        ThreeGrade = playerGrades.ThreeGrade,
                        FTGrade = playerGrades.FTGrade,
                        ORebGrade = playerGrades.ORebGrade,
                        DRebGrade = playerGrades.DRebGrade,
                        StealGrade = playerGrades.StealGrade,
                        BlockGrade = playerGrades.BlockGrade,
                        StaminaGrade = playerGrades.StaminaGrade,
                        HandlingGrade = playerGrades.HandlingGrade,
                        TwoPointTendancy = playerTendancies.TwoPointTendancy,
                        ThreePointTendancy = playerTendancies.ThreePointTendancy,
                        PassTendancy = playerTendancies.PassTendancy,
                        FouledTendancy = playerTendancies.FouledTendancy,
                        TurnoverTendancy = playerTendancies.TurnoverTendancy,
                        TwoRating = playerRatings.TwoRating,
                        ThreeRating = playerRatings.ThreeRating,
                        FtRating = playerRatings.FTRating,
                        OrebRating = playerRatings.ORebRating,
                        DrebRating = playerRatings.DRebRating,
                        AssistRating = playerRatings.AssitRating,
                        PassAssistRating = playerRatings.PassAssistRating,
                        StealRating = playerRatings.StealRating,
                        BlockRating = playerRatings.BlockRating,
                        UsageRating = playerRatings.UsageRating,
                        StaminaRating = playerRatings.StaminaRating,
                        OrpmRating = playerRatings.ORPMRating,
                        DrpmRating = playerRatings.DRPMRating,
                        FoulingRating = playerRatings.FoulingRating,
                        PassingGrade = playerGrades.PassingGrade,
                        IntangiblesGrade = playerGrades.IntangiblesGrade,
                        TeamName = teamname,
                        GamesStats = playerStats.GamesPlayed,
                        MinutesStats = playerStats.Minutes,
                        FgmStats = playerStats.FieldGoalsMade,
                        FgaStats = playerStats.FieldGoalsAttempted,
                        ThreeFgmStats = playerStats.ThreeFieldGoalsMade,
                        ThreeFgaStats = playerStats.ThreeFieldGoalsAttempted,
                        FtmStats = playerStats.FreeThrowsMade,
                        FtaStats = playerStats.FreeThrowsAttempted,
                        OrebsStats = playerStats.ORebs,
                        DrebsStats = playerStats.DRebs,
                        AstStats = playerStats.Assists,
                        StlStats = playerStats.Steals,
                        BlkStats = playerStats.Blocks,
                        FlsStats = playerStats.Fouls,
                        ToStats = playerStats.Turnovers,
                        PtsStats = playerStats.Points,
                        PlayoffGamesStats = 0,
                        PlayoffMinutesStats = 0,
                        PlayoffFgmStats = 0,
                        PlayoffFgaStats = 0,
                        PlayoffThreeFgmStats = 0,
                        PlayoffThreeFgaStats = 0,
                        PlayoffFtmStats = 0,
                        PlayoffFtaStats = 0,
                        PlayoffOrebsStats = 0,
                        PlayoffDrebsStats = 0,
                        PlayoffAstStats = 0,
                        PlayoffStlStats = 0,
                        PlayoffBlkStats = 0,
                        PlayoffFlsStats = 0,
                        PlayoffToStats = 0,
                        PlayoffPtsStats = 0
                    };
                    return player;
                }
            }
            else
            {
                CompletePlayerDto player = new CompletePlayerDto
                {
                    PlayerId = playerId,
                    FirstName = playerDetails.FirstName,
                    Surname = playerDetails.Surname,
                    PGPosition = playerDetails.PGPosition,
                    SGPosition = playerDetails.SGPosition,
                    SFPosition = playerDetails.SFPosition,
                    PFPosition = playerDetails.PFPosition,
                    CPosition = playerDetails.CPosition,
                    Age = playerDetails.Age,
                    TwoGrade = playerGrades.TwoGrade,
                    ThreeGrade = playerGrades.ThreeGrade,
                    FTGrade = playerGrades.FTGrade,
                    ORebGrade = playerGrades.ORebGrade,
                    DRebGrade = playerGrades.DRebGrade,
                    StealGrade = playerGrades.StealGrade,
                    BlockGrade = playerGrades.BlockGrade,
                    StaminaGrade = playerGrades.StaminaGrade,
                    HandlingGrade = playerGrades.HandlingGrade,
                    TwoPointTendancy = playerTendancies.TwoPointTendancy,
                    ThreePointTendancy = playerTendancies.ThreePointTendancy,
                    PassTendancy = playerTendancies.PassTendancy,
                    FouledTendancy = playerTendancies.FouledTendancy,
                    TurnoverTendancy = playerTendancies.TurnoverTendancy,
                    TwoRating = playerRatings.TwoRating,
                    ThreeRating = playerRatings.ThreeRating,
                    FtRating = playerRatings.FTRating,
                    OrebRating = playerRatings.ORebRating,
                    DrebRating = playerRatings.DRebRating,
                    AssistRating = playerRatings.AssitRating,
                    PassAssistRating = playerRatings.PassAssistRating,
                    StealRating = playerRatings.StealRating,
                    BlockRating = playerRatings.BlockRating,
                    UsageRating = playerRatings.UsageRating,
                    StaminaRating = playerRatings.StaminaRating,
                    OrpmRating = playerRatings.ORPMRating,
                    DrpmRating = playerRatings.DRPMRating,
                    FoulingRating = playerRatings.FoulingRating,
                    PassingGrade = playerGrades.PassingGrade,
                    IntangiblesGrade = playerGrades.IntangiblesGrade,
                    TeamName = teamname,
                    GamesStats = 0,
                    MinutesStats = 0,
                    FgmStats = 0,
                    FgaStats = 0,
                    ThreeFgmStats = 0,
                    ThreeFgaStats = 0,
                    FtmStats = 0,
                    FtaStats = 0,
                    OrebsStats = 0,
                    DrebsStats = 0,
                    AstStats = 0,
                    StlStats = 0,
                    BlkStats = 0,
                    FlsStats = 0,
                    ToStats = 0,
                    PtsStats = 0,
                    PlayoffGamesStats = 0,
                    PlayoffMinutesStats = 0,
                    PlayoffFgmStats = 0,
                    PlayoffFgaStats = 0,
                    PlayoffThreeFgmStats = 0,
                    PlayoffThreeFgaStats = 0,
                    PlayoffFtmStats = 0,
                    PlayoffFtaStats = 0,
                    PlayoffOrebsStats = 0,
                    PlayoffDrebsStats = 0,
                    PlayoffAstStats = 0,
                    PlayoffStlStats = 0,
                    PlayoffBlkStats = 0,
                    PlayoffFlsStats = 0,
                    PlayoffToStats = 0,
                    PlayoffPtsStats = 0,
                };
                return player;
            }
        }

        public async Task<PlayerContractQuickViewDto> GetContractForPlayer(int playerId, int leagueId)
        {
            var playerContract = await _context.PlayerContracts.FirstOrDefaultAsync(x => x.PlayerId == playerId && x.LeagueId == leagueId);

            if (playerContract != null)
            {
                int years = 0;
                int total = 0;
                if (playerContract.YearFive > 0) {
                    years = 5;
                    total = playerContract.YearFive + playerContract.YearFour + playerContract.YearThree + playerContract.YearTwo + playerContract.YearOne;
                } else if (playerContract.YearFour > 0) {
                    years = 4;
                    total = playerContract.YearFour + playerContract.YearThree + playerContract.YearTwo + playerContract.YearOne;
                } else if (playerContract.YearThree > 0) {
                    years = 3;
                    total = playerContract.YearThree + playerContract.YearTwo + playerContract.YearOne;
                } else if (playerContract.YearTwo > 0) {
                    years = 2;
                    total = playerContract.YearTwo + playerContract.YearOne;
                } else if (playerContract.YearOne > 0) {
                    years = 1;
                    total = playerContract.YearOne;
                }

                PlayerContractQuickViewDto contract = new PlayerContractQuickViewDto
                {
                    PlayerId = playerId,
                    Years = years,
                    CurrentYearAmount = playerContract.YearOne,
                    TotalAmount = total
                };
                return contract;
            }
            else
            {
                PlayerContractQuickViewDto contract = new PlayerContractQuickViewDto
                {
                    PlayerId = playerId,
                    Years = 0,
                    CurrentYearAmount = 0,
                    TotalAmount = 0
                };
                return contract;
            }
        }

        public async Task<int> GetCountOfDraftPlayers(int leagueId)
        {
            var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == leagueId);
            if (league.StateId <= 5) {
                var count = await _context.PlayerTeams.Where(x => x.TeamId == 31 && x.LeagueId == leagueId).CountAsync();
                return count;
            } else if (league.StateId > 5) {
                // var count = await _context.PlayerTeams.Where(x => x.TeamId == 31).CountAsync();
                return 0; // NEED TO UPDATE THIS TO PULL FROM THE RIGHT TABLE
            }
            return 0;
        }

        public async Task<DetailedRetiredPlayerDto> GetDetailRetiredPlayer(int playerId)
        {
            var retiredPlayer = await _context.Players.FirstOrDefaultAsync(x => x.PlayerId == playerId);

            DetailedRetiredPlayerDto dto = new DetailedRetiredPlayerDto
            {
                PlayerId = playerId,
                FirstName = retiredPlayer.FirstName,
                Surname = retiredPlayer.Surname,
                PGPosition = retiredPlayer.PGPosition,
                SGPosition = retiredPlayer.SGPosition,
                SFPosition = retiredPlayer.SFPosition,
                PFPosition = retiredPlayer.PFPosition,
                CPosition = retiredPlayer.CPosition,
            };
            return dto;
        }

        public async Task<IEnumerable<Player>> GetFilteredFreeAgents(string filter, int leagueId)
        {
            List<Player> freeAgents = new List<Player>();
            var query = String.Format("SELECT * FROM Players where Surname like '%" + filter + "%' or FirstName like '%" + filter + "%' and LeagueId = " + leagueId);
            var players = await _context.Players.FromSqlRaw(query).ToListAsync();

            foreach (var player in players)
            {
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == player.PlayerId && x.LeagueId == leagueId);

                if (playerTeam.TeamId == 0)
                {
                    // Player is free agent
                    freeAgents.Add(player);
                }
            }
            return freeAgents;
        }

        public async Task<IEnumerable<Player>> GetFreeAgents(int leagueId)
        {
            List<Player> freeAgents = new List<Player>();
            // var players = await _context.Players.Where(x => x.LeagueId == leagueId).ToListAsync();
            var players = await _context.Players.
            Join(
                _context.PlayerTeams,
                player => new {player.PlayerId, player.LeagueId },
                playerTeam => new { playerTeam.PlayerId, playerTeam.LeagueId },
                (players, playerTeam) => new
                {
                    PlayerId = players.PlayerId,
                    FirstName = players.FirstName,
                    Surname = players.Surname,
                    LeagueId = players.LeagueId,
                    TeamId = playerTeam.TeamId,
                    Age = players.Age,
                    PGPosition = players.PGPosition,
                    SGPosition = players.SGPosition,
                    SFPosition = players.SFPosition,
                    PFPosition = players.PFPosition,
                    CPosition = players.CPosition,
                }
            ).Where(x => x.TeamId == 0).ToListAsync();

            foreach (var player in players)
            {
                Player p = new Player
                {
                    PlayerId = player.PlayerId,
                    FirstName = player.FirstName,
                    Surname = player.Surname,
                    Age = player.Age,
                    PGPosition = player.PGPosition,
                    SGPosition = player.SGPosition,
                    SFPosition = player.SFPosition,
                    PFPosition = player.PFPosition,
                    CPosition = player.CPosition
                };
                freeAgents.Add(p);
            }
            return freeAgents;

            // ASH TODO
            // var players = await _context.Players.Join(
            //     _context.PlayerTeams,
            //     player => new { player.PlayerId, player.LeagueId },
            //     playerTeam => new { playerTeam.PlayerId, playerTeam.LeagueId },
            //     (players, playerTeam) => new
            //     {
            //         PlayerId = players.PlayerId,
            //         FirstName = players.FirstName,
            //         Surname = players.Surname,
            //         LeagueId = players.LeagueId,
            //         TeamId = playerTeam.TeamId
            //     } 
            // ).Where(x => x.LeagueId == leagueId && x.TeamId == 31).OrderBy(x => x.Surname).ToListAsync();

            // foreach (var player in players)
            // {
            //     var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == player.PlayerId && x.LeagueId == leagueId);

            //     if (playerTeam.TeamId == 0)
            //     {
            //         // Player is free agent
            //         freeAgents.Add(player);
            //     }
            // }
            
        }

        public async Task<IEnumerable<Player>> GetFreeAgentsByPos(PlayerIdLeagueDto pos)
        {
            List<Player> freeAgents = new List<Player>();
            List<Player> players = new List<Player>();

            if (pos.PlayerId == 1)
            {
                players = await _context.Players.Where(x => x.PGPosition == 1 && x.LeagueId == pos.LeagueId).OrderBy(x => x.Surname).ToListAsync();
            }
            else if (pos.PlayerId == 2)
            {
                players = await _context.Players.Where(x => x.SGPosition == 1 && x.LeagueId == pos.LeagueId).OrderBy(x => x.Surname).ToListAsync();
            }
            else if (pos.PlayerId == 3)
            {
                players = await _context.Players.Where(x => x.SFPosition == 1 && x.LeagueId == pos.LeagueId).OrderBy(x => x.Surname).ToListAsync();
            }
            else if (pos.PlayerId == 4)
            {
                players = await _context.Players.Where(x => x.PFPosition == 1 && x.LeagueId == pos.LeagueId).OrderBy(x => x.Surname).ToListAsync();
            }
            else if (pos.PlayerId == 5)
            {
                players = await _context.Players.Where(x => x.CPosition == 1 && x.LeagueId == pos.LeagueId).OrderBy(x => x.Surname).ToListAsync();
            }

            foreach (var player in players)
            {
                var playerTeam = await _context.PlayerTeams.FirstOrDefaultAsync(x => x.PlayerId == player.PlayerId && x.LeagueId == pos.LeagueId);

                if (playerTeam.TeamId == 0)
                {
                    // Player is free agent
                    freeAgents.Add(player);
                }
            }
            return freeAgents;
        }

        public async Task<PlayerContract> GetFullContractForPlayer(int playerId, int leagueId)
        {
            var contract = await _context.PlayerContracts.FirstOrDefaultAsync(x => x.PlayerId == playerId && x.LeagueId == leagueId);
            return contract;
        }

        public async Task<IEnumerable<DraftPlayerDto>> GetInitialDraftPlayerPoolPage(int page, int leagueId)
        {
            List<DraftPlayerDto> draftPool = new List<DraftPlayerDto>();

            // Get players
            int start = (page * 50) - 50;
            int end = (page * 50);
            var players = await _context.Players.Where(x => x.LeagueId == leagueId).OrderBy(x => x.Surname).ToListAsync();
            int total = players.Count;

            if (end > total)
            {
                end = total;
            }

            // foreach (var player in players)- 1;
            for (int i = start; i < end; i++)
            {
                var player = players[i];
                // NEED TO CHECK WHETHER THE PLAYER HAS BEEN DRAFTED
                var playerTeamForPlayerId = await _context.PlayerTeams.Where(x => x.LeagueId == leagueId).FirstOrDefaultAsync(x => x.PlayerId == player.PlayerId);

                if (playerTeamForPlayerId.TeamId == 31)
                {
                    var playerGrade = await _context.PlayerGradings.FirstOrDefaultAsync(x => x.PlayerId == player.PlayerId && x.LeagueId == leagueId);

                    // Now create the Dto
                    DraftPlayerDto newPlayer = new DraftPlayerDto();
                    newPlayer.PlayerId = player.PlayerId;
                    newPlayer.BlockGrade = playerGrade.BlockGrade;
                    newPlayer.CPosition = player.CPosition;
                    newPlayer.Age = player.Age;
                    newPlayer.DRebGrade = playerGrade.DRebGrade;
                    newPlayer.FirstName = player.FirstName;
                    newPlayer.FTGrade = playerGrade.FTGrade;
                    newPlayer.HandlingGrade = playerGrade.HandlingGrade;
                    newPlayer.IntangiblesGrade = playerGrade.IntangiblesGrade;
                    newPlayer.ORebGrade = playerGrade.ORebGrade;
                    newPlayer.PassingGrade = playerGrade.PassingGrade;
                    newPlayer.PFPosition = player.PFPosition;
                    newPlayer.PGPosition = player.PGPosition;
                    newPlayer.SFPosition = player.SFPosition;
                    newPlayer.SGPosition = player.SGPosition;
                    newPlayer.StaminaGrade = playerGrade.StaminaGrade;
                    newPlayer.StealGrade = playerGrade.StealGrade;
                    newPlayer.Surname = player.Surname;
                    newPlayer.ThreeGrade = playerGrade.ThreeGrade;
                    newPlayer.TwoGrade = playerGrade.TwoGrade;

                    draftPool.Add(newPlayer);
                }
            }
            return draftPool;
        }

        public async Task<IEnumerable<DraftSelectionPlayerDto>> GetInitialDraftSelectionPlayerPool(int leagueId)
        {
            List<DraftSelectionPlayerDto> draftPool = new List<DraftSelectionPlayerDto>();
            // Get players
            var players = await _context.Players.Join(
                _context.PlayerTeams,
                player => new { player.PlayerId, player.LeagueId },
                playerTeam => new { playerTeam.PlayerId, playerTeam.LeagueId },
                (players, playerTeam) => new
                {
                    PlayerId = players.PlayerId,
                    FirstName = players.FirstName,
                    Surname = players.Surname,
                    LeagueId = players.LeagueId,
                    TeamId = playerTeam.TeamId
                } 
            ).Where(x => x.LeagueId == leagueId && x.TeamId == 31).OrderBy(x => x.Surname).ToListAsync();

            foreach (var player in players)
            {
                DraftSelectionPlayerDto newPlayer = new DraftSelectionPlayerDto();
                newPlayer.PlayerId = player.PlayerId;
                newPlayer.FirstName = player.FirstName;
                newPlayer.Surname = player.Surname;
                newPlayer.LeagueId = player.LeagueId;
                draftPool.Add(newPlayer);
            }
           return draftPool;
        }

        public async Task<IEnumerable<DraftPlayerDto>> GetInitialDraftPlayerPool(int leagueId)
        {
            List<DraftPlayerDto> draftPool = new List<DraftPlayerDto>();

            var players = await _context.Players
            .Join(
                _context.PlayerTeams,
                player => new { player.PlayerId, player.LeagueId },
                playerTeam => new { playerTeam.PlayerId, playerTeam.LeagueId },
                (players, playerTeam) => new
                {
                    PlayerId = players.PlayerId,
                    FirstName = players.FirstName,
                    Surname = players.Surname,
                    TeamId = playerTeam.TeamId,
                    LeagueId = players.LeagueId,
                    Age = players.Age,
                    PGPosition = players.PGPosition,
                    SGPosition = players.SGPosition,
                    SFPosition = players.SFPosition,
                    PFPosition = players.PFPosition,
                    CPosition = players.CPosition
                } 
            )
            .Join(
                _context.PlayerGradings,
                combined => new { combined.PlayerId, combined.LeagueId },
                gradings => new { gradings.PlayerId, gradings.LeagueId },
                (combined, gradings) => new
                {
                    PlayerId = combined.PlayerId,
                    FirstName = combined.FirstName,
                    Surname = combined.Surname,
                    TeamId = combined.TeamId,
                    LeagueId = combined.LeagueId,
                    Age = combined.Age,
                    PGPosition = combined.PGPosition,
                    SGPosition = combined.SGPosition,
                    SFPosition = combined.SFPosition,
                    PFPosition = combined.PFPosition,
                    CPosition = combined.CPosition,
                    BlockGrade = gradings.BlockGrade,
                    DRebGrade = gradings.DRebGrade,
                    FTGrade = gradings.FTGrade,
                    HandlingGrade = gradings.HandlingGrade,
                    IntangiblesGrade = gradings.IntangiblesGrade,
                    ORebGrade = gradings.ORebGrade,
                    PassingGrade = gradings.PassingGrade,
                    StaminaGrade = gradings.StaminaGrade,
                    StealGrade = gradings.StealGrade,
                    ThreeGrade = gradings.ThreeGrade,
                    TwoGrade = gradings.TwoGrade
                }
            ).Where(x => x.LeagueId == leagueId && x.TeamId == 31).OrderBy(x => x.Surname).ToListAsync();

            foreach (var player in players)
            {
                // Now create the Dto
                DraftPlayerDto newPlayer = new DraftPlayerDto();
                newPlayer.PlayerId = player.PlayerId;
                newPlayer.BlockGrade = player.BlockGrade;
                newPlayer.CPosition = player.CPosition;
                newPlayer.Age = player.Age;
                newPlayer.DRebGrade = player.DRebGrade;
                newPlayer.FirstName = player.FirstName;
                newPlayer.FTGrade = player.FTGrade;
                newPlayer.HandlingGrade = player.HandlingGrade;
                newPlayer.IntangiblesGrade = player.IntangiblesGrade;
                newPlayer.ORebGrade = player.ORebGrade;
                newPlayer.PassingGrade = player.PassingGrade;
                newPlayer.PFPosition = player.PFPosition;
                newPlayer.PGPosition = player.PGPosition;
                newPlayer.SFPosition = player.SFPosition;
                newPlayer.SGPosition = player.SGPosition;
                newPlayer.StaminaGrade = player.StaminaGrade;
                newPlayer.StealGrade = player.StealGrade;
                newPlayer.Surname = player.Surname;
                newPlayer.ThreeGrade = player.ThreeGrade;
                newPlayer.TwoGrade = player.TwoGrade;

                draftPool.Add(newPlayer);
            }
            return draftPool;
        }

        public async Task<Player> GetPlayerForId(int playerId)
        {
            var player = await _context.Players.FirstOrDefaultAsync(x => x.PlayerId == playerId);
            return player;
        }

        public async Task<Player> GetPlayerForName(string playername, int leagueId)
        {
            string[] components = playername.Split(' ');
            int componentCount = components.Length;

            string first = "";
            string surname = "";

            if (componentCount == 2)
            {
                // What if there is a 2 word last name?
                first = components[0];
                surname = components[1];

                var p = await _context.Players.FirstOrDefaultAsync(x => x.FirstName == first && x.Surname == surname && x.LeagueId == leagueId);
                return p;
            }
            else
            {
                return null;
            }
        }

        public async Task<IEnumerable<RetiredPlayer>> GetRetiredPlayers()
        {
            var retiredPlayers = await _context.RetiredPlayers.ToListAsync();
            return retiredPlayers;
        }
    }
}