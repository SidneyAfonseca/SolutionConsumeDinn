using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ConsumeDinnApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateWithSeeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AccountPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountPlans", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DInReturns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Payload = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Response = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Success = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DInReturns", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AccountPlans",
                columns: new[] { "Id", "Code", "Description" },
                values: new object[,]
                {
                    { 1, "1111", "Disponibilidades Imediatas" },
                    { 2, "1121", "Contas a Receber Maquinas Novas" },
                    { 3, "1122", "Contas a Receber Maquinas Usadas" },
                    { 4, "1123", "Contas a Receber Peças" },
                    { 5, "1124", "Contas a Receber Serviços" },
                    { 6, "1125", "Contas a Receber Comissões e Consórcios" },
                    { 7, "1126", "Contas a Receber Outros" },
                    { 8, "1127", "Empresas Coligadas" },
                    { 9, "1128", "Consorcio em Andamento" },
                    { 10, "1131", "Adiantamentos Fornecedores" },
                    { 11, "1132", "Adiantamentos Funcionários" },
                    { 12, "1133", "Despesas Antecipadas - Curto Prazo" },
                    { 13, "1151", "Máquinas e Implementos AGCO - Novos" },
                    { 14, "1152", "Máquinas e Implementos AGCO - Usados" },
                    { 15, "1155", "Peças AGCO" },
                    { 16, "2111", "Fornecedores Maquinas Novas" },
                    { 17, "2112", "Fornecedores Maquinas Usadas" },
                    { 18, "2113", "Fornecedores Peças" },
                    { 19, "2114", "Fornecedores Serviços" },
                    { 20, "2131", "Salários e Bônus a Pagar" }
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "Code", "Description" },
                values: new object[,]
                {
                    { 1, "1101", "Maquinas Novas" },
                    { 2, "1102", "Maquinas Usadas" },
                    { 3, "1201", "Peças" },
                    { 4, "1301", "Serviços" },
                    { 5, "1401", "Administrativo" },
                    { 6, "1501", "Outros Negócios" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountPlans");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "DInReturns");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
