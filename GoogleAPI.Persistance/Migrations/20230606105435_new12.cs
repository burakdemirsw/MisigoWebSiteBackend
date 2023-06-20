using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoogleAPI.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class new12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductCards_ProductPropertyId",
                table: "ProductCards");

            migrationBuilder.DropColumn(
                name: "ProductCardId",
                table: "ProductProperty");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCards_ProductPropertyId",
                table: "ProductCards",
                column: "ProductPropertyId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductCards_ProductPropertyId",
                table: "ProductCards");

            migrationBuilder.AddColumn<int>(
                name: "ProductCardId",
                table: "ProductProperty",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCards_ProductPropertyId",
                table: "ProductCards",
                column: "ProductPropertyId");
        }
    }
}
