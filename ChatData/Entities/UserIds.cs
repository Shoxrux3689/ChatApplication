namespace ChatData.Entities;

public class UserIds
{
	public Chat? Chat { get; set; }
	public Guid ChatId { get; set; }
	public Guid UserId { get; set; }
	public bool IsAdmin { get; set; }
}
