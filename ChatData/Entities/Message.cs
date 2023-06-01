using IdentityApi.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
namespace ChatData.Entities;

public class Message
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public Chat? Chat { get; set; }
    public Guid ChatId { get; set; }
    public User? User { get; set; }
    public Guid FromUserId { get; set; }
    public DateTime Date { get; set; }
}
