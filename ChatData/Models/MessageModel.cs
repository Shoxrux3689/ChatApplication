namespace ChatData.Models;

public class MessageModel
{
	public Guid Id { get; set; }
	public string Text { get; set; }
	public Guid ChatId { get; set; }
	public Guid FromUserId { get; set; }
	public DateTime Date { get; set; }
}
