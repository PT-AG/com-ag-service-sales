using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Ambassador.Service.Sales.Lib.Migrations
{
    public partial class AddColumnBookingOrderonCostCalculationTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CCQuantity",
                table: "GarmentBookingOrderItems",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "BOQuantity",
                table: "CostCalculationGarments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BookingOrderId",
                table: "CostCalculationGarments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BookingOrderItemId",
                table: "CostCalculationGarments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BookingOrderNo",
                table: "CostCalculationGarments",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CCQuantity",
                table: "GarmentBookingOrderItems");

            migrationBuilder.DropColumn(
                name: "BOQuantity",
                table: "CostCalculationGarments");

            migrationBuilder.DropColumn(
                name: "BookingOrderId",
                table: "CostCalculationGarments");

            migrationBuilder.DropColumn(
                name: "BookingOrderItemId",
                table: "CostCalculationGarments");

            migrationBuilder.DropColumn(
                name: "BookingOrderNo",
                table: "CostCalculationGarments");
        }
    }
}
