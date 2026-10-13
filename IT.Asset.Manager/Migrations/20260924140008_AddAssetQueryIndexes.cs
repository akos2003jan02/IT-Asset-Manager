using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IT.Asset.Manager.Migrations
{
    /// <inheritdoc />
    public partial class AddAssetQueryIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Assets_AssignedUser",
                table: "Assets",
                column: "AssignedUser");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_Manufacturer",
                table: "Assets",
                column: "Manufacturer");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_Model",
                table: "Assets",
                column: "Model");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_PurchaseDate",
                table: "Assets",
                column: "PurchaseDate");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_Status",
                table: "Assets",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Assets_AssignedUser",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_Manufacturer",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_Model",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_PurchaseDate",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_Status",
                table: "Assets");
        }
    }
}
