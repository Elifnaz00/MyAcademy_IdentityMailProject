using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityMail.Web.Migrations
{
    /// <inheritdoc />
    public partial class addIsComplaint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMessages_UserMessages_ReplyToMessageId",
                table: "UserMessages");

            migrationBuilder.AddColumn<bool>(
                name: "IsComplaint",
                table: "UserMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMessages_UserMessages_ReplyToMessageId",
                table: "UserMessages",
                column: "ReplyToMessageId",
                principalTable: "UserMessages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMessages_UserMessages_ReplyToMessageId",
                table: "UserMessages");

            migrationBuilder.DropColumn(
                name: "IsComplaint",
                table: "UserMessages");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMessages_UserMessages_ReplyToMessageId",
                table: "UserMessages",
                column: "ReplyToMessageId",
                principalTable: "UserMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
