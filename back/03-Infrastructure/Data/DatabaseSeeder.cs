using Microsoft.EntityFrameworkCore;
using NoPrumo.Domain.Entities;

namespace NoPrumo.Infrastructure.Data;

/// <summary>
/// A migration cria as tabelas vazias — sem esta rotina não existe papel nem
/// usuário, e ninguém entra. Roda a cada start e só insere o que falta.
/// </summary>
public static class DatabaseSeeder
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

    private static (string Name, string Description, string[] Permissions)[] RoleCatalog()
    {
        var all = PermissionCatalog.Select(p => p.Code).ToArray();

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

    public static async Task SeedAsync(AppDbContext db)
    {
        await SeedPermissionsAsync(db);
        await SeedRolesAsync(db);
        await SeedFirstAdminAsync(db);
    }

    private static async Task SeedPermissionsAsync(AppDbContext db)
    {
        var existing = await db.Permission.Select(p => p.Code).ToListAsync();

        var missing = PermissionCatalog
            .Where(item => !existing.Contains(item.Code))
            .Select(item => new Permission { Code = item.Code, Description = item.Description })
            .ToList();

        if (missing.Count == 0) return;

        db.Permission.AddRange(missing);
        await db.SaveChangesAsync();
    }

    private static async Task SeedRolesAsync(AppDbContext db)
    {
        var permissions = await db.Permission.ToDictionaryAsync(p => p.Code);

        // Sem o Include o EF veria a coleção vazia e duplicaria as ligações.
        var roles = await db.Role.Include(r => r.Permissions).ToListAsync();

        foreach (var (name, description, codes) in RoleCatalog())
        {
            var role = roles.FirstOrDefault(r => r.Name == name);

            if (role is null)
            {
                role = new Role { Name = name, Description = description };
                db.Role.Add(role);
                roles.Add(role);
            }
            else if (role.Description != description)
            {
                // O rótulo é do catálogo, não do banco: se mudou aqui, sincroniza.
                role.Description = description;
            }

            foreach (var code in codes)
            {
                var alreadyLinked = role.Permissions.Any(p => p.Code == code);

                if (!alreadyLinked && permissions.TryGetValue(code, out var permission))
                {
                    role.Permissions.Add(permission);
                }
            }
        }

        await db.SaveChangesAsync();
    }

    private static async Task SeedFirstAdminAsync(AppDbContext db)
    {
        // Só em banco sem nenhum usuário: recriar o admin num banco povoado seria uma porta dos fundos.
        if (await db.User.AnyAsync()) return;

        var adminRole = await db.Role.FirstAsync(r => r.Name == "admin");

        db.User.Add(new User
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

        await db.SaveChangesAsync();
    }
}