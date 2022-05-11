using Microsoft.EntityFrameworkCore.Migrations;

namespace FA21.P05.Web.Migrations
{
    public partial class test49 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Day",
                table: "Scheduler",
                newName: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Scheduler",
                newName: "Day");
        }
    }
}
