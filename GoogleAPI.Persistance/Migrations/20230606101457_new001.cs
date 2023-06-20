using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoogleAPI.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class new001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductPrices_ProductPriceTypes_ProductPriceTypeId",
                table: "ProductPrices");

            migrationBuilder.DropTable(
                name: "ProductPriceTypes");

            migrationBuilder.DropIndex(
                name: "IX_ProductPrices_ProductPriceTypeId",
                table: "ProductPrices");

            migrationBuilder.DropColumn(
                name: "MemberTypePrice10",
                table: "ProductPrices");

            migrationBuilder.DropColumn(
                name: "MemberTypePrice6",
                table: "ProductPrices");

            migrationBuilder.DropColumn(
                name: "MemberTypePrice7",
                table: "ProductPrices");

            migrationBuilder.DropColumn(
                name: "MemberTypePrice8",
                table: "ProductPrices");

            migrationBuilder.DropColumn(
                name: "MemberTypePrice9",
                table: "ProductPrices");

            migrationBuilder.DropColumn(
                name: "ProductPriceTypeId",
                table: "ProductPrices");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountedPrice",
                table: "ProductPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PurchasePrice",
                table: "ProductPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SellingPrice",
                table: "ProductPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VATRate",
                table: "ProductPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountedPrice",
                table: "ProductPrices");

            migrationBuilder.DropColumn(
                name: "PurchasePrice",
                table: "ProductPrices");

            migrationBuilder.DropColumn(
                name: "SellingPrice",
                table: "ProductPrices");

            migrationBuilder.DropColumn(
                name: "VATRate",
                table: "ProductPrices");

            migrationBuilder.AddColumn<decimal>(
                name: "MemberTypePrice10",
                table: "ProductPrices",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MemberTypePrice6",
                table: "ProductPrices",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MemberTypePrice7",
                table: "ProductPrices",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MemberTypePrice8",
                table: "ProductPrices",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MemberTypePrice9",
                table: "ProductPrices",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductPriceTypeId",
                table: "ProductPrices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ProductPriceTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LangCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPriceTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductPrices_ProductPriceTypeId",
                table: "ProductPrices",
                column: "ProductPriceTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPrices_ProductPriceTypes_ProductPriceTypeId",
                table: "ProductPrices",
                column: "ProductPriceTypeId",
                principalTable: "ProductPriceTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
