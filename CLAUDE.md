# NoPrumo — instruções para o Claude Code

Sistema de gestão de obras para construtoras de médio e grande porte.
Projeto Integrador Entra21.

---

## ⚠️ Limite de atuação

**O Claude Code trabalha SOMENTE na pasta `front/`.**

Não editar, criar nem apagar nada em:

| Caminho | Motivo |
|---|---|
| `back/` | a equipe escreve o back-end à mão, é o aprendizado do módulo |
| `back/03-Infrastructure/Migrations/` | migration é gerada por comando, nunca editada por IA |
| banco de dados MySQL | ninguém roda SQL destrutivo sem a equipe pedir |
| `docs/historico/` | registro histórico, não se altera |

Se uma tarefa do front exigir mudança no back (endpoint novo, campo faltando
num DTO), **pare e avise** — não implemente do outro lado. A equipe decide.

Ler o `back/` para entender contratos de API é permitido e esperado.

### Git é da equipe

**O Claude não commita, não faz merge, não faz push e não abre PR.**

Nenhum comando que altere o histórico ou o estado do repositório:
`commit`, `merge`, `rebase`, `push`, `reset`, `revert`, `checkout` de branch,
`cherry-pick`, `stash`, `tag`, `gh pr create`.

O Claude edita os arquivos e **para**. Quem revisa, commita, mergeia e publica é
a equipe — é assim que todo mundo continua sabendo o que entrou no projeto.

Comandos de leitura são permitidos: `git status`, `git diff`, `git log`,
`git show`, `git blame`.

Se pedirem uma mensagem de commit ou descrição de PR, **escreva o texto** para a
pessoa copiar — não execute.

---

## Stack

**Back-end** (feito pela equipe, não mexer)
.NET 8 · ASP.NET Core Web API · EF Core 8 + Pomelo · MySQL 8 · Clean Architecture

**Front-end** (aqui o Claude atua)
A definir pela equipe. Consumir a API REST do back.

---

## Estrutura

```
NoPrumo/
├── back/                    NÃO MEXER
│   ├── 01-Presentation/     Controllers, Program.cs
│   ├── 02-Application/      DTOs, services
│   ├── 03-Infrastructure/   AppDbContext, Migrations
│   ├── 04-Domain/Entities/  36 entidades (37 tabelas, sem views)
│   └── 05-Tests/
├── front/                   área de trabalho do Claude
└── docs/
    ├── ROADMAP.md           o que falta fazer
    └── historico/           material antigo, não alterar
```

---

## Regras de produto que o front precisa respeitar

Estas não são preferências — são decisões de negócio já fechadas.

### Quem enxerga o quê

> **Mestre enxerga quantidade. Admin enxerga dinheiro.**

O mestre de obras **nunca** vê valor de contrato, custo de mão de obra, salário,
preço de material ou margem. Não basta esconder na tela: se o dado vier no JSON,
qualquer um abre o DevTools e lê.

Por isso existem DTOs separados por perfil. Nunca montar um DTO único com campos
nulos para o mestre — isso falha aberto: alguém adiciona um campo novo, esquece
de anular, e vaza.

### Perfis

Seis perfis, todos funcionários da empresa: `admin`, `engineer`, `foreman`,
`safety_technician`, `warehouse_keeper`, `purchasing`.

- **Cliente não tem login** — acessa por link com token (`project_link`)
- **Empreiteiro não tem login** — é fornecedor, não usuário
- **Não existe auto-cadastro** — o admin cria o usuário e entrega senha provisória

A tela de "cadastro" é uma tela interna de gestão de usuários, não um signup
público.

### Estoque

Três fluxos diferentes, não confundir:

| Tipo | Onde fica o saldo |
|---|---|
| Material de consumo | **por obra** (entrega direto no canteiro, não há depósito) |
| EPI | depósito da empresa → funcionário |
| Ferramenta | depósito → funcionário → **volta** |

Saldo **nunca** vem de uma coluna guardada — é sempre somado de `stock_movement`,
que é o razão. As views que faziam essa soma foram removidas; agora a conta é
feita em C#.

### Compras

O mestre **solicita** informando só quantidade. A administração cota, compra e
registra preço e nota fiscal. Tela de solicitação do mestre não mostra valor.

### Status derivado

`atrasada` **não** é um status gravado — é calculado a partir da data. Não existe
esse valor em `project.status` nem em `stage.status`, e as views que faziam o
cálculo foram removidas: comparar a data prevista com hoje, em C#, nunca ler o
campo `status` cru quando o assunto for atraso.

### O que o banco não valida

A migration é gerada **só** pelo comando, sem uma linha de SQL escrita à mão —
nunca editar o arquivo depois de gerado. Isso tem um preço que o código paga:

| Não existe no banco | Quem resolve |
|---|---|
| CHECK constraints — status válido, `valor > 0`, data fim ≥ data início | validação na entidade ou no service |
| `DEFAULT 0.00` nas colunas numéricas | valor padrão da propriedade em C# |
| Seeds — papéis, permissões, tipos de NR, setores, funções | rotina de seed em C# |
| Usuário inicial | **não existe nenhum** — sem essa rotina ninguém entra no sistema |

O banco ainda garante FK, unique, `NOT NULL`, os defaults declarados no
`AppDbContext` (`status`, `active`, `created_at`) e as 7 colunas calculadas
(`active_key` e `time_entry.hours`).

Se algo precisar existir no banco, declare no `AppDbContext`
(`HasCheckConstraint`, `HasDefaultValue`, `HasData`) e deixe o comando gerar.

---

## Convenções

- **Idioma:** código e banco em **inglês** — classe, propriedade, tabela, coluna,
  nome de arquivo e valor gravado (`status = 'pending'`, não `'pendente'`). Texto
  de tela e mensagem de commit em **português**, que é a língua de quem usa o
  sistema e de quem revisa o PR.
  Glossário: obra = `Project` · etapa = `Stage` · ficha de EPI = `PpeIssue` ·
  empreitada = `Subcontract` · setor = `Department` · função = `JobRole` ·
  papel = `Role` · ponto = `TimeEntry` · fornecedor = `Supplier`.
  Sigla brasileira não se traduz: `cno`, `crea_rt`, `ca`, `inss`, `NR-35`.
- **Banco:** `snake_case`. **C#:** `PascalCase`. A conversão é automática pelo
  `UseSnakeCaseNamingConvention()` no `Program.cs`, e por isso o `AppDbContext`
  não declara nome de tabela nem de coluna. Renomear uma propriedade renomeia a
  coluna — o nome no banco é consequência do nome em C#, não uma escolha à parte.
- **Commits:** `tipo(escopo): descrição` — ex. `feat(front): tela de login`
- **Uma branch por feature**, nunca commitar direto na `main`

---

## Ao trabalhar no front

- Ler `docs/ROADMAP.md` antes de começar — ele diz o que está pronto e o que não está
- Conferir o contrato real do endpoint em `back/01-Presentation/Controllers/`
  antes de assumir formato de resposta
- Não inventar endpoint que não existe. Se faltar, avisar a equipe
- Não commitar credencial, token ou connection string
- A API roda em `http://localhost:5262` (ver `back/01-Presentation/Properties/launchSettings.json`)

---

## Banco local

Cada pessoa tem o seu, criado pela migration. O Claude não roda esses comandos —
estão aqui só como referência de como o ambiente funciona:

```
dotnet ef database update --project back/03-Infrastructure --startup-project back/01-Presentation
```

A connection string fica em User Secrets, nunca no `appsettings.json`.
