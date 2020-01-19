using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Moonparser.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    Summary = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    Source = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    UrlSource = table.Column<string>(nullable: true),
                    UrlMainImg = table.Column<string>(nullable: true),
                    Views = table.Column<int>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Articles");
        }
    }
}
