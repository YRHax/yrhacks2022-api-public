using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace yrhacks2022_api.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CacheData",
                columns: table => new
                {
                    Url = table.Column<string>(type: "TEXT", nullable: false),
                    LastCached = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Result_Title = table.Column<string>(type: "TEXT", nullable: false),
                    Result_Color = table.Column<string>(type: "TEXT", nullable: false),
                    Result_Description = table.Column<string>(type: "TEXT", nullable: false),
                    Result_Delete = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CacheData", x => x.Url);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CacheData");
        }
    }
}
