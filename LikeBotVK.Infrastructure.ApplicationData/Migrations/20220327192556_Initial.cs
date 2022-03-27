using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LikeBotVK.Infrastructure.ApplicationData.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobsData",
                columns: table => new
                {
                    JobId = table.Column<int>(type: "int", nullable: false),
                    BackgroundJobId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hashtag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateTimeLimitation = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WorkType = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobsData", x => x.JobId);
                });

            migrationBuilder.CreateTable(
                name: "UsersData",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    IsBanned = table.Column<bool>(type: "bit", nullable: false),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false),
                    CurrentVkId = table.Column<int>(type: "int", nullable: true),
                    ReferralId = table.Column<long>(type: "bigint", nullable: false),
                    BonusAccount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersData", x => x.UserId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobsData");

            migrationBuilder.DropTable(
                name: "UsersData");
        }
    }
}
