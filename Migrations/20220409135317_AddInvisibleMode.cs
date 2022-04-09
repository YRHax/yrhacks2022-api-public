using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace yrhacks2022_api.Migrations
{
    public partial class AddInvisibleMode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Result_Invisible",
                table: "FileCache",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Result_Invisible",
                table: "FileCache");
        }
    }
}
