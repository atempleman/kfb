using System.Collections.Generic;
using System.Threading.Tasks;
using ABASim.api.Dtos;

namespace ABASim.api.Data
{
    public interface IContactRepository
    {
        Task<bool> SaveContactForm(ContactFormDto contactFormDto);

        Task<bool> SaveChatRecord(GlobalChatDto chatDto);

        Task<IEnumerable<GlobalChatDto>> GetChatRecords(int leagueId);

        Task<IEnumerable<InboxMessageDto>> GetInboxMessages(int teamId, int leagueId);

        Task<bool> SendInboxMessage(InboxMessageDto message);

        Task<bool> MarkMessageRead(int messageId);

        Task<bool> DeleteInboxMessage(int messageId);

        Task<InboxMessageCountDto> GetCountOfMessages(int teamId, int leagueId);
    }
}