namespace ChatData.Models
{
	public class NewMessageModel
	{
		public string Text { get; set; }
		public Guid? ChatId { get; set; }
		public Guid? ToUserId { get; set; }
		public Guid FromUserId { get; set; }
	}
}
