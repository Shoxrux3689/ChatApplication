namespace ChatData.Entities;

public class Message
{
	public Guid Id { get; set; }
	public required string Text { get; set; }
	public Chat? Chat { get; set; }
	public Guid ChatId { get; set; }
	public Guid FromUserId { get; set; }
	public Guid? ToUserId { get; set; }
	public DateTime Date { get; set; } = DateTime.UtcNow;
}
