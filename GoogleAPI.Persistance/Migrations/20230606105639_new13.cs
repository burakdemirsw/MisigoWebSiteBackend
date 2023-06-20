using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoogleAPI.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class new13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductCards_ProductSeoDetailId",
                table: "ProductCards");

            migrationBuilder.DropColumn(
                name: "ProductCardId",
                table: "ProductSeoDetails");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCards_ProductSeoDetailId",
                table: "ProductCards",
                column: "ProductSeoDetailId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductCards_ProductSeoDetailId",
                table: "ProductCards");

            migrationBuilder.AddColumn<int>(
                name: "ProductCardId",
                table: "ProductSeoDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCards_ProductSeoDetailId",
                table: "ProductCards",
                column: "ProductSeoDetailId");
        }
    }
}
