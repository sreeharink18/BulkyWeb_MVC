using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkyWeb.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addBookCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalBooktCount",
                table: "products",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "Id",
                keyValue: 1,
                column: "TotalBooktCount",
                value: null);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "Id",
                keyValue: 2,
                column: "TotalBooktCount",
                value: null);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "Id",
                keyValue: 3,
                column: "TotalBooktCount",
                value: null);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "Id",
                keyValue: 4,
                column: "TotalBooktCount",
                value: null);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "Id",
                keyValue: 5,
                column: "TotalBooktCount",
                value: null);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "Id",
                keyValue: 6,
                column: "TotalBooktCount",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalBooktCount",
                table: "products");
        }
    }
}
