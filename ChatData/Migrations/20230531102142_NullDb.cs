using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatData.Migrations
{
	/// <inheritdoc />
	public partial class NullDb : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_Messages_User_UserId",
				table: "Messages");

			migrationBuilder.AlterColumn<Guid>(
				name: "UserId",
				table: "Messages",
				type: "uniqueidentifier",
				nullable: true,
				oldClrType: typeof(Guid),
				oldType: "uniqueidentifier");

			migrationBuilder.AddForeignKey(
				name: "FK_Messages_User_UserId",
				table: "Messages",
				column: "UserId",
				principalTable: "User",
				principalColumn: "Id");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_Messages_User_UserId",
				table: "Messages");

			migrationBuilder.AlterColumn<Guid>(
				name: "UserId",
				table: "Messages",
				type: "uniqueidentifier",
				nullable: false,
				defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
				oldClrType: typeof(Guid),
				oldType: "uniqueidentifier",
				oldNullable: true);

			migrationBuilder.AddForeignKey(
				name: "FK_Messages_User_UserId",
				table: "Messages",
				column: "UserId",
				principalTable: "User",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}
	}
}
