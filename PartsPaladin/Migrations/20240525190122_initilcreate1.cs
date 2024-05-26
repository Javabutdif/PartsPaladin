using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PartsPaladin.Migrations
{
    /// <inheritdoc />
    public partial class initilcreate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "product_quantity",
                table: "Product",
                newName: "product_stocks");

            migrationBuilder.AddColumn<string>(
                name: "product_image",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "product_image",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "product_stocks",
                table: "Product",
                newName: "product_quantity");
        }
    }
}
