using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoogleAPI.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class new16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductStocks_ColorID",
                table: "ProductStocks");

            migrationBuilder.DropIndex(
                name: "IX_ProductStocks_DimentionID",
                table: "ProductStocks");

            migrationBuilder.CreateIndex(
                name: "IX_ProductStocks_ColorID",
                table: "ProductStocks",
                column: "ColorID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductStocks_DimentionID",
                table: "ProductStocks",
                column: "DimentionID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductStocks_ColorID",
                table: "ProductStocks");

            migrationBuilder.DropIndex(
                name: "IX_ProductStocks_DimentionID",
                table: "ProductStocks");

            migrationBuilder.CreateIndex(
                name: "IX_ProductStocks_ColorID",
                table: "ProductStocks",
                column: "ColorID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductStocks_DimentionID",
                table: "ProductStocks",
                column: "DimentionID",
                unique: true);
        }
    }
}
