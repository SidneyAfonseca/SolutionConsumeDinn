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
                    { 13, "1139", "Outros Adiantamentos" },
                    { 14, "1141", "Impostos a Compensar e Recuperar Curto Prazo" },
                    { 15, "1151", "Máquinas e Implementos AGCO - Novos" },
                    { 16, "1152", "Máquinas e Implementos AGCO - Usados" },
                    { 17, "1153", "Máquinas e Implementos Outras Marcas - Novos" },
                    { 18, "1154", "Máquinas e Implementos Outras Marcas - Usados" },
                    { 19, "1155", "Peças AGCO" },
                    { 20, "1156", "Peças Outras Marcas" },
                    { 21, "1157", "Estoque de Outros Negócios - AGCO" },
                    { 22, "1158", "Estoque de Outros Negócios" },
                    { 23, "1159", "(-) Estoques - Ajuste a Valor Presente" },
                    { 24, "1160", "(-) Provisão de Estoque Obsoleto" },
                    { 25, "1194", "(-) Devolução de Vendas" },
                    { 26, "1195", "Valores em Transito" },
                    { 27, "1196", "(-) Contas a Receber - Ajuste a Valor Presente" },
                    { 28, "1197", "(-) PCLD - Provisão de Créditos Duvidosos" },
                    { 29, "1198", "Outros Créditos" },
                    { 30, "1199", "Fundo AGCO" },
                    { 31, "1211", "Contas a Receber - Vendas - Longo Prazo - Novos" },
                    { 32, "1212", "Contas a Receber - Vendas - Longo Prazo - Usados" },
                    { 33, "1213", "Aplicações Financeiras - Longo Prazo" },
                    { 34, "1214", "Consórcio" },
                    { 35, "1215", "Créditos com Partes Relacionadas - LP" },
                    { 36, "1216", "(-) Investimentos Permanentes - Ajuste a Valor Presente" },
                    { 37, "1217", "(-) Contas a Receber LP - Ajuste a Valor Presente" },
                    { 38, "1221", "Impostos Diferidos" },
                    { 39, "1225", "Despesas Antecipadas - Longo Prazo" },
                    { 40, "1229", "Outros Créditos" },
                    { 41, "1231", "Terrenos" },
                    { 42, "1232", "Construções e Prédios" },
                    { 43, "1233", "Máquinas/equipamentos e ferramentas" },
                    { 44, "1234", "Móveis e Utensílios" },
                    { 45, "1235", "Veículos" },
                    { 46, "1236", "Frota para Locação" },
                    { 47, "1237", "Imobilizado outros" },
                    { 48, "1238", "Benfeitorias em prédio de Terceiros" },
                    { 49, "1239", "Consorcios em Andamentos para Imobilziado" },
                    { 50, "1240", "Computadores e Perifericos" },
                    { 51, "1296", "(-) Aplicações Financeiras LP - Ajuste a Valor Presente" },
                    { 52, "1297", "(-) Ativo Imobilizado - Ajuste a Valor Presente" },
                    { 53, "1298", "Intangível" },
                    { 54, "1299", "(-) Depreciação e Amortização" },
                    { 55, "2111", "Fornecedores Maquinas Novas" },
                    { 56, "2112", "Fornecedores Maquinas Usadas" },
                    { 57, "2113", "Fornecedores Peças" },
                    { 58, "2114", "Fornecedores Serviços" },
                    { 59, "2115", "Fornecedores Outros" },
                    { 60, "2116", "Contas a Pagar Partes Relacionadas" },
                    { 61, "2117", "Outras contas a pagar" },
                    { 62, "2118", "Contas a Pagar Empresas Coligadas" },
                    { 63, "2121", "Financiamentos AGCO FINANCE CP" },
                    { 64, "2122", "Empréstimos Bancários CP" },
                    { 65, "2123", "Outros Empréstimos CP" },
                    { 66, "2124", "Financiamentos Outros Bancos CP" },
                    { 67, "2125", "Empréstimos de Sócios CP" },
                    { 68, "2126", "Leasing a Pagar CP" },
                    { 69, "2127", "Duplicatas Descontadas" },
                    { 70, "2128", "Financiamentos FINAME" },
                    { 71, "2130", "(-) Ajuste a Valor Presente - Empréstimos" },
                    { 72, "2131", "Salários e Bônus a Pagar" },
                    { 73, "2132", "Obrigações Trabalhistas a Recolher" },
                    { 74, "2133", "Provisões Trabalhistas" },
                    { 75, "2141", "Obrigações Tributárias a Recolher" },
                    { 76, "2142", "Outras Obrigações Fiscais a Recolher" },
                    { 77, "2151", "Adiantamento de Clientes" },
                    { 78, "2152", "Outros Adiantamentos" },
                    { 79, "2161", "Dividendos a Pagar" },
                    { 80, "2162", "Consórcios a Pagar" },
                    { 81, "2163", "Faturamento para Entrega Futura" },
                    { 82, "2164", "Receitas e Custos Diferidos" },
                    { 83, "2165", "Impostos Diferidos" },
                    { 84, "2166", "Valores em Transito" },
                    { 85, "2197", "(-) Devoluções de Compras" },
                    { 86, "2198", "(-) Contas a Pagar - Reclassificação para Longo Prazo" },
                    { 87, "2199", "(-) Ajuste a Valor Presente - Contas a Pagar" },
                    { 88, "2211", "Financiamentos AGCO FINANCE LP" },
                    { 89, "2212", "Financiamentos Outros Bancos LP" },
                    { 90, "2213", "Empréstimos Bancários LP" },
                    { 91, "2214", "(-) Ajuste a Valor Presente - Empréstimos" },
                    { 92, "2221", "Contas a Pagar - Empresas Coligadas LP" },
                    { 93, "2222", "Contas a Pagar e Fornecedores LP" },
                    { 94, "2223", "Outros Investimentos a pagar" },
                    { 95, "2224", "Contas a Pagar de Partes Relacionadas LP" },
                    { 96, "2225", "Outros Empréstimos LP" },
                    { 97, "2226", "Parcelamento de Tributos" },
                    { 98, "2227", "Provisões para Contingências" },
                    { 99, "2231", "Capital Social Integralizado" },
                    { 100, "2232", "Adiantamentos para Futuro Aumento de Capital" },
                    { 101, "2233", "Reservas de Lucro" },
                    { 102, "2234", "Reservas de Capital" },
                    { 103, "2235", "(-) Ações/Quotas em Tesouraria" },
                    { 104, "2236", "(-) Dividendos Distribuídos" },
                    { 105, "2237", "(-) Juros Capital Próprio" },
                    { 106, "2238", "(-) Prejuízos Acumulado" },
                    { 107, "2239", "Resultado do Exercício" },
                    { 108, "2299", "(-) Ajuste a Valor Presente - Outras Contas a Pagar LP" },
                    { 109, "3111", "Comissões sobre Vendas - Consórcio" },
                    { 110, "3112", "(-) Dedução de Comissões sobre Vendas - Consórcio" },
                    { 111, "3113", "Comissões sobre Vendas de Máquinas" },
                    { 112, "3114", "(-) Dedução de Comissões sobre Vendas de Máquinas" },
                    { 113, "3115", "Receita com Outras Comissões" },
                    { 114, "3116", "(-) Dedução de Outras Comissões" },
                    { 115, "3117", "Máquinas AGCO - Novos" },
                    { 116, "3118", "(-) Dedução de Máquinas AGCO - Novos" },
                    { 117, "3119", "Implementos AGCO - Novos" },
                    { 118, "3120", "(-) Dedução de Implementos AGCO - Novos" },
                    { 119, "3121", "Implementos Outras Marcas - Novos" },
                    { 120, "3122", "(-) Dedução de Máquinas Outras Marcas - Novos" },
                    { 121, "3131", "Máquinas e Implementos AGCO - Usados" },
                    { 122, "3132", "(-) Dedução de Máquinas e Implementos AGCO - Usados" },
                    { 123, "3133", "Máquinas e implementos Outras Marcas - Usados" },
                    { 124, "3134", "(-) Dedução de Máquinas e Implementos Outras Marcas - Usados" },
                    { 125, "3211", "Receita com Comissões Sobre Vendas de Peças" },
                    { 126, "3212", "(-) Dedução de Comissões Sobre Vendas de Peças" },
                    { 127, "3213", "Receitas Peças AGCO" },
                    { 128, "3214", "Receitas Peças Oficina AGCO" },
                    { 129, "3215", "(-) Dedução de Peças AGCO" },
                    { 130, "3216", "(-) Dedução de Peças Oficina AGCO" },
                    { 131, "3221", "Peças Outras Marcas" },
                    { 132, "3222", "Peças Oficina Outras Marcas" },
                    { 133, "3223", "(-) Dedução de Peças Outras Marcas" },
                    { 134, "3224", "(-) Dedução de Peças Oficina Outras Marcas" },
                    { 135, "3225", "Lubrificantes" },
                    { 136, "3226", "(-) Dedução de Lubrificantes" },
                    { 137, "3311", "Receita com Serviços Prestados a Clientes" },
                    { 138, "3312", "(-) Dedução de Serviços Prestados a Clientes" },
                    { 139, "3313", "Receita com Serviços Internos" },
                    { 140, "3314", "(-) Dedução de Serviços Internos" },
                    { 141, "3315", "Receita com Serviços de Garantia" },
                    { 142, "3316", "(-) Dedução de Serviços de Garantia" },
                    { 143, "3317", "Receita com Serviços Recall" },
                    { 144, "3318", "(-) Dedução de Serviços Recall" },
                    { 145, "3319", "Receita com KM de Serviços de Assistência a Campo" },
                    { 146, "3320", "(-) Dedução de KM de Serviços de Assistência a Campo" },
                    { 147, "3321", "Receita com deslocamento de Assistência a Campo" },
                    { 148, "3322", "(-) Dedução de deslocamento de Assistência a Campo" },
                    { 149, "3323", "Serviços Realizados por Terceiros" },
                    { 150, "3324", "(-) Dedução de Serviços Realizados por Terceiros" },
                    { 151, "3411", "Receitas de Tecnologia - ACGO" },
                    { 152, "3412", "Receita Outros Negócios AGCO" },
                    { 153, "3413", "(-) Dedução de Receitas Tecnologia - AGCO" },
                    { 154, "3414", "(-) Dedução de Receitas com Outros Negócios - AGCO" },
                    { 155, "3511", "Receita Outros Negócios" },
                    { 156, "3512", "(-) Dedução de Receitas com Outros Negócios" },
                    { 157, "4117", "Custo Máquinas AGCO - Novos" },
                    { 158, "4119", "Custo Implementos AGCO - Novos" },
                    { 159, "4121", "Custo Máquinas Outras Marcas - Novos" },
                    { 160, "4131", "Custo Máquinas e Implementos AGCO - Usados" },
                    { 161, "4133", "Custo Máquinas e implementos Outras Marcas - Usados" },
                    { 162, "4213", "Custo Peças AGCO" },
                    { 163, "4214", "Custo Peças Oficina AGCO" },
                    { 164, "4221", "Peças Outras Marcas" },
                    { 165, "4222", "Peças Oficina Outras Marcas" },
                    { 166, "4311", "Materiais de Consumo - Oficina" },
                    { 167, "4312", "Salários e Encargos dos Técnicos" },
                    { 168, "4313", "Custos Serviços Realizados por Terceiros" },
                    { 169, "4314", "Custo com deslocamento técnicos" },
                    { 170, "4315", "Custos com Pós - Venda/Garantias Indeferidas e Retrabalhos" },
                    { 171, "4319", "Uso e Consumo Oficina" },
                    { 172, "4320", "Ferramental de pequeno valor" },
                    { 173, "4399", "Outros Custos Sobre Serviços" },
                    { 174, "4411", "Custo de Tecnologia ACGO" },
                    { 175, "4412", "Custo com Outros Negócios - AGCO" },
                    { 176, "4511", "Custo com Outros Negócios" },
                    { 177, "5111", "Brindes" },
                    { 178, "5112", "Comissões e Encargo Folha de Pagamento" },
                    { 179, "5113", "Bônus e Participação nos Resultados" },
                    { 180, "5114", "Despesas com Ajustes de Acordo Comercial" },
                    { 181, "5116", "Perdas com Devedores Duvidosos" },
                    { 182, "5117", "Taxa Flat de Financiamento" },
                    { 183, "5118", "Despesas de Consumo Interno Oficina" },
                    { 184, "5119", "Fretes" },
                    { 185, "5120", "Despesa com reforma de máquinas usadas" },
                    { 186, "5121", "Remuneração Dirigentes" },
                    { 187, "5122", "Salários - Gerência e Supervisão" },
                    { 188, "5123", "Salários - Funcionários" },
                    { 189, "5124", "Provisões - 13o. Sal. e Férias" },
                    { 190, "5125", "Encargos Sociais - todos" },
                    { 191, "5126", "Benefícios" },
                    { 192, "5127", "Água, Luz e Gás" },
                    { 193, "5128", "Aluguel" },
                    { 194, "5129", "Amortizações e Depreciações" },
                    { 195, "5130", "Despesa com Alimentação" },
                    { 196, "5131", "Despesas com Material de Segurança e Uniformes" },
                    { 197, "5132", "Despesas com Móveis e Utensílios" },
                    { 198, "5133", "Despesas de Comunicação" },
                    { 199, "5134", "Donativos e Contribuições" },
                    { 200, "5135", "Eventos e Confraternização" },
                    { 201, "5136", "Garantias Recusadas - AGCO" },
                    { 202, "5137", "Impostos e Taxas Diversos" },
                    { 203, "5138", "Leasing" },
                    { 204, "5139", "Legais e Auditoria" },
                    { 205, "5140", "Manutenção de Veículos frota da Adm" },
                    { 206, "5141", "Manutenção Predial" },
                    { 207, "5142", "Materiais Diversos / Escritório" },
                    { 208, "5143", "Multas" },
                    { 209, "5144", "Processamento Dados - Terceiros" },
                    { 210, "5145", "Propaganda e Promoção de Vendas" },
                    { 211, "5146", "Publicações, Assinaturas e Assoc." },
                    { 212, "5147", "Retorno de Garantia ( Pecas e Serviços )" },
                    { 213, "5148", "Seguros" },
                    { 214, "5149", "Serviços Profissionais - Terceiros" },
                    { 215, "5150", "Treinamento" },
                    { 216, "5151", "Combustíveis e Lubrificantes" },
                    { 217, "5152", "Viagens, Represent. e Hospitalidade" },
                    { 218, "5153", "Taxas Associativas" },
                    { 219, "5154", "Sistemas, Software e Informática" },
                    { 220, "5155", "Postagens, Correios e Telégrafos" },
                    { 221, "5156", "Aviso Prévio" },
                    { 222, "5157", "Ações Trabalhistas" },
                    { 223, "5158", "Estágio" },
                    { 224, "5199", "Outros Despesas" },
                    { 225, "6111", "Rendimento Aplicações Financeiras" },
                    { 226, "6112", "Juros Recebidos" },
                    { 227, "6113", "Variações Monetárias Ativas" },
                    { 228, "6114", "Descontos Obtidos" },
                    { 229, "6115", "Juros de CAR" },
                    { 230, "6121", "Despesas Financeiras s/ Capital Giro" },
                    { 231, "6122", "IOF" },
                    { 232, "6123", "Despesa com Juros" },
                    { 233, "6124", "Multas Financeiras" },
                    { 234, "6125", "Juros sobre Capital Próprio" },
                    { 235, "6126", "Variações Monetárias Passivas" },
                    { 236, "6127", "Despesas Bancárias" },
                    { 237, "6128", "Descontos Passivos" },
                    { 238, "7111", "Ganho com Venda de Ativo Imobilizado" },
                    { 239, "7112", "Outras Receitas" },
                    { 240, "7121", "Perda com Venda de Ativo" },
                    { 241, "7122", "Outras Despesas" },
                    { 242, "8111", "IRPJ" },
                    { 243, "8112", "CSLL" },
                    { 244, "9111", "Diretores" },
                    { 245, "9112", "Gerentes" },
                    { 246, "9113", "Supervisores" },
                    { 247, "9114", "Coordenadores" },
                    { 248, "9115", "Vendedores" },
                    { 249, "9116", "Analistas" },
                    { 250, "9117", "Assistentes" },
                    { 251, "9118", "Auxiliares" },
                    { 252, "9119", "Diretores" },
                    { 253, "9211", "Novos" },
                    { 254, "9212", "Usados" },
                    { 255, "9213", "Outros Negócios" },
                    { 256, "9311", "Trator" },
                    { 257, "9312", "Colheitadeira" },
                    { 258, "9313", "Colhedora" },
                    { 259, "9314", "Plantadeira" },
                    { 260, "9315", "Pulverizador" },
                    { 261, "9316", "Plataforma" },
                    { 262, "9317", "Implementos" },
                    { 263, "9318", "Outros" },
                    { 264, "9411", "Trator" },
                    { 265, "9412", "Colheitadeira" },
                    { 266, "9413", "Colhedora" },
                    { 267, "9414", "Plantadeira" },
                    { 268, "9415", "Pulverizador" },
                    { 269, "9416", "Plataforma" },
                    { 270, "9417", "Implementos" },
                    { 271, "9418", "Outros" },
                    { 272, "9419", "Colheitadeira" },
                    { 273, "9420", "Colhedora" },
                    { 274, "9421", "Plantadeira" },
                    { 275, "9422", "Pulverizador" },
                    { 276, "9423", "Plataforma" },
                    { 277, "9424", "Implementos" },
                    { 278, "9511", "Horas Disponíveis" },
                    { 279, "9512", "Horas Faturadas" },
                    { 280, "9513", "Tempo de O.S." },
                    { 281, "9514", "Valor O.S. em Aberto" },
                    { 282, "9611", "Tempo de Pedido" },
                    { 283, "9612", "Horas Faturadas" },
                    { 284, "9613", "Valor Pedidos Abertos" }
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
