using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkyWeb.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddMultipleAddressInOrderheadear : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MultipleAddressId",
                table: "orderHeader",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_orderHeader_MultipleAddressId",
                table: "orderHeader",
                column: "MultipleAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_orderHeader_MultipleAddresses_MultipleAddressId",
                table: "orderHeader",
                column: "MultipleAddressId",
                principalTable: "MultipleAddresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orderHeader_MultipleAddresses_MultipleAddressId",
                table: "orderHeader");

            migrationBuilder.DropIndex(
                name: "IX_orderHeader_MultipleAddressId",
                table: "orderHeader");

            migrationBuilder.DropColumn(
                name: "MultipleAddressId",
                table: "orderHeader");
        }
    }
}
