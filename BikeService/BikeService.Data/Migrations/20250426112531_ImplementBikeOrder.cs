using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeService.Data.Migrations
{
    public partial class ImplementBikeOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PartId",
                table: "CartItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "BicycleId",
                table: "CartItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrderBicycle",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    BicycleId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderBicycle", x => new { x.OrderId, x.BicycleId });
                    table.ForeignKey(
                        name: "FK_OrderBicycle_Bicycles_BicycleId",
                        column: x => x.BicycleId,
                        principalTable: "Bicycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderBicycle_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_BicycleId",
                table: "CartItems",
                column: "BicycleId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderBicycle_BicycleId",
                table: "OrderBicycle",
                column: "BicycleId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Bicycles_BicycleId",
                table: "CartItems",
                column: "BicycleId",
                principalTable: "Bicycles",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Bicycles_BicycleId",
                table: "CartItems");

            migrationBuilder.DropTable(
                name: "OrderBicycle");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_BicycleId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "BicycleId",
                table: "CartItems");

            migrationBuilder.AlterColumn<int>(
                name: "PartId",
                table: "CartItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
