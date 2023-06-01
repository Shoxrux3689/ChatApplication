using ChatData.Context;
using ChatData.Models;
using IdentityApi.Data.Context;
using IdentityApi.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChatData.Entities;
using System.Security.Claims;
using Mapster;

namespace ButunYozish.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly ChatDbContext chatDb;
    private readonly IdentityDbContext identityDb;

    public ChatController(ChatDbContext chatDbContext, IdentityDbContext identityDbContext)
    {
        chatDb = chatDbContext;
        identityDb = identityDbContext;
    }

    [HttpPost("sendmessage")]
    public async Task<IActionResult> SendAndSaveMessage(NewMessageModel newMessage)
    {
        if (newMessage.Text == null || (newMessage.ToUserId == null && newMessage.ChatId == null))
        {
            return BadRequest("yetib kemayapti");
        }

		var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

		var chat = await chatDb.Chats.FirstOrDefaultAsync(c => c.Id == newMessage.ChatId);

        if (chat == null)
        {
            var fromUser = identityDb.Users.FirstOrDefault(u => 
                u.Id == userId);

            var toUser = identityDb.Users.FirstOrDefault(u => u.Id == newMessage.ToUserId);

            chat = new Chat()
            {
                Users = new List<User>()
                {
                     fromUser!,
                     toUser!,
                }
            };

            chatDb.Chats.Add(chat);
            await chatDb.SaveChangesAsync();

            chat = await chatDb.Chats
                .FirstOrDefaultAsync(c => c.Users.Count == 2 && 
                c.Users.Any(u => u.Id == fromUser!.Id) && 
                c.Users.Any(u => u.Id == toUser!.Id));
        }


        var message = new Message()
        {
            ChatId = chat!.Id,
            Text = newMessage.Text,
            FromUserId = userId,
            Date = DateTime.UtcNow,
        };

        chat.Messages.Add(message);
        chatDb.Messages.Add(message);
        await chatDb.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("chats")]
    public IActionResult GetChats()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var chats = chatDb.Chats.Where(c => c.Users.Any(u => u.Id == userId)).ToList();

        var chatModels = new List<ChatModel>();

        foreach (var chat in chats)
        {
			var chatModel = new ChatModel()
			{
				Id = chat.Id,
			};
            chatModels.Add(chatModel);
		}

        return Ok(chatModels);
    }

    [HttpGet("chat")]
    public IActionResult GetChat(Guid chatId)
    {
        var chat = chatDb.Chats.Include(c => c.Messages).FirstOrDefault(c => c.Id == chatId);

        //var chatModel = chat!.Adapt<T>();
        var messages = chat.Messages.Adapt<List<MessageModel>>();

        return Ok(messages);
    }
}
