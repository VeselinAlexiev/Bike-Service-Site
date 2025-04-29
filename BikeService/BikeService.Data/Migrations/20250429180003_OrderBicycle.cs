using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeService.Data.Migrations
{
    public partial class OrderBicycle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderBicycle_Bicycles_BicycleId",
                table: "OrderBicycle");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderBicycle_Orders_OrderId",
                table: "OrderBicycle");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderBicycle",
                table: "OrderBicycle");

            migrationBuilder.RenameTable(
                name: "OrderBicycle",
                newName: "OrderBicycles");

            migrationBuilder.RenameIndex(
                name: "IX_OrderBicycle_BicycleId",
                table: "OrderBicycles",
                newName: "IX_OrderBicycles_BicycleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderBicycles",
                table: "OrderBicycles",
                columns: new[] { "OrderId", "BicycleId" });

            migrationBuilder.AddForeignKey(
                name: "FK_OrderBicycles_Bicycles_BicycleId",
                table: "OrderBicycles",
                column: "BicycleId",
                principalTable: "Bicycles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderBicycles_Orders_OrderId",
                table: "OrderBicycles",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderBicycles_Bicycles_BicycleId",
                table: "OrderBicycles");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderBicycles_Orders_OrderId",
                table: "OrderBicycles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderBicycles",
                table: "OrderBicycles");

            migrationBuilder.RenameTable(
                name: "OrderBicycles",
                newName: "OrderBicycle");

            migrationBuilder.RenameIndex(
                name: "IX_OrderBicycles_BicycleId",
                table: "OrderBicycle",
                newName: "IX_OrderBicycle_BicycleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderBicycle",
                table: "OrderBicycle",
                columns: new[] { "OrderId", "BicycleId" });

            migrationBuilder.AddForeignKey(
                name: "FK_OrderBicycle_Bicycles_BicycleId",
                table: "OrderBicycle",
                column: "BicycleId",
                principalTable: "Bicycles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderBicycle_Orders_OrderId",
                table: "OrderBicycle",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
