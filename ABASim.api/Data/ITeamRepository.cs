using System.Collections.Generic;
using System.Threading.Tasks;
using ABASim.api.Dtos;
using ABASim.api.Models;

namespace ABASim.api.Data
{
    public interface ITeamRepository
    {
         Task<bool> CheckForAvailableTeams();

         Task<IEnumerable<Team>> GetAvailableTeams();

         Task<IEnumerable<Team>> GetAvailableTeamsForPrivate(string leagueCode);

         Task<Team> GetTeamForUserId(int userId);

         Task<Team> GetTeamForTeamId(int teamId, int leagueId);

         Task<IEnumerable<Player>> GetRosterForTeam(int teamId, int leagueId);

         Task<IEnumerable<Team>> GetAllTeams(int leagueId);

         Task<IEnumerable<Team>> GetTeamInitialLotteryOrder(int leagueId);

         Task<Team> GetTeamForTeamName(string teamname, int leagueId);

         Task<Team> GetTeamForTeamMascot(string teamname, int leagueId);

         Task<IEnumerable<DepthChart>> GetDepthChartForTeam(int teamId, int leagueId);

         Task<bool> SaveDepthChartForTeam(DepthChart[] charts);

         Task<bool> RosterSpotCheck(GetRosterQuickViewDto dto);

         Task<IEnumerable<CompletePlayerDto>> GetExtendPlayersForTeam(int teamId, int leagueId);

         Task<IEnumerable<QuickViewPlayerDto>> GetQuickViewRoster(int teamId, int leagueId);

         Task<IEnumerable<LeaguePlayerInjuryDto>> GetTeamInjuries(int teamId, int leagueId);

         Task<bool> WaivePlayer(WaivePlayerDto waived);

         Task<bool> SignPlayer(SignedPlayerDto signed);

         Task<CoachSetting> GetCoachSettingForTeamId(int teamId, int leagueId);

         Task<bool> SaveCoachingSetting(CoachSetting setting);

         Task<IEnumerable<Team>> GetAllTeamsExceptUsers(int teamId, int leagueId);

        Task<IEnumerable<TradeDto>> GetTradeOffers(int teamId, int leagueId);

         Task<bool> SaveTradeProposal(TradeDto[] trades);

         Task<bool> RejectTradeProposal(TradeMessageDto message);

        Task<bool> AcceptTradeProposal(int tradeId);

        Task<bool> PullTradeProposal(int tradeId);

        Task<TradeMessageDto> GetTradeMessage(int tradeId);

        Task<IEnumerable<TeamDraftPickDto>> GetTeamsDraftPicks(int teamId, int leagueId);

        Task<IEnumerable<PlayerInjury>> GetPlayerInjuriesForTeam(int teamId, int leagueId);

        Task<IEnumerable<PlayerInjury>> GetInjuriesForFreeAgents(int leagueId);

        Task<PlayerInjury> GetInjuryForPlayer(int playerId, int teamId);

        Task<TeamSalaryCapInfo> GetTeamSalaryCapDetails(int teamId, int leagueId);

        Task<IEnumerable<PlayerContractDetailedDto>> GetTeamContracts(int teamId, int leagueId);

        Task<IEnumerable<OffensiveStrategy>> GetOffensiveStrategies();

        Task<IEnumerable<DefensiveStrategy>> GetDefensiveStrategies();

        Task<TeamStrategyDto> GetStrategyForTeam(int teamId, int leagueId);

        Task<bool> SaveStrategy(TeamStrategyDto strategy);

        Task<bool> SaveContractOffer(ContractOfferDto offer);

        Task<IEnumerable<ContractOfferDto>> GetContractOffersForTeam(int teamId, int leagueId);

        Task<bool> DeleteFreeAgentOffer(int contractId);

        Task<IEnumerable<WaivedContractDto>> GetWaivedContracts(int teamId, int leagueId);

        Task<IEnumerable<TradePlayerViewDto>> GetTradePlayerViews(int teamId, int leagueId);

        Task<StandingsDto> GetTeamRecord(int teamId, int leagueId);
    }
}