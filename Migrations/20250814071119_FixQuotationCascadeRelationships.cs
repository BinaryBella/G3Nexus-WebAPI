using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace G3NexusBackend.Migrations
{
    public partial class FixQuotationCascadeRelationships : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Quotations");

            migrationBuilder.RenameColumn(
                name: "CreationDate",
                table: "Quotations",
                newName: "CreatedDate");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "Quotations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Quotations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_ClientId",
                table: "Quotations",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_EmployeeId",
                table: "Quotations",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_ProjectId",
                table: "Quotations",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quotations_Clients_ClientId",
                table: "Quotations",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotations_Employees_EmployeeId",
                table: "Quotations",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotations_Projects_ProjectId",
                table: "Quotations",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quotations_Clients_ClientId",
                table: "Quotations");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotations_Employees_EmployeeId",
                table: "Quotations");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotations_Projects_ProjectId",
                table: "Quotations");

            migrationBuilder.DropIndex(
                name: "IX_Quotations_ClientId",
                table: "Quotations");

            migrationBuilder.DropIndex(
                name: "IX_Quotations_EmployeeId",
                table: "Quotations");

            migrationBuilder.DropIndex(
                name: "IX_Quotations_ProjectId",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Quotations");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Quotations",
                newName: "CreationDate");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Quotations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
