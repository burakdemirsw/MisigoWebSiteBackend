using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoogleAPI.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class mign5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BarcodeTypes_Barcodes_BarcodeId",
                table: "BarcodeTypes");

            migrationBuilder.DropIndex(
                name: "IX_BarcodeTypes_BarcodeId",
                table: "BarcodeTypes");

            migrationBuilder.DropColumn(
                name: "BarcodeId",
                table: "BarcodeTypes");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BarcodeBarcodeType");

            migrationBuilder.AddColumn<int>(
                name: "BarcodeId",
                table: "BarcodeTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
                onDelete: ReferentialAction.Cascade);
        }
    }
}
