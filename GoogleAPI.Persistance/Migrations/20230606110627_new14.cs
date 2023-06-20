using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoogleAPI.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class new14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BarcodeBarcodeType");

            migrationBuilder.AddColumn<int>(
                name: "BarcodeTypeId",
                table: "Barcodes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Barcodes_BarcodeTypeId",
                table: "Barcodes",
                column: "BarcodeTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Barcodes_BarcodeTypes_BarcodeTypeId",
                table: "Barcodes",
                column: "BarcodeTypeId",
                principalTable: "BarcodeTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Barcodes_BarcodeTypes_BarcodeTypeId",
                table: "Barcodes");

            migrationBuilder.DropIndex(
                name: "IX_Barcodes_BarcodeTypeId",
                table: "Barcodes");

            migrationBuilder.DropColumn(
                name: "BarcodeTypeId",
                table: "Barcodes");

            migrationBuilder.CreateTable(
                name: "BarcodeBarcodeType",
                columns: table => new
                {
                    BarcodeTypesId = table.Column<int>(type: "int", nullable: false),
                    BarcodesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BarcodeBarcodeType", x => new { x.BarcodeTypesId, x.BarcodesId });
                    table.ForeignKey(
                        name: "FK_BarcodeBarcodeType_BarcodeTypes_BarcodeTypesId",
                        column: x => x.BarcodeTypesId,
                        principalTable: "BarcodeTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BarcodeBarcodeType_Barcodes_BarcodesId",
                        column: x => x.BarcodesId,
                        principalTable: "Barcodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BarcodeBarcodeType_BarcodesId",
                table: "BarcodeBarcodeType",
                column: "BarcodesId");
        }
    }
}
