namespace IdentityBlazor.Models;

public class ChatModel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public bool IsPersonal { get; set; }
    public Guid CurrentUserId { get; set; }
    public List<MessageModel> Messages { get; set; } = new List<MessageModel>();

    public ChatModel()
    {

    }
}
