using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace G3NexusBackend.Migrations
{
    public partial class modifymodelbug : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Attachment",
                table: "Bugs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<bool>(
                name: "IsQuoted",
                table: "Bugs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "QuotationId",
                table: "Bugs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bugs_QuotationId",
                table: "Bugs",
                column: "QuotationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bugs_Quotations_QuotationId",
                table: "Bugs",
                column: "QuotationId",
                principalTable: "Quotations",
                principalColumn: "QuotationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bugs_Quotations_QuotationId",
                table: "Bugs");

            migrationBuilder.DropIndex(
                name: "IX_Bugs_QuotationId",
                table: "Bugs");

            migrationBuilder.DropColumn(
                name: "IsQuoted",
                table: "Bugs");

            migrationBuilder.DropColumn(
                name: "QuotationId",
                table: "Bugs");

            migrationBuilder.AlterColumn<string>(
                name: "Attachment",
                table: "Bugs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);
        }
    }
}
