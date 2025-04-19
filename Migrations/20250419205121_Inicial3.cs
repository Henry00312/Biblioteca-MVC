using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblioteca_MVC.Migrations
{
    /// <inheritdoc />
    public partial class Inicial3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "henry@HENRYfecharegistro",
                table: "Material",
                newName: "fecharegistro");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "fecharegistro",
                table: "Material",
                newName: "henry@HENRYfecharegistro");
        }
    }
}
