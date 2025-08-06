using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace G3NexusBackend.Migrations
{
    public partial class AddedIsNeToRequirement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsNew",
                table: "Requirements",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNew",
                table: "Requirements");
        }
    }
}
