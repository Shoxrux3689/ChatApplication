namespace ChatData.Entities;

public class Chat
{
	public Guid Id { get; set; }
	public string? Name { get; set; }
	public List<UserIds> UserIds { get; set; }
	public List<Message>? Messages { get; set; }
	public bool IsPersonal { get; set; }
}
