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
                name: "PaymentsData",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentsData", x => x.Id);
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
                    CurrentJobsId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReferralId = table.Column<long>(type: "bigint", nullable: true),
                    BonusAccount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersData", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "SubscribesData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserDataUserId = table.Column<long>(type: "bigint", nullable: false),
                    EndSubscribe = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscribesData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscribesData_UsersData_UserDataUserId",
                        column: x => x.UserDataUserId,
                        principalTable: "UsersData",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubscribesData_UserDataUserId",
                table: "SubscribesData",
                column: "UserDataUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobsData");

            migrationBuilder.DropTable(
                name: "PaymentsData");

            migrationBuilder.DropTable(
                name: "SubscribesData");

            migrationBuilder.DropTable(
                name: "UsersData");
        }
    }
}
