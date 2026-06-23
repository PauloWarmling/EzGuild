using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EzGuild.Migrations
{
    /// <inheritdoc />
    public partial class SistemaDeTempoFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Descricao",
                table: "Missoes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TempoSegundosBase",
                table: "Missoes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descricao",
                table: "Missoes");

            migrationBuilder.DropColumn(
                name: "TempoSegundosBase",
                table: "Missoes");
        }
    }
}
