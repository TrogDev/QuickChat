using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickChat.Message.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChatIdIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatId_Id",
                table: "Messages",
                columns: ["ChatId", "Id"]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_ChatId_Id",
                table: "Messages");
        }
    }
}
