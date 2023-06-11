using ChatData.Context;
using ChatData.Entities;
using ChatData.Models;
using ChatData.Services;
using IdentityApi.Data.Context;
using IdentityApi.Data.Entities;
using IdentityApi.Data.Providers;
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
	private readonly UserProvider _userProvider;
	private readonly ChatService _chatService;
	public ChatController(ChatDbContext chatDbContext, UserProvider userProvider, IdentityDbContext identityDbContext, ChatService chatService)
	{
		chatDb = chatDbContext;
		identityDb = identityDbContext;
		_userProvider = userProvider;
		_chatService = chatService;
	}

	
	[HttpPost("sendmessage")]
	public async Task SendAndSaveMessage(NewMessageModel newMessage)
	{
		await _chatService.SaveAndSendMessage(newMessage);
	}
	
	[HttpGet("chats")]
	public async Task<List<ChatModel>> GetChats()
	{   
		return await _chatService.GetChatsForUser();
	}

	[HttpGet("chat")]
	public async Task<ChatModel?> GetChat(Guid chatId)
	{
		return await _chatService.GetChatForUser(chatId);
	}
}
