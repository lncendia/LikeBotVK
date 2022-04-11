using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LikeBotVK.Infrastructure.ApplicationData.Migrations
{
    public partial class countLimitation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "JobsData",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                table: "JobsData");
        }
    }
}
