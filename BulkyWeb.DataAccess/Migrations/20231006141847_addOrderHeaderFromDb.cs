using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkyWeb.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addOrderHeaderFromDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orderDetails_OrderHeader_OrderId",
                table: "orderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderHeader_AspNetUsers_ApplicationUserId",
                table: "OrderHeader");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderHeader",
                table: "OrderHeader");

            migrationBuilder.RenameTable(
                name: "OrderHeader",
                newName: "orderHeader");

            migrationBuilder.RenameIndex(
                name: "IX_OrderHeader_ApplicationUserId",
                table: "orderHeader",
                newName: "IX_orderHeader_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_orderHeader",
                table: "orderHeader",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_orderDetails_orderHeader_OrderId",
                table: "orderDetails",
                column: "OrderId",
                principalTable: "orderHeader",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_orderHeader_AspNetUsers_ApplicationUserId",
                table: "orderHeader",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orderDetails_orderHeader_OrderId",
                table: "orderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_orderHeader_AspNetUsers_ApplicationUserId",
                table: "orderHeader");

            migrationBuilder.DropPrimaryKey(
                name: "PK_orderHeader",
                table: "orderHeader");

            migrationBuilder.RenameTable(
                name: "orderHeader",
                newName: "OrderHeader");

            migrationBuilder.RenameIndex(
                name: "IX_orderHeader_ApplicationUserId",
                table: "OrderHeader",
                newName: "IX_OrderHeader_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderHeader",
                table: "OrderHeader",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_orderDetails_OrderHeader_OrderId",
                table: "orderDetails",
                column: "OrderId",
                principalTable: "OrderHeader",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHeader_AspNetUsers_ApplicationUserId",
                table: "OrderHeader",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
