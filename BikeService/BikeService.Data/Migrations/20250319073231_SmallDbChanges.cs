using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeService.Data.Migrations
{
    public partial class SmallDbChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PartId",
                table: "SpareParts",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "Services",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Orders",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "CartId",
                table: "Carts",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "CartItemId",
                table: "CartItems",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "BicycleId",
                table: "Bicycles",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "AppointmentId",
                table: "Appointments",
                newName: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "SpareParts",
                newName: "PartId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Services",
                newName: "ServiceId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Orders",
                newName: "OrderId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Carts",
                newName: "CartId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "CartItems",
                newName: "CartItemId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Bicycles",
                newName: "BicycleId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Appointments",
                newName: "AppointmentId");
        }
    }
}
