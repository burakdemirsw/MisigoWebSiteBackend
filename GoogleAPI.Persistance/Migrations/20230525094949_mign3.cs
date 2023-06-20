using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoogleAPI.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class mign3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Barcodes_BarcodeTypes_BarcodeTypeId",
                table: "Barcodes");

            migrationBuilder.DropIndex(
                name: "IX_ProductPrices_ProductPriceTypeId",
                table: "ProductPrices");

            migrationBuilder.DropIndex(
                name: "IX_Photos_PhotoTypeId",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_Barcodes_BarcodeTypeId",
                table: "Barcodes");

            migrationBuilder.AddColumn<int>(
                name: "BarcodeId",
                table: "BarcodeTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductPrices_ProductPriceTypeId",
                table: "ProductPrices",
                column: "ProductPriceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_PhotoTypeId",
                table: "Photos",
                column: "PhotoTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BarcodeTypes_BarcodeId",
                table: "BarcodeTypes",
                column: "BarcodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_BarcodeTypes_Barcodes_BarcodeId",
                table: "BarcodeTypes",
                column: "BarcodeId",
                principalTable: "Barcodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BarcodeTypes_Barcodes_BarcodeId",
                table: "BarcodeTypes");

            migrationBuilder.DropIndex(
                name: "IX_ProductPrices_ProductPriceTypeId",
                table: "ProductPrices");

            migrationBuilder.DropIndex(
                name: "IX_Photos_PhotoTypeId",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_BarcodeTypes_BarcodeId",
                table: "BarcodeTypes");

            migrationBuilder.DropColumn(
                name: "BarcodeId",
                table: "BarcodeTypes");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPrices_ProductPriceTypeId",
                table: "ProductPrices",
                column: "ProductPriceTypeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Photos_PhotoTypeId",
                table: "Photos",
                column: "PhotoTypeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Barcodes_BarcodeTypeId",
                table: "Barcodes",
                column: "BarcodeTypeId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Barcodes_BarcodeTypes_BarcodeTypeId",
                table: "Barcodes",
                column: "BarcodeTypeId",
                principalTable: "BarcodeTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
