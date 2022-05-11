using Microsoft.EntityFrameworkCore.Migrations;

namespace FA21.P05.Web.Migrations
{
    public partial class suh : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Scheduler",
                newName: "Day");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Day",
                table: "Scheduler",
                newName: "Id");
        }
    }
}
