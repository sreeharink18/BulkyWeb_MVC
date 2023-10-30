using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkyWeb.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addWalletInapplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Wallet",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Wallet",
                table: "AspNetUsers");
        }
    }
}
