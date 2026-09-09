using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityMail.Web.Migrations
{
    /// <inheritdoc />
    public partial class replymessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
       name: "ReplyToMessageId",
       table: "UserMessages",
       type: "int",
       nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserMessages_ReplyToMessageId",
                table: "UserMessages",
                column: "ReplyToMessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMessages_UserMessages_ReplyToMessageId",
                table: "UserMessages",
                column: "ReplyToMessageId",
                principalTable: "UserMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
        name: "FK_UserMessages_UserMessages_ReplyToMessageId",
        table: "UserMessages");

            migrationBuilder.DropIndex(
                name: "IX_UserMessages_ReplyToMessageId",
                table: "UserMessages");

            migrationBuilder.DropColumn(
                name: "ReplyToMessageId",
                table: "UserMessages");
        }
    }
}
