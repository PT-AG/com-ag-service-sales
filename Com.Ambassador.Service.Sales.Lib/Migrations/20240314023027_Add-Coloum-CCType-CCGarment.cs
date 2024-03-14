using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Ambassador.Service.Sales.Lib.Migrations
{
    public partial class AddColoumCCTypeCCGarment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Article",
                table: "CostCalculationGarments",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CCType",
                table: "CostCalculationGarments",
                maxLength: 20,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CCType",
                table: "CostCalculationGarments");

            migrationBuilder.AlterColumn<string>(
                name: "Article",
                table: "CostCalculationGarments",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 1000,
                oldNullable: true);
        }
    }
}
