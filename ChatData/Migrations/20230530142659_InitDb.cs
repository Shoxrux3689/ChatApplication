using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatData.Migrations
{
	/// <inheritdoc />
	public partial class InitDb : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Chats",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Chats", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "User",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
					PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
					ChatId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_User", x => x.Id);
					table.ForeignKey(
						name: "FK_User_Chats_ChatId",
						column: x => x.ChatId,
						principalTable: "Chats",
						principalColumn: "Id");
				});

			migrationBuilder.CreateTable(
				name: "Messages",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
					ChatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					FromUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Messages", x => x.Id);
					table.ForeignKey(
						name: "FK_Messages_Chats_ChatId",
						column: x => x.ChatId,
						principalTable: "Chats",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_Messages_User_UserId",
						column: x => x.UserId,
						principalTable: "User",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_Messages_ChatId",
				table: "Messages",
				column: "ChatId");

			migrationBuilder.CreateIndex(
				name: "IX_Messages_UserId",
				table: "Messages",
				column: "UserId");

			migrationBuilder.CreateIndex(
				name: "IX_User_ChatId",
				table: "User",
				column: "ChatId");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "Messages");

			migrationBuilder.DropTable(
				name: "User");

			migrationBuilder.DropTable(
				name: "Chats");
		}
	}
}
