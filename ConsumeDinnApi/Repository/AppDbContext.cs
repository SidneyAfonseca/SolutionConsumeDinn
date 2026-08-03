using DIn.Domain;
using Microsoft.EntityFrameworkCore;

namespace DIn.Repository;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<UserLogin> Users { get; set; }
    public DbSet<DInReturn> DInReturns { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<AccountPlan> AccountPlans { get; set; }
    public DbSet<Dealer> Dealers { get; set; }
    public DbSet<DealerBranch> DealerBranches { get; set; }
    public DbSet<AccountMapping> AccountMappings { get; set; }
    public DbSet<DepartmentMapping> DepartmentMappings { get; set; }

    /// <summary>
    /// Hash PBKDF2 da senha inicial do administrador ("admin123"). Salt fixo
    /// para que o seed da migração seja determinístico — a senha deve ser
    /// trocada no primeiro acesso.
    /// </summary>
    private const string AdminSeedPasswordHash =
        "pbkdf2.sha256.100000.REluU2VlZFNhbHQyMDI2IQ==.UMSU6QvT7iFi+az8X8F20sfnepz17NyQvQzWA6ry9G0=";

    /// <summary>
    /// Hash PBKDF2 da senha inicial do usuário comum ("usuario123"). Mesma
    /// observação do hash do admin: trocar no primeiro acesso.
    /// </summary>
    private const string UserNormalSeedPasswordHash =
        "pbkdf2.sha256.100000.REluU2VlZFVzZXIyMDI2IQ==.xT6z9JCMstAg/lUOCv2rI/50mHcW6Z+u8WgAbNi3Qmk=";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserLogin>(entity =>
        {
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.Role).HasConversion<int>();
        });

        modelBuilder.Entity<Dealer>(entity =>
        {
            entity.HasIndex(d => d.HeadOfficeCnpj).IsUnique();
            entity.HasMany(d => d.Branches)
                  .WithOne(b => b.Dealer!)
                  .HasForeignKey(b => b.DealerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Uma conta (ou centro de custo) de origem tem um único destino por empresa.
        modelBuilder.Entity<DealerBranch>()
            .HasIndex(b => new { b.DealerId, b.Cnpj }).IsUnique();

        modelBuilder.Entity<AccountMapping>(entity =>
        {
            entity.HasIndex(m => new { m.DealerId, m.SourceCode }).IsUnique();
            entity.HasOne(m => m.Dealer)
                  .WithMany()
                  .HasForeignKey(m => m.DealerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DepartmentMapping>(entity =>
        {
            entity.HasIndex(m => new { m.DealerId, m.SourceCode }).IsUnique();
            entity.HasOne(m => m.Dealer)
                  .WithMany()
                  .HasForeignKey(m => m.DealerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // O envio guarda o usuário que o disparou; ao remover um usuário o
        // histórico é preservado (UserId fica nulo, Username permanece).
        modelBuilder.Entity<DInReturn>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Usuários iniciais: um de cada perfil
        modelBuilder.Entity<UserLogin>().HasData(
            new UserLogin
            {
                Id = 1,
                Username = "admin",
                PasswordHash = AdminSeedPasswordHash,
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new UserLogin
            {
                Id = 2,
                Username = "usuario",
                PasswordHash = UserNormalSeedPasswordHash,
                Role = UserRole.UserNormal,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        // Seed de Departamentos (item 7 do Guia de implementação da API D-IN v4.1)
        modelBuilder.Entity<Department>().HasData(
            new Department { Id = 1, Code = "1101", Description = "Maquinas Novas" },
            new Department { Id = 2, Code = "1102", Description = "Maquinas Usadas" },
            new Department { Id = 3, Code = "1201", Description = "Peças" },
            new Department { Id = 4, Code = "1301", Description = "Serviços" },
            new Department { Id = 5, Code = "1401", Description = "Administrativo" },
            new Department { Id = 6, Code = "1501", Description = "Outros Negócios" }
        );

        // Seed do Plano de Contas AGCO completo (item 8 do Guia de implementação da API D-IN v4.1)
        modelBuilder.Entity<AccountPlan>().HasData(
            // Ativo (1xxx)
            new AccountPlan { Id = 1, Code = "1111", Description = "Disponibilidades Imediatas" },
            new AccountPlan { Id = 2, Code = "1121", Description = "Contas a Receber Maquinas Novas" },
            new AccountPlan { Id = 3, Code = "1122", Description = "Contas a Receber Maquinas Usadas" },
            new AccountPlan { Id = 4, Code = "1123", Description = "Contas a Receber Peças" },
            new AccountPlan { Id = 5, Code = "1124", Description = "Contas a Receber Serviços" },
            new AccountPlan { Id = 6, Code = "1125", Description = "Contas a Receber Comissões e Consórcios" },
            new AccountPlan { Id = 7, Code = "1126", Description = "Contas a Receber Outros" },
            new AccountPlan { Id = 8, Code = "1127", Description = "Empresas Coligadas" },
            new AccountPlan { Id = 9, Code = "1128", Description = "Consorcio em Andamento" },
            new AccountPlan { Id = 10, Code = "1131", Description = "Adiantamentos Fornecedores" },
            new AccountPlan { Id = 11, Code = "1132", Description = "Adiantamentos Funcionários" },
            new AccountPlan { Id = 12, Code = "1133", Description = "Despesas Antecipadas - Curto Prazo" },
            new AccountPlan { Id = 13, Code = "1139", Description = "Outros Adiantamentos" },
            new AccountPlan { Id = 14, Code = "1141", Description = "Impostos a Compensar e Recuperar Curto Prazo" },
            new AccountPlan { Id = 15, Code = "1151", Description = "Máquinas e Implementos AGCO - Novos" },
            new AccountPlan { Id = 16, Code = "1152", Description = "Máquinas e Implementos AGCO - Usados" },
            new AccountPlan { Id = 17, Code = "1153", Description = "Máquinas e Implementos Outras Marcas - Novos" },
            new AccountPlan { Id = 18, Code = "1154", Description = "Máquinas e Implementos Outras Marcas - Usados" },
            new AccountPlan { Id = 19, Code = "1155", Description = "Peças AGCO" },
            new AccountPlan { Id = 20, Code = "1156", Description = "Peças Outras Marcas" },
            new AccountPlan { Id = 21, Code = "1157", Description = "Estoque de Outros Negócios - AGCO" },
            new AccountPlan { Id = 22, Code = "1158", Description = "Estoque de Outros Negócios" },
            new AccountPlan { Id = 23, Code = "1159", Description = "(-) Estoques - Ajuste a Valor Presente" },
            new AccountPlan { Id = 24, Code = "1160", Description = "(-) Provisão de Estoque Obsoleto" },
            new AccountPlan { Id = 25, Code = "1194", Description = "(-) Devolução de Vendas" },
            new AccountPlan { Id = 26, Code = "1195", Description = "Valores em Transito" },
            new AccountPlan { Id = 27, Code = "1196", Description = "(-) Contas a Receber - Ajuste a Valor Presente" },
            new AccountPlan { Id = 28, Code = "1197", Description = "(-) PCLD - Provisão de Créditos Duvidosos" },
            new AccountPlan { Id = 29, Code = "1198", Description = "Outros Créditos" },
            new AccountPlan { Id = 30, Code = "1199", Description = "Fundo AGCO" },
            new AccountPlan { Id = 31, Code = "1211", Description = "Contas a Receber - Vendas - Longo Prazo - Novos" },
            new AccountPlan { Id = 32, Code = "1212", Description = "Contas a Receber - Vendas - Longo Prazo - Usados" },
            new AccountPlan { Id = 33, Code = "1213", Description = "Aplicações Financeiras - Longo Prazo" },
            new AccountPlan { Id = 34, Code = "1214", Description = "Consórcio" },
            new AccountPlan { Id = 35, Code = "1215", Description = "Créditos com Partes Relacionadas - LP" },
            new AccountPlan { Id = 36, Code = "1216", Description = "(-) Investimentos Permanentes - Ajuste a Valor Presente" },
            new AccountPlan { Id = 37, Code = "1217", Description = "(-) Contas a Receber LP - Ajuste a Valor Presente" },
            new AccountPlan { Id = 38, Code = "1221", Description = "Impostos Diferidos" },
            new AccountPlan { Id = 39, Code = "1225", Description = "Despesas Antecipadas - Longo Prazo" },
            new AccountPlan { Id = 40, Code = "1229", Description = "Outros Créditos" },
            new AccountPlan { Id = 41, Code = "1231", Description = "Terrenos" },
            new AccountPlan { Id = 42, Code = "1232", Description = "Construções e Prédios" },
            new AccountPlan { Id = 43, Code = "1233", Description = "Máquinas/equipamentos e ferramentas" },
            new AccountPlan { Id = 44, Code = "1234", Description = "Móveis e Utensílios" },
            new AccountPlan { Id = 45, Code = "1235", Description = "Veículos" },
            new AccountPlan { Id = 46, Code = "1236", Description = "Frota para Locação" },
            new AccountPlan { Id = 47, Code = "1237", Description = "Imobilizado outros" },
            new AccountPlan { Id = 48, Code = "1238", Description = "Benfeitorias em prédio de Terceiros" },
            new AccountPlan { Id = 49, Code = "1239", Description = "Consorcios em Andamentos para Imobilziado" },
            new AccountPlan { Id = 50, Code = "1240", Description = "Computadores e Perifericos" },
            new AccountPlan { Id = 51, Code = "1296", Description = "(-) Aplicações Financeiras LP - Ajuste a Valor Presente" },
            new AccountPlan { Id = 52, Code = "1297", Description = "(-) Ativo Imobilizado - Ajuste a Valor Presente" },
            new AccountPlan { Id = 53, Code = "1298", Description = "Intangível" },
            new AccountPlan { Id = 54, Code = "1299", Description = "(-) Depreciação e Amortização" },

            // Passivo e Patrimônio Líquido (2xxx)
            new AccountPlan { Id = 55, Code = "2111", Description = "Fornecedores Maquinas Novas" },
            new AccountPlan { Id = 56, Code = "2112", Description = "Fornecedores Maquinas Usadas" },
            new AccountPlan { Id = 57, Code = "2113", Description = "Fornecedores Peças" },
            new AccountPlan { Id = 58, Code = "2114", Description = "Fornecedores Serviços" },
            new AccountPlan { Id = 59, Code = "2115", Description = "Fornecedores Outros" },
            new AccountPlan { Id = 60, Code = "2116", Description = "Contas a Pagar Partes Relacionadas" },
            new AccountPlan { Id = 61, Code = "2117", Description = "Outras contas a pagar" },
            new AccountPlan { Id = 62, Code = "2118", Description = "Contas a Pagar Empresas Coligadas" },
            new AccountPlan { Id = 63, Code = "2121", Description = "Financiamentos AGCO FINANCE CP" },
            new AccountPlan { Id = 64, Code = "2122", Description = "Empréstimos Bancários CP" },
            new AccountPlan { Id = 65, Code = "2123", Description = "Outros Empréstimos CP" },
            new AccountPlan { Id = 66, Code = "2124", Description = "Financiamentos Outros Bancos CP" },
            new AccountPlan { Id = 67, Code = "2125", Description = "Empréstimos de Sócios CP" },
            new AccountPlan { Id = 68, Code = "2126", Description = "Leasing a Pagar CP" },
            new AccountPlan { Id = 69, Code = "2127", Description = "Duplicatas Descontadas" },
            new AccountPlan { Id = 70, Code = "2128", Description = "Financiamentos FINAME" },
            new AccountPlan { Id = 71, Code = "2130", Description = "(-) Ajuste a Valor Presente - Empréstimos" },
            new AccountPlan { Id = 72, Code = "2131", Description = "Salários e Bônus a Pagar" },
            new AccountPlan { Id = 73, Code = "2132", Description = "Obrigações Trabalhistas a Recolher" },
            new AccountPlan { Id = 74, Code = "2133", Description = "Provisões Trabalhistas" },
            new AccountPlan { Id = 75, Code = "2141", Description = "Obrigações Tributárias a Recolher" },
            new AccountPlan { Id = 76, Code = "2142", Description = "Outras Obrigações Fiscais a Recolher" },
            new AccountPlan { Id = 77, Code = "2151", Description = "Adiantamento de Clientes" },
            new AccountPlan { Id = 78, Code = "2152", Description = "Outros Adiantamentos" },
            new AccountPlan { Id = 79, Code = "2161", Description = "Dividendos a Pagar" },
            new AccountPlan { Id = 80, Code = "2162", Description = "Consórcios a Pagar" },
            new AccountPlan { Id = 81, Code = "2163", Description = "Faturamento para Entrega Futura" },
            new AccountPlan { Id = 82, Code = "2164", Description = "Receitas e Custos Diferidos" },
            new AccountPlan { Id = 83, Code = "2165", Description = "Impostos Diferidos" },
            new AccountPlan { Id = 84, Code = "2166", Description = "Valores em Transito" },
            new AccountPlan { Id = 85, Code = "2197", Description = "(-) Devoluções de Compras" },
            new AccountPlan { Id = 86, Code = "2198", Description = "(-) Contas a Pagar - Reclassificação para Longo Prazo" },
            new AccountPlan { Id = 87, Code = "2199", Description = "(-) Ajuste a Valor Presente - Contas a Pagar" },
            new AccountPlan { Id = 88, Code = "2211", Description = "Financiamentos AGCO FINANCE LP" },
            new AccountPlan { Id = 89, Code = "2212", Description = "Financiamentos Outros Bancos LP" },
            new AccountPlan { Id = 90, Code = "2213", Description = "Empréstimos Bancários LP" },
            new AccountPlan { Id = 91, Code = "2214", Description = "(-) Ajuste a Valor Presente - Empréstimos" },
            new AccountPlan { Id = 92, Code = "2221", Description = "Contas a Pagar - Empresas Coligadas LP" },
            new AccountPlan { Id = 93, Code = "2222", Description = "Contas a Pagar e Fornecedores LP" },
            new AccountPlan { Id = 94, Code = "2223", Description = "Outros Investimentos a pagar" },
            new AccountPlan { Id = 95, Code = "2224", Description = "Contas a Pagar de Partes Relacionadas LP" },
            new AccountPlan { Id = 96, Code = "2225", Description = "Outros Empréstimos LP" },
            new AccountPlan { Id = 97, Code = "2226", Description = "Parcelamento de Tributos" },
            new AccountPlan { Id = 98, Code = "2227", Description = "Provisões para Contingências" },
            new AccountPlan { Id = 99, Code = "2231", Description = "Capital Social Integralizado" },
            new AccountPlan { Id = 100, Code = "2232", Description = "Adiantamentos para Futuro Aumento de Capital" },
            new AccountPlan { Id = 101, Code = "2233", Description = "Reservas de Lucro" },
            new AccountPlan { Id = 102, Code = "2234", Description = "Reservas de Capital" },
            new AccountPlan { Id = 103, Code = "2235", Description = "(-) Ações/Quotas em Tesouraria" },
            new AccountPlan { Id = 104, Code = "2236", Description = "(-) Dividendos Distribuídos" },
            new AccountPlan { Id = 105, Code = "2237", Description = "(-) Juros Capital Próprio" },
            new AccountPlan { Id = 106, Code = "2238", Description = "(-) Prejuízos Acumulado" },
            new AccountPlan { Id = 107, Code = "2239", Description = "Resultado do Exercício" },
            new AccountPlan { Id = 108, Code = "2299", Description = "(-) Ajuste a Valor Presente - Outras Contas a Pagar LP" },

            // Receitas (3xxx)
            new AccountPlan { Id = 109, Code = "3111", Description = "Comissões sobre Vendas - Consórcio" },
            new AccountPlan { Id = 110, Code = "3112", Description = "(-) Dedução de Comissões sobre Vendas - Consórcio" },
            new AccountPlan { Id = 111, Code = "3113", Description = "Comissões sobre Vendas de Máquinas" },
            new AccountPlan { Id = 112, Code = "3114", Description = "(-) Dedução de Comissões sobre Vendas de Máquinas" },
            new AccountPlan { Id = 113, Code = "3115", Description = "Receita com Outras Comissões" },
            new AccountPlan { Id = 114, Code = "3116", Description = "(-) Dedução de Outras Comissões" },
            new AccountPlan { Id = 115, Code = "3117", Description = "Máquinas AGCO - Novos" },
            new AccountPlan { Id = 116, Code = "3118", Description = "(-) Dedução de Máquinas AGCO - Novos" },
            new AccountPlan { Id = 117, Code = "3119", Description = "Implementos AGCO - Novos" },
            new AccountPlan { Id = 118, Code = "3120", Description = "(-) Dedução de Implementos AGCO - Novos" },
            new AccountPlan { Id = 119, Code = "3121", Description = "Implementos Outras Marcas - Novos" },
            new AccountPlan { Id = 120, Code = "3122", Description = "(-) Dedução de Máquinas Outras Marcas - Novos" },
            new AccountPlan { Id = 121, Code = "3131", Description = "Máquinas e Implementos AGCO - Usados" },
            new AccountPlan { Id = 122, Code = "3132", Description = "(-) Dedução de Máquinas e Implementos AGCO - Usados" },
            new AccountPlan { Id = 123, Code = "3133", Description = "Máquinas e implementos Outras Marcas - Usados" },
            new AccountPlan { Id = 124, Code = "3134", Description = "(-) Dedução de Máquinas e Implementos Outras Marcas - Usados" },
            new AccountPlan { Id = 125, Code = "3211", Description = "Receita com Comissões Sobre Vendas de Peças" },
            new AccountPlan { Id = 126, Code = "3212", Description = "(-) Dedução de Comissões Sobre Vendas de Peças" },
            new AccountPlan { Id = 127, Code = "3213", Description = "Receitas Peças AGCO" },
            new AccountPlan { Id = 128, Code = "3214", Description = "Receitas Peças Oficina AGCO" },
            new AccountPlan { Id = 129, Code = "3215", Description = "(-) Dedução de Peças AGCO" },
            new AccountPlan { Id = 130, Code = "3216", Description = "(-) Dedução de Peças Oficina AGCO" },
            new AccountPlan { Id = 131, Code = "3221", Description = "Peças Outras Marcas" },
            new AccountPlan { Id = 132, Code = "3222", Description = "Peças Oficina Outras Marcas" },
            new AccountPlan { Id = 133, Code = "3223", Description = "(-) Dedução de Peças Outras Marcas" },
            new AccountPlan { Id = 134, Code = "3224", Description = "(-) Dedução de Peças Oficina Outras Marcas" },
            new AccountPlan { Id = 135, Code = "3225", Description = "Lubrificantes" },
            new AccountPlan { Id = 136, Code = "3226", Description = "(-) Dedução de Lubrificantes" },
            new AccountPlan { Id = 137, Code = "3311", Description = "Receita com Serviços Prestados a Clientes" },
            new AccountPlan { Id = 138, Code = "3312", Description = "(-) Dedução de Serviços Prestados a Clientes" },
            new AccountPlan { Id = 139, Code = "3313", Description = "Receita com Serviços Internos" },
            new AccountPlan { Id = 140, Code = "3314", Description = "(-) Dedução de Serviços Internos" },
            new AccountPlan { Id = 141, Code = "3315", Description = "Receita com Serviços de Garantia" },
            new AccountPlan { Id = 142, Code = "3316", Description = "(-) Dedução de Serviços de Garantia" },
            new AccountPlan { Id = 143, Code = "3317", Description = "Receita com Serviços Recall" },
            new AccountPlan { Id = 144, Code = "3318", Description = "(-) Dedução de Serviços Recall" },
            new AccountPlan { Id = 145, Code = "3319", Description = "Receita com KM de Serviços de Assistência a Campo" },
            new AccountPlan { Id = 146, Code = "3320", Description = "(-) Dedução de KM de Serviços de Assistência a Campo" },
            new AccountPlan { Id = 147, Code = "3321", Description = "Receita com deslocamento de Assistência a Campo" },
            new AccountPlan { Id = 148, Code = "3322", Description = "(-) Dedução de deslocamento de Assistência a Campo" },
            new AccountPlan { Id = 149, Code = "3323", Description = "Serviços Realizados por Terceiros" },
            new AccountPlan { Id = 150, Code = "3324", Description = "(-) Dedução de Serviços Realizados por Terceiros" },
            new AccountPlan { Id = 151, Code = "3411", Description = "Receitas de Tecnologia - ACGO" },
            new AccountPlan { Id = 152, Code = "3412", Description = "Receita Outros Negócios AGCO" },
            new AccountPlan { Id = 153, Code = "3413", Description = "(-) Dedução de Receitas Tecnologia - AGCO" },
            new AccountPlan { Id = 154, Code = "3414", Description = "(-) Dedução de Receitas com Outros Negócios - AGCO" },
            new AccountPlan { Id = 155, Code = "3511", Description = "Receita Outros Negócios" },
            new AccountPlan { Id = 156, Code = "3512", Description = "(-) Dedução de Receitas com Outros Negócios" },

            // Custos (4xxx)
            new AccountPlan { Id = 157, Code = "4117", Description = "Custo Máquinas AGCO - Novos" },
            new AccountPlan { Id = 158, Code = "4119", Description = "Custo Implementos AGCO - Novos" },
            new AccountPlan { Id = 159, Code = "4121", Description = "Custo Máquinas Outras Marcas - Novos" },
            new AccountPlan { Id = 160, Code = "4131", Description = "Custo Máquinas e Implementos AGCO - Usados" },
            new AccountPlan { Id = 161, Code = "4133", Description = "Custo Máquinas e implementos Outras Marcas - Usados" },
            new AccountPlan { Id = 162, Code = "4213", Description = "Custo Peças AGCO" },
            new AccountPlan { Id = 163, Code = "4214", Description = "Custo Peças Oficina AGCO" },
            new AccountPlan { Id = 164, Code = "4221", Description = "Peças Outras Marcas" },
            new AccountPlan { Id = 165, Code = "4222", Description = "Peças Oficina Outras Marcas" },
            new AccountPlan { Id = 166, Code = "4311", Description = "Materiais de Consumo - Oficina" },
            new AccountPlan { Id = 167, Code = "4312", Description = "Salários e Encargos dos Técnicos" },
            new AccountPlan { Id = 168, Code = "4313", Description = "Custos Serviços Realizados por Terceiros" },
            new AccountPlan { Id = 169, Code = "4314", Description = "Custo com deslocamento técnicos" },
            new AccountPlan { Id = 170, Code = "4315", Description = "Custos com Pós - Venda/Garantias Indeferidas e Retrabalhos" },
            new AccountPlan { Id = 171, Code = "4319", Description = "Uso e Consumo Oficina" },
            new AccountPlan { Id = 172, Code = "4320", Description = "Ferramental de pequeno valor" },
            new AccountPlan { Id = 173, Code = "4399", Description = "Outros Custos Sobre Serviços" },
            new AccountPlan { Id = 174, Code = "4411", Description = "Custo de Tecnologia ACGO" },
            new AccountPlan { Id = 175, Code = "4412", Description = "Custo com Outros Negócios - AGCO" },
            new AccountPlan { Id = 176, Code = "4511", Description = "Custo com Outros Negócios" },

            // Despesas Operacionais (5xxx)
            new AccountPlan { Id = 177, Code = "5111", Description = "Brindes" },
            new AccountPlan { Id = 178, Code = "5112", Description = "Comissões e Encargo Folha de Pagamento" },
            new AccountPlan { Id = 179, Code = "5113", Description = "Bônus e Participação nos Resultados" },
            new AccountPlan { Id = 180, Code = "5114", Description = "Despesas com Ajustes de Acordo Comercial" },
            new AccountPlan { Id = 181, Code = "5116", Description = "Perdas com Devedores Duvidosos" },
            new AccountPlan { Id = 182, Code = "5117", Description = "Taxa Flat de Financiamento" },
            new AccountPlan { Id = 183, Code = "5118", Description = "Despesas de Consumo Interno Oficina" },
            new AccountPlan { Id = 184, Code = "5119", Description = "Fretes" },
            new AccountPlan { Id = 185, Code = "5120", Description = "Despesa com reforma de máquinas usadas" },
            new AccountPlan { Id = 186, Code = "5121", Description = "Remuneração Dirigentes" },
            new AccountPlan { Id = 187, Code = "5122", Description = "Salários - Gerência e Supervisão" },
            new AccountPlan { Id = 188, Code = "5123", Description = "Salários - Funcionários" },
            new AccountPlan { Id = 189, Code = "5124", Description = "Provisões - 13o. Sal. e Férias" },
            new AccountPlan { Id = 190, Code = "5125", Description = "Encargos Sociais - todos" },
            new AccountPlan { Id = 191, Code = "5126", Description = "Benefícios" },
            new AccountPlan { Id = 192, Code = "5127", Description = "Água, Luz e Gás" },
            new AccountPlan { Id = 193, Code = "5128", Description = "Aluguel" },
            new AccountPlan { Id = 194, Code = "5129", Description = "Amortizações e Depreciações" },
            new AccountPlan { Id = 195, Code = "5130", Description = "Despesa com Alimentação" },
            new AccountPlan { Id = 196, Code = "5131", Description = "Despesas com Material de Segurança e Uniformes" },
            new AccountPlan { Id = 197, Code = "5132", Description = "Despesas com Móveis e Utensílios" },
            new AccountPlan { Id = 198, Code = "5133", Description = "Despesas de Comunicação" },
            new AccountPlan { Id = 199, Code = "5134", Description = "Donativos e Contribuições" },
            new AccountPlan { Id = 200, Code = "5135", Description = "Eventos e Confraternização" },
            new AccountPlan { Id = 201, Code = "5136", Description = "Garantias Recusadas - AGCO" },
            new AccountPlan { Id = 202, Code = "5137", Description = "Impostos e Taxas Diversos" },
            new AccountPlan { Id = 203, Code = "5138", Description = "Leasing" },
            new AccountPlan { Id = 204, Code = "5139", Description = "Legais e Auditoria" },
            new AccountPlan { Id = 205, Code = "5140", Description = "Manutenção de Veículos frota da Adm" },
            new AccountPlan { Id = 206, Code = "5141", Description = "Manutenção Predial" },
            new AccountPlan { Id = 207, Code = "5142", Description = "Materiais Diversos / Escritório" },
            new AccountPlan { Id = 208, Code = "5143", Description = "Multas" },
            new AccountPlan { Id = 209, Code = "5144", Description = "Processamento Dados - Terceiros" },
            new AccountPlan { Id = 210, Code = "5145", Description = "Propaganda e Promoção de Vendas" },
            new AccountPlan { Id = 211, Code = "5146", Description = "Publicações, Assinaturas e Assoc." },
            new AccountPlan { Id = 212, Code = "5147", Description = "Retorno de Garantia ( Pecas e Serviços )" },
            new AccountPlan { Id = 213, Code = "5148", Description = "Seguros" },
            new AccountPlan { Id = 214, Code = "5149", Description = "Serviços Profissionais - Terceiros" },
            new AccountPlan { Id = 215, Code = "5150", Description = "Treinamento" },
            new AccountPlan { Id = 216, Code = "5151", Description = "Combustíveis e Lubrificantes" },
            new AccountPlan { Id = 217, Code = "5152", Description = "Viagens, Represent. e Hospitalidade" },
            new AccountPlan { Id = 218, Code = "5153", Description = "Taxas Associativas" },
            new AccountPlan { Id = 219, Code = "5154", Description = "Sistemas, Software e Informática" },
            new AccountPlan { Id = 220, Code = "5155", Description = "Postagens, Correios e Telégrafos" },
            new AccountPlan { Id = 221, Code = "5156", Description = "Aviso Prévio" },
            new AccountPlan { Id = 222, Code = "5157", Description = "Ações Trabalhistas" },
            new AccountPlan { Id = 223, Code = "5158", Description = "Estágio" },
            new AccountPlan { Id = 224, Code = "5199", Description = "Outros Despesas" },

            // Resultado Financeiro (6xxx)
            new AccountPlan { Id = 225, Code = "6111", Description = "Rendimento Aplicações Financeiras" },
            new AccountPlan { Id = 226, Code = "6112", Description = "Juros Recebidos" },
            new AccountPlan { Id = 227, Code = "6113", Description = "Variações Monetárias Ativas" },
            new AccountPlan { Id = 228, Code = "6114", Description = "Descontos Obtidos" },
            new AccountPlan { Id = 229, Code = "6115", Description = "Juros de CAR" },
            new AccountPlan { Id = 230, Code = "6121", Description = "Despesas Financeiras s/ Capital Giro" },
            new AccountPlan { Id = 231, Code = "6122", Description = "IOF" },
            new AccountPlan { Id = 232, Code = "6123", Description = "Despesa com Juros" },
            new AccountPlan { Id = 233, Code = "6124", Description = "Multas Financeiras" },
            new AccountPlan { Id = 234, Code = "6125", Description = "Juros sobre Capital Próprio" },
            new AccountPlan { Id = 235, Code = "6126", Description = "Variações Monetárias Passivas" },
            new AccountPlan { Id = 236, Code = "6127", Description = "Despesas Bancárias" },
            new AccountPlan { Id = 237, Code = "6128", Description = "Descontos Passivos" },

            // Outras Receitas e Despesas (7xxx)
            new AccountPlan { Id = 238, Code = "7111", Description = "Ganho com Venda de Ativo Imobilizado" },
            new AccountPlan { Id = 239, Code = "7112", Description = "Outras Receitas" },
            new AccountPlan { Id = 240, Code = "7121", Description = "Perda com Venda de Ativo" },
            new AccountPlan { Id = 241, Code = "7122", Description = "Outras Despesas" },

            // Impostos sobre o Lucro (8xxx)
            new AccountPlan { Id = 242, Code = "8111", Description = "IRPJ" },
            new AccountPlan { Id = 243, Code = "8112", Description = "CSLL" },

            // Contas Gerenciais (9xxx)
            new AccountPlan { Id = 244, Code = "9111", Description = "Diretores" },
            new AccountPlan { Id = 245, Code = "9112", Description = "Gerentes" },
            new AccountPlan { Id = 246, Code = "9113", Description = "Supervisores" },
            new AccountPlan { Id = 247, Code = "9114", Description = "Coordenadores" },
            new AccountPlan { Id = 248, Code = "9115", Description = "Vendedores" },
            new AccountPlan { Id = 249, Code = "9116", Description = "Analistas" },
            new AccountPlan { Id = 250, Code = "9117", Description = "Assistentes" },
            new AccountPlan { Id = 251, Code = "9118", Description = "Auxiliares" },
            new AccountPlan { Id = 252, Code = "9119", Description = "Diretores" },
            new AccountPlan { Id = 253, Code = "9211", Description = "Novos" },
            new AccountPlan { Id = 254, Code = "9212", Description = "Usados" },
            new AccountPlan { Id = 255, Code = "9213", Description = "Outros Negócios" },
            new AccountPlan { Id = 256, Code = "9311", Description = "Trator" },
            new AccountPlan { Id = 257, Code = "9312", Description = "Colheitadeira" },
            new AccountPlan { Id = 258, Code = "9313", Description = "Colhedora" },
            new AccountPlan { Id = 259, Code = "9314", Description = "Plantadeira" },
            new AccountPlan { Id = 260, Code = "9315", Description = "Pulverizador" },
            new AccountPlan { Id = 261, Code = "9316", Description = "Plataforma" },
            new AccountPlan { Id = 262, Code = "9317", Description = "Implementos" },
            new AccountPlan { Id = 263, Code = "9318", Description = "Outros" },
            new AccountPlan { Id = 264, Code = "9411", Description = "Trator" },
            new AccountPlan { Id = 265, Code = "9412", Description = "Colheitadeira" },
            new AccountPlan { Id = 266, Code = "9413", Description = "Colhedora" },
            new AccountPlan { Id = 267, Code = "9414", Description = "Plantadeira" },
            new AccountPlan { Id = 268, Code = "9415", Description = "Pulverizador" },
            new AccountPlan { Id = 269, Code = "9416", Description = "Plataforma" },
            new AccountPlan { Id = 270, Code = "9417", Description = "Implementos" },
            new AccountPlan { Id = 271, Code = "9418", Description = "Outros" },
            new AccountPlan { Id = 272, Code = "9419", Description = "Colheitadeira" },
            new AccountPlan { Id = 273, Code = "9420", Description = "Colhedora" },
            new AccountPlan { Id = 274, Code = "9421", Description = "Plantadeira" },
            new AccountPlan { Id = 275, Code = "9422", Description = "Pulverizador" },
            new AccountPlan { Id = 276, Code = "9423", Description = "Plataforma" },
            new AccountPlan { Id = 277, Code = "9424", Description = "Implementos" },
            new AccountPlan { Id = 278, Code = "9511", Description = "Horas Disponíveis" },
            new AccountPlan { Id = 279, Code = "9512", Description = "Horas Faturadas" },
            new AccountPlan { Id = 280, Code = "9513", Description = "Tempo de O.S." },
            new AccountPlan { Id = 281, Code = "9514", Description = "Valor O.S. em Aberto" },
            new AccountPlan { Id = 282, Code = "9611", Description = "Tempo de Pedido" },
            new AccountPlan { Id = 283, Code = "9612", Description = "Horas Faturadas" },
            new AccountPlan { Id = 284, Code = "9613", Description = "Valor Pedidos Abertos" }
        );
    }
}
