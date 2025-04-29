using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeService.Data.Migrations
{
    public partial class AppointmentChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkshopId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Appointments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_WorkshopId",
                table: "AspNetUsers",
                column: "WorkshopId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Workshops_WorkshopId",
                table: "AspNetUsers",
                column: "WorkshopId",
                principalTable: "Workshops",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Workshops_WorkshopId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_WorkshopId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "WorkshopId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Appointments");
        }
    }
}
