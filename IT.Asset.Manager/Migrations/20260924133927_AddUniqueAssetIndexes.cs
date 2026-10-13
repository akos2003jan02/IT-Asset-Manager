using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IT.Asset.Manager.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueAssetIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InventoryId",
                table: "Assets",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_InventoryId",
                table: "Assets",
                column: "InventoryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assets_SerialNumber",
                table: "Assets",
                column: "SerialNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Assets_InventoryId",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_SerialNumber",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "InventoryId",
                table: "Assets");
        }
    }
}