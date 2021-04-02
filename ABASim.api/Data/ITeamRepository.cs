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

         Task<Team> GetTeamForTeamId(GetRosterQuickViewDto dto);

         Task<IEnumerable<Player>> GetRosterForTeam(GetRosterQuickViewDto dto);

         Task<IEnumerable<Team>> GetAllTeams(int leagueId);

         Task<IEnumerable<Team>> GetTeamInitialLotteryOrder(int leagueId);

         Task<Team> GetTeamForTeamName(TeamNameLeagueDto dto);

         Task<Team> GetTeamForTeamMascot(TeamNameLeagueDto name);

         Task<IEnumerable<DepthChart>> GetDepthChartForTeam(int teamId);

         Task<bool> SaveDepthChartForTeam(DepthChart[] charts);

         Task<bool> RosterSpotCheck(GetRosterQuickViewDto dto);

         Task<IEnumerable<CompletePlayerDto>> GetExtendPlayersForTeam(GetRosterQuickViewDto dto);

         Task<IEnumerable<QuickViewPlayerDto>> GetQuickViewRoster(GetRosterQuickViewDto quickview);

         Task<IEnumerable<LeaguePlayerInjuryDto>> GetTeamInjuries(GetRosterQuickViewDto quickview);

         Task<bool> WaivePlayer(WaivePlayerDto waived);

         Task<bool> SignPlayer(SignedPlayerDto signed);

         Task<CoachSetting> GetCoachSettingForTeamId(GetRosterQuickViewDto dto);

         Task<bool> SaveCoachingSetting(CoachSetting setting);

         Task<IEnumerable<Team>> GetAllTeamsExceptUsers(GetRosterQuickViewDto dto);

        Task<IEnumerable<TradeDto>> GetTradeOffers(GetRosterQuickViewDto dto);

         Task<bool> SaveTradeProposal(TradeDto[] trades);

         Task<bool> RejectTradeProposal(TradeMessageDto message);

        Task<bool> AcceptTradeProposal(int tradeId);

        Task<bool> PullTradeProposal(int tradeId);

        Task<TradeMessageDto> GetTradeMessage(int tradeId);

        Task<IEnumerable<TeamDraftPickDto>> GetTeamsDraftPicks(GetRosterQuickViewDto dto);

        Task<IEnumerable<PlayerInjury>> GetPlayerInjuriesForTeam(GetRosterQuickViewDto dto);

        Task<IEnumerable<PlayerInjury>> GetInjuriesForFreeAgents(int leagueId);

        Task<PlayerInjury> GetInjuryForPlayer(PlayerIdLeagueDto dto);

        Task<TeamSalaryCapInfo> GetTeamSalaryCapDetails(GetRosterQuickViewDto dto);

        Task<IEnumerable<PlayerContractDetailedDto>> GetTeamContracts(GetRosterQuickViewDto dto);

        Task<IEnumerable<OffensiveStrategy>> GetOffensiveStrategies();

        Task<IEnumerable<DefensiveStrategy>> GetDefensiveStrategies();

        Task<TeamStrategyDto> GetStrategyForTeam(GetRosterQuickViewDto dto);

        Task<bool> SaveStrategy(TeamStrategyDto strategy);

        Task<bool> SaveContractOffer(ContractOfferDto offer);

        Task<IEnumerable<ContractOfferDto>> GetContractOffersForTeam(int teamId);

        Task<bool> DeleteFreeAgentOffer(int contractId);

        Task<IEnumerable<WaivedContractDto>> GetWaivedContracts(GetRosterQuickViewDto dto);

        Task<IEnumerable<TradePlayerViewDto>> GetTradePlayerViews(GetRosterQuickViewDto dto);

        Task<StandingsDto> GetTeamRecord(GetRosterQuickViewDto dto);
    }
}