using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace yrhacks2022_api.Migrations
{
    public partial class RemoveThing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CacheData",
                table: "CacheData");

            migrationBuilder.RenameTable(
                name: "CacheData",
                newName: "FileCache");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileCache",
                table: "FileCache",
                column: "Url");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FileCache",
                table: "FileCache");

            migrationBuilder.RenameTable(
                name: "FileCache",
                newName: "CacheData");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CacheData",
                table: "CacheData",
                column: "Url");
        }
    }
}
