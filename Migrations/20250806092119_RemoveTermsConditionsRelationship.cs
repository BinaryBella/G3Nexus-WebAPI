using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace G3NexusBackend.Migrations
{
    public partial class RemoveTermsConditionsRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TermsConditions_Projects_ProjectId",
                table: "TermsConditions");

            migrationBuilder.DropIndex(
                name: "IX_TermsConditions_ProjectId",
                table: "TermsConditions");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "TermsConditions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "TermsConditions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TermsConditions_ProjectId",
                table: "TermsConditions",
                column: "ProjectId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TermsConditions_Projects_ProjectId",
                table: "TermsConditions",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
