using ButunYozish.Hubs;
using ChatData.Context;
using ChatData.Entities;
using ChatData.Models;
using IdentityApi.Data.Context;
using IdentityApi.Data.Entities;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ButunYozish.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ChatController : ControllerBase
{
	private readonly ChatDbContext chatDb;
	private readonly IdentityDbContext identityDb;
	private readonly IHubContext<ChatHub> chatHubContext;
	public ChatController(ChatDbContext chatDbContext, IdentityDbContext identityDbContext, IHubContext<ChatHub> chat)
	{
		chatDb = chatDbContext;
		identityDb = identityDbContext;
		chatHubContext = chat;
	}

	
	[HttpPost("sendmessage")]
	public async Task SendAndSaveMessage(NewMessageModel newMessage)
	{
		if (newMessage.Text == null || (newMessage.ToUserId == null && newMessage.ChatId == null))
		{
			throw new Exception("yetib kemayapti");
		}
		var chat = await chatDb.Chats.Include(c => c.UserIds).FirstOrDefaultAsync(c => c.Id == newMessage.ChatId);

		var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

		if (chat == null && newMessage.ToUserId != null)
		{
			chat = new Chat()
			{
				IsPersonal = true,
			};

			chatDb.Chats.Add(chat);
			await chatDb.SaveChangesAsync();

			var userIds = new UserIds()
			{
				UserId = userId,
				ChatId = chat.Id,
				IsAdmin = false,
			};
			var userIds2 = new UserIds()
			{
				UserId = newMessage.ToUserId.Value,
				ChatId = chat.Id,
				IsAdmin = false,
			};

			var message = new Message()
			{
				Text = newMessage.Text,
				ChatId = chat.Id,
				FromUserId = userId,
				ToUserId = newMessage.ToUserId,
			};

			chatDb.UserIds.Add(userIds);
			chatDb.UserIds.Add(userIds2);
			chatDb.Messages.Add(message);
			await chatDb.SaveChangesAsync();

			var messageModel = message.Adapt<MessageModel>();

			//example uchun qoldirdim. connection orniga Users metodini ishlatsa ham boladi
			var connectionId = UserConnectionIdService.ConnectionIds.FirstOrDefault(c => c.Item1 == userId)?.Item2;
			if (connectionId != null)
			{
				await chatHubContext.Clients
					.Client(connectionId)
					.SendAsync("NewMessage", messageModel);
			}

			var connectionId2 = UserConnectionIdService.ConnectionIds.FirstOrDefault(c => c.Item1 != userId)?.Item2;
			if (connectionId2 != null)
			{
				await chatHubContext.Clients
					.Client(connectionId2)
					.SendAsync("NewMessage", messageModel);
			}
		}
		else if (chat != null)
		{
			var message = new Message()
			{
				Text = newMessage.Text,
				ChatId = chat.Id,
				FromUserId = userId,
				ToUserId = newMessage.ToUserId,
			};

			chatDb.Messages.Add(message);
			await chatDb.SaveChangesAsync();

			var messageModel = message.Adapt<MessageModel>();

			await chatHubContext.Clients
				.Users(chat.UserIds.Select(u => u.UserId.ToString()))
				.SendAsync("NewMessage", messageModel);

			//await chatHubContext.Clients.All.SendAsync("NewMessage", messageModel);
		}
		else
		{
			throw new Exception("Bunaqa chat yogu ahmoq");
			//shuning uchun group chat oldindan yaratilgan bolishi shart
		}
	}
	
	[HttpGet("chats")]
	public async Task<List<ChatModel>> GetChats()
	{
		var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

		var chats = await chatDb.Chats
			.Include(c => c.UserIds)
			.Where(c => c.UserIds.Any(u => u.UserId == userId))
			.ToListAsync();

		var chatModels = chats.Adapt<List<ChatModel>>();

		return chatModels;
	}

	[HttpGet("chat")]
	public async Task<List<MessageModel>> GetChat(Guid chatId)
	{
		var chat = await chatDb.Chats.Include(c => c.Messages).FirstOrDefaultAsync(c => c.Id == chatId);

		//var chatModel = chat!.Adapt<T>();
		var messages = chat!.Messages!.Adapt<List<MessageModel>>();

		return messages;
	}
}
