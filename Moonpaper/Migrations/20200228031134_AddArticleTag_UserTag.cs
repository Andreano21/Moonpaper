using Microsoft.EntityFrameworkCore.Migrations;

namespace Moonpaper.Migrations
{
    public partial class AddArticleTag_UserTag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Articles");

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TagValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArticleTag",
                columns: table => new
                {
                    ArticleId = table.Column<int>(nullable: false),
                    TagId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleTag", x => new { x.ArticleId, x.TagId });
                    table.ForeignKey(
                        name: "FK_ArticleTag_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticleTag_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTag",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    TagId = table.Column<int>(nullable: false),
                    Rating = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTag", x => new { x.UserId, x.TagId });
                    table.ForeignKey(
                        name: "FK_UserTag_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTag_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleTag_TagId",
                table: "ArticleTag",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTag_TagId",
                table: "UserTag",
                column: "TagId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleTag");

            migrationBuilder.DropTable(
                name: "UserTag");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
