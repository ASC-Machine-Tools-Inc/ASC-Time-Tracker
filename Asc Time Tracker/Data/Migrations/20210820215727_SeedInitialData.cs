using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Asc_Time_Tracker.Data.Migrations
{
    public partial class SeedInitialData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TimeLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JobNum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Time = table.Column<double>(type: "float", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rd = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeLog", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TimeLog",
                columns: new[] { "Id", "Date", "EmpId", "JobNum", "Notes", "Rd", "Time" },
                values: new object[] { 1, new DateTime(2021, 8, 20, 0, 0, 0, 0, DateTimeKind.Local), "timeuser@ascmt.com", "23456", "First log!", false, 6000.0 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TimeLog");
        }
    }
}
