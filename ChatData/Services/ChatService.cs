using ChatData.Context;
using ChatData.Entities;
using ChatData.Models;
using IdentityApi.Data.Providers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Mapster;
using ChatData.Hubs;
using System;
using IdentityApi.Data.Entities;
using IdentityApi.Data.Context;

namespace ChatData.Services;

public class ChatService
{
    private readonly ChatDbContext chat_db;
    private readonly IHubContext<ChatHub> chatHub;
    private readonly Guid _userId;
    private readonly IdentityDbContext _identityContext;

    public ChatService(ChatDbContext Chat_db,
        IHubContext<ChatHub> ChatHub, 
        UserProvider userProvider,
        IdentityDbContext identityDbContext)
    {
        chat_db = Chat_db;
        chatHub = ChatHub;
        _userId = userProvider.UserId;
        _identityContext = identityDbContext;
    }

    public async Task SaveAndSendMessage(NewMessageModel newMessage)
    {
        if (newMessage.Text == "" || (newMessage.ToUserId == null && newMessage.ChatId == null))
        {
            throw new Exception("yetib kemayapti");
        }

        var chat = await GetChatWithUserids(newMessage.ChatId.Value);

        if (chat == null && newMessage.ToUserId != null)
        {
            chat = await CreatePersonalChat();

            await CreateUserIds(chat.Id, _userId);
            await CreateUserIds(chat.Id, newMessage.ToUserId.Value);

            var message = await CreateMessage(newMessage, chat.Id);

            var messageModel = message.Adapt<MessageModel>();

            //example uchun qoldirdim. connection orniga Users metodini ishlatsa ham boladi
            var connectionId = UserConnectionIdService.ConnectionIds.FirstOrDefault(c => c.Item1 == _userId)?.Item2;
            if (connectionId != null)
            {
                await chatHub.Clients
                    .Client(connectionId)
                    .SendAsync("NewMessage", messageModel);
            }

            var connectionId2 = UserConnectionIdService.ConnectionIds.FirstOrDefault(c => c.Item1 != _userId)?.Item2;
            if (connectionId2 != null)
            {
                await chatHub.Clients
                    .Client(connectionId2)
                    .SendAsync("NewMessage", messageModel);
            }
        }
        else if (chat != null)
        {
            var message = await CreateMessage(newMessage, chat.Id);

            var messageModel = message.Adapt<MessageModel>();

            await chatHub.Clients
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

    public async Task<ChatModel> GetChatForUser(Guid chatId)
    {
        var chat = await GetChatWithMessages(chatId);

        if (chat == null)
        {
            throw new Exception("chat not found");
        }

        var chatModel = chat!.Adapt<ChatModel>();
        chatModel.Messages = chat!.Messages!.Adapt<List<MessageModel>>();
        chatModel.CurrentUserId = _userId;

        return chatModel;
    }

    public async Task<List<ChatModel>> GetChatsForUser()
    {
        var chats = await chat_db.Chats
            .Include(c => c.UserIds)
            .Include(c => c.Messages)
            .Where(c => c.UserIds.Any(u => u.UserId == _userId))
            .ToListAsync();

        return await SetChatmodelsName(chats);
    }
    private async Task<Chat> CreatePersonalChat()
    {
        var chat = new Chat()
        {
            IsPersonal = true,
        };

        chat_db.Chats.Add(chat);
        await chat_db.SaveChangesAsync();

        return chat;
    }

    private async Task CreateUserIds(Guid chatId, Guid userId)
    {
        var userIds = new UserIds()
        {
            UserId = userId,
            ChatId = chatId,
            IsAdmin = false,
        };
        chat_db.UserIds.Add(userIds);
        await chat_db.SaveChangesAsync();
    }

    private async Task<Message> CreateMessage(NewMessageModel newMessageModel, Guid chatId)
    {
        var message = new Message()
        {
            Text = newMessageModel.Text,
            ChatId = chatId,
            FromUserId = _userId,
            ToUserId = newMessageModel.ToUserId,
        };

        chat_db.Messages.Add(message);
        await chat_db.SaveChangesAsync();

        return message;
    }


    private async Task<Chat> GetChatWithMessages(Guid chatId)
    {
        return await chat_db.Chats.Include(m => m.Messages).FirstOrDefaultAsync(c => c.Id == chatId);
    }
    private async Task<Chat> GetChatWithUserids(Guid chatId)
    {
        return await chat_db.Chats.Include(c => c.UserIds).FirstOrDefaultAsync(c => c.Id == chatId);
    }
    private async Task<List<ChatModel>> SetChatmodelsName(List<Chat> chats)
    {
        var chatModels = chats.Adapt<List<ChatModel>>();
        var chatUserIds = chats
            //.Where(c => c.IsPersonal)
            .Select(c => c.UserIds.Where(u => u.UserId != _userId)
            .Select(u => u.UserId).ToList())
            .ToList();

        var users = await _identityContext.Users.ToListAsync();

        for (int i = 0; i < chatModels.Count; i++)
        {
            if (chatModels[i].IsPersonal)
            {
                var username = users.FirstOrDefault(u => u.Id == chatUserIds[i][0])!.Username;
                
                chatModels[i].Name = username;
            }
        }

        return chatModels;
        /*
        for (int i = 0; i < chatModels.Count; i++)
        {
            if (chatModels[i].IsPersonal)
            {
                var UserID = Guid.Parse(chats[i].UserIds.First(u => u.UserId != _userId).UserId.ToString());
                var result = await _identityContext.Users.FirstOrDefaultAsync(u => u.Id == UserID)!;
                var username = result!.Username;
                chatModels[i].Name = username;
            }
        }*/
    }
}
