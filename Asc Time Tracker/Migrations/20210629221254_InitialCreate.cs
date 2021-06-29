using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Asc_Time_Tracker.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TimeLog",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EMPID = table.Column<int>(type: "int", nullable: false),
                    JOBNUM = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TIME = table.Column<double>(type: "float", nullable: false),
                    NOTES = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RD = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeLog", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TimeLog");
        }
    }
}
