using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PartsPaladin.Migrations
{
    /// <inheritdoc />
    public partial class addSubtotal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "subtotal",
                table: "CartItems",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "subtotal",
                table: "CartItems");
        }
    }
}
