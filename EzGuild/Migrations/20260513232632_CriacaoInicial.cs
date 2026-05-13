using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EzGuild.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Jogadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nickname = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StatusConta = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jogadores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Missoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Titulo = table.Column<string>(type: "TEXT", nullable: false),
                    RecompensaOuro = table.Column<decimal>(type: "TEXT", nullable: false),
                    nivelMinimo = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Missoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Personagens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Classe = table.Column<string>(type: "TEXT", nullable: false),
                    Nivel = table.Column<int>(type: "INTEGER", nullable: false),
                    SaldoOuro = table.Column<decimal>(type: "TEXT", nullable: false),
                    JogadorId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personagens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Personagens_Jogadores_JogadorId",
                        column: x => x.JogadorId,
                        principalTable: "Jogadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MissaoPersonagem",
                columns: table => new
                {
                    MissoesId = table.Column<int>(type: "INTEGER", nullable: false),
                    PersonagemsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MissaoPersonagem", x => new { x.MissoesId, x.PersonagemsId });
                    table.ForeignKey(
                        name: "FK_MissaoPersonagem_Missoes_MissoesId",
                        column: x => x.MissoesId,
                        principalTable: "Missoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MissaoPersonagem_Personagens_PersonagemsId",
                        column: x => x.PersonagemsId,
                        principalTable: "Personagens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MissaoPersonagem_PersonagemsId",
                table: "MissaoPersonagem",
                column: "PersonagemsId");

            migrationBuilder.CreateIndex(
                name: "IX_Personagens_JogadorId",
                table: "Personagens",
                column: "JogadorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MissaoPersonagem");

            migrationBuilder.DropTable(
                name: "Missoes");

            migrationBuilder.DropTable(
                name: "Personagens");

            migrationBuilder.DropTable(
                name: "Jogadores");
        }
    }
}
