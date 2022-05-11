using Microsoft.EntityFrameworkCore.Migrations;

namespace FA21.P05.Web.Migrations
{
    public partial class localbs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Scheduler",
                newName: "Day");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Day",
                table: "Scheduler",
                newName: "ID");
        }
    }
}
