using IdentityApi.Data.Entities;

namespace IdentityApi.Data.Models
{
	public class UserModel
	{
		public Guid Id { get; set; }
		public string Username { get; set; }
		public string? PhotoPath { get; set; }

		public UserModel(User user)
		{
			Id = user.Id;
			Username = user.Username;
			PhotoPath = user.PhotoPath;
		}
	}
}
