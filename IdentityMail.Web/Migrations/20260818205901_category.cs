using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityMail.Web.Migrations
{
    /// <inheritdoc />
    public partial class category : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryUserMessage");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "UserMessages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserMessages_CategoryId",
                table: "UserMessages",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMessages_Categories_CategoryId",
                table: "UserMessages",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMessages_Categories_CategoryId",
                table: "UserMessages");

            migrationBuilder.DropIndex(
                name: "IX_UserMessages_CategoryId",
                table: "UserMessages");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "UserMessages");

            migrationBuilder.CreateTable(
                name: "CategoryUserMessage",
                columns: table => new
                {
                    CategoriesId = table.Column<int>(type: "int", nullable: false),
                    UserMessagesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryUserMessage", x => new { x.CategoriesId, x.UserMessagesId });
                    table.ForeignKey(
                        name: "FK_CategoryUserMessage_Categories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryUserMessage_UserMessages_UserMessagesId",
                        column: x => x.UserMessagesId,
                        principalTable: "UserMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryUserMessage_UserMessagesId",
                table: "CategoryUserMessage",
                column: "UserMessagesId");
        }
    }
}
