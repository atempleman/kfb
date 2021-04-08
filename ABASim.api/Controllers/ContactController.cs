using System;
using System.Threading.Tasks;
using ABASim.api.Data;
using ABASim.api.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ABASim.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContactRepository _repo;

        public ContactController(IContactRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("savecontact")]
        public async Task<IActionResult> SaveContact(ContactFormDto contactFormDto)
        {
            var createdForm = await _repo.SaveContactForm(contactFormDto);
            return StatusCode(201);
        }

        [HttpPost("savechatrecord")]
        public async Task<IActionResult> SaveChatRecord(GlobalChatDto chatDto)
        {
            var result = await _repo.SaveChatRecord(chatDto);
            return Ok(result);
        }

        [HttpGet("getchatrecords/{leagueId}")]
        public async Task<IActionResult> GetChatRecords(int leagueId)
        {
            var records = await _repo.GetChatRecords(leagueId);
            return Ok(records);
        }

        [HttpGet("getinboxmessages")]
        public async Task<IActionResult> GetInboxMessages(string teamId, string leagueId)
        {
            var result = await _repo.GetInboxMessages(Int32.Parse(teamId), Int32.Parse(leagueId));
            return Ok(result);
        }

        [HttpPost("sendinboxmessage")]
        public async Task<IActionResult> SendInboxMessage(InboxMessageDto message)
        {
            var result = await _repo.SendInboxMessage(message);
            return Ok(result);
        }

        [HttpGet("deletemessage/{messageId}")]
        public async Task<IActionResult> DeleteInboxMessage(int messageId)
        {
            var result = await _repo.DeleteInboxMessage(messageId);
            return Ok(result);
        }

        [HttpGet("getMessageCount")]
        public async Task<IActionResult> GetMessageCount(string teamId, string leagueId)
        {
            var count = await _repo.GetCountOfMessages(Int32.Parse(teamId), Int32.Parse(leagueId));
            return Ok(count);
        }

        [HttpGet("markasread/{messageId}")]
        public async Task<IActionResult> MarkMessageRead(int messageId)
        {
            var result = await _repo.MarkMessageRead(messageId);
            return Ok(result);
        }
    }
}