using Microsoft.EntityFrameworkCore;
using NoPrumo.Domain.Entities;

namespace NoPrumo.Infrastructure.Data;

/// <summary>
/// A migration cria as tabelas vazias — sem esta rotina não existe papel nem
/// usuário, e ninguém entra. Roda a cada start e só insere o que falta.
/// </summary>
public sealed class DatabaseSeeder(AppDbContext appDbContext)
{
    private static readonly (string Code, string Description)[] PermissionCatalog =
    [
        ("manage_users",     "Criar e editar usuários do sistema"),
        ("manage_projects",  "Cadastrar obras, etapas e prazos"),
        ("manage_employees", "Cadastrar funcionários e equipes"),
        ("view_stock",       "Ver saldo e movimentação de estoque"),
        ("manage_stock",     "Lançar entrada e saída de estoque, EPI e ferramentas"),
        ("manage_purchases", "Cotar, comprar e registrar nota fiscal"),
        ("manage_safety",    "Treinamentos de NR, ASO e fichas de EPI"),
        ("view_finance",     "Ver contrato, custo, salário e margem"),
    ];

    /// <summary>
    /// As três categorias que definem os fluxos do sistema inteiro. As duas flags
    /// dizem qual é qual: consumo tem saldo por obra, EPI vai do depósito para o
    /// funcionário, ferramenta vai e volta.
    /// </summary>
    private static readonly (string Name, bool TracksProjectBalance, bool RequiresReturn)[] StockCategoryCatalog =
    [
        ("Consumable material", true,  false),
        ("PPE",                 false, false),
        ("Tool",                false, true),
    ];

    private static (string Name, string Description, string[] Permissions)[] RoleCatalog()
    {
        var all = PermissionCatalog.Select(permission => permission.Code).ToArray();

        return
        [
            ("admin",             "Administrador",        all),
            ("engineer",          "Engenheiro",           ["manage_projects", "manage_employees", "view_finance"]),
            // Mestre enxerga quantidade, administração enxerga dinheiro.
            ("foreman",           "Mestre de obras",      ["view_stock", "manage_stock"]),
            ("safety_technician", "Técnico de segurança", ["manage_safety"]),
            ("warehouse_keeper",  "Almoxarife",           ["view_stock", "manage_stock"]),
            ("purchasing",        "Compras",              ["manage_purchases", "view_finance"]),
        ];
    }

    public async Task SeedAsync()
    {
        await SeedPermissionsAsync();
        await SeedRolesAsync();
        await SeedStockCategoriesAsync();
        await SeedFirstAdminAsync();
    }

    private async Task SeedPermissionsAsync()
    {
        var existing = await appDbContext.Permission.Select(permission => permission.Code).ToListAsync();

        var missing = PermissionCatalog
            .Where(item => !existing.Contains(item.Code))
            .Select(item => new Permission { Code = item.Code, Description = item.Description })
            .ToList();

        if (missing.Count == 0) return;

        appDbContext.Permission.AddRange(missing);
        await appDbContext.SaveChangesAsync();
    }

    private async Task SeedRolesAsync()
    {
        var permissions = await appDbContext.Permission.ToDictionaryAsync(permission => permission.Code);

        // Sem o Include o EF veria a coleção vazia e duplicaria as ligações.
        var roles = await appDbContext.Role.Include(role => role.Permissions).ToListAsync();

        foreach (var (name, description, codes) in RoleCatalog())
        {
            var role = roles.FirstOrDefault(existingRole => existingRole.Name == name);

            if (role is null)
            {
                role = new Role { Name = name, Description = description };
                appDbContext.Role.Add(role);
                roles.Add(role);
            }
            else if (role.Description != description)
            {
                // O rótulo é do catálogo, não do banco: se mudou aqui, sincroniza.
                role.Description = description;
            }

            foreach (var code in codes)
            {
                var alreadyLinked = role.Permissions.Any(permission => permission.Code == code);

                if (!alreadyLinked && permissions.TryGetValue(code, out var permission))
                {
                    role.Permissions.Add(permission);
                }
            }
        }

        await appDbContext.SaveChangesAsync();
    }

    private async Task SeedStockCategoriesAsync()
    {
        var existing = await appDbContext.StockCategory.Select(category => category.Name).ToListAsync();

        // Comparar sem diferenciar maiúscula, para "ppe" e "PPE" não virarem duas categorias.
        var missing = StockCategoryCatalog
            .Where(item => !existing.Contains(item.Name, StringComparer.OrdinalIgnoreCase))
            .Select(item => new StockCategory
            {
                Name = item.Name,
                TracksProjectBalance = item.TracksProjectBalance,
                RequiresReturn = item.RequiresReturn,
            })
            .ToList();

        if (missing.Count == 0) return;

        appDbContext.StockCategory.AddRange(missing);
        await appDbContext.SaveChangesAsync();
    }

    private async Task SeedFirstAdminAsync()
    {
        // Só em banco sem nenhum usuário: recriar o admin num banco povoado seria uma porta dos fundos.
        if (await appDbContext.User.AnyAsync()) return;

        var adminRole = await appDbContext.Role.FirstAsync(role => role.Name == "admin");

        appDbContext.User.Add(new User
        {
            Username = "admin",
            Name = "Administrador",
            // Senha de instalação. MustChangePassword obriga a troca no
            // primeiro acesso, então ela não sobrevive ao primeiro login.
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin", workFactor: 12),
            RoleId = adminRole.Id,
            Active = true,
            MustChangePassword = true,
        });

        await appDbContext.SaveChangesAsync();
    }
}