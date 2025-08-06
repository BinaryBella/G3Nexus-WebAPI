using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace G3NexusBackend.Migrations
{
    public partial class addmodalprojecttermscondition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ServerCost",
                table: "QuotationCosts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "ProjectTermsConditions",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    TCId = table.Column<int>(type: "int", nullable: false),
                    IsChecked = table.Column<bool>(type: "bit", nullable: false),
                    TermsConditionsTCId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTermsConditions", x => new { x.ProjectId, x.TCId });
                    table.ForeignKey(
                        name: "FK_ProjectTermsConditions_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectTermsConditions_TermsConditions_TCId",
                        column: x => x.TCId,
                        principalTable: "TermsConditions",
                        principalColumn: "TCId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectTermsConditions_TermsConditions_TermsConditionsTCId",
                        column: x => x.TermsConditionsTCId,
                        principalTable: "TermsConditions",
                        principalColumn: "TCId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTermsConditions_TCId",
                table: "ProjectTermsConditions",
                column: "TCId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTermsConditions_TermsConditionsTCId",
                table: "ProjectTermsConditions",
                column: "TermsConditionsTCId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectTermsConditions");

            migrationBuilder.DropColumn(
                name: "ServerCost",
                table: "QuotationCosts");
        }
    }
}
