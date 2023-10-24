using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkyWeb.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addnullmultiAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orderHeader_MultipleAddresses_MultipleAddressId",
                table: "orderHeader");

            migrationBuilder.AlterColumn<int>(
                name: "MultipleAddressId",
                table: "orderHeader",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_orderHeader_MultipleAddresses_MultipleAddressId",
                table: "orderHeader",
                column: "MultipleAddressId",
                principalTable: "MultipleAddresses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orderHeader_MultipleAddresses_MultipleAddressId",
                table: "orderHeader");

            migrationBuilder.AlterColumn<int>(
                name: "MultipleAddressId",
                table: "orderHeader",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_orderHeader_MultipleAddresses_MultipleAddressId",
                table: "orderHeader",
                column: "MultipleAddressId",
                principalTable: "MultipleAddresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
