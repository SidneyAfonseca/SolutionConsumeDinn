using DIn.Domain;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed de Departamentos (Extraídos do Guia API D-IN)
        modelBuilder.Entity<Department>().HasData(
            new Department { Id = 1, Code = "1101", Description = "Maquinas Novas" },
            new Department { Id = 2, Code = "1102", Description = "Maquinas Usadas" },
            new Department { Id = 3, Code = "1201", Description = "Peças" },
            new Department { Id = 4, Code = "1301", Description = "Serviços" },
            new Department { Id = 5, Code = "1401", Description = "Administrativo" },
            new Department { Id = 6, Code = "1501", Description = "Outros Negócios" }
        );

        // Seed de Plano de Contas (Amostra principal extraída do Guia API D-IN v4.1)
        modelBuilder.Entity<AccountPlan>().HasData(
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
            new AccountPlan { Id = 13, Code = "1151", Description = "Máquinas e Implementos AGCO - Novos" },
            new AccountPlan { Id = 14, Code = "1152", Description = "Máquinas e Implementos AGCO - Usados" },
            new AccountPlan { Id = 15, Code = "1155", Description = "Peças AGCO" },
            new AccountPlan { Id = 16, Code = "2111", Description = "Fornecedores Maquinas Novas" },
            new AccountPlan { Id = 17, Code = "2112", Description = "Fornecedores Maquinas Usadas" },
            new AccountPlan { Id = 18, Code = "2113", Description = "Fornecedores Peças" },
            new AccountPlan { Id = 19, Code = "2114", Description = "Fornecedores Serviços" },
            new AccountPlan { Id = 20, Code = "2131", Description = "Salários e Bônus a Pagar" }
        );
    }
}
