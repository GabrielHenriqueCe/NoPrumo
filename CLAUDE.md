# NoPrumo — instruções para o Claude Code

Sistema de gestão de obras para construtoras de médio e grande porte.
Projeto Integrador Entra21.

## ⚠️ Limite de atuação

**O Claude Code edita SOMENTE a pasta `front/`.** Ler o `back/` para entender
contratos de API é permitido e esperado.

| Não mexer | Motivo |
|---|---|
| `back/` | a equipe escreve o back-end à mão, é o aprendizado do módulo |
| `back/03-Infrastructure/Migrations/` | gerada por comando, nunca editada |
| banco MySQL | ninguém roda SQL destrutivo sem a equipe pedir |
| `docs/historico/` | registro histórico |

Se o front precisar de mudança no back (endpoint novo, campo num DTO), **pare e
avise** — a equipe decide.

**Git é da equipe.** O Claude não roda nada que altere o repositório (`commit`,
`merge`, `rebase`, `push`, `reset`, `revert`, `checkout` de branch,
`cherry-pick`, `stash`, `tag`, `gh pr create`). Leitura pode (`status`, `diff`,
`log`, `show`, `blame`). Mensagem de commit ou PR: escrever o texto para a
pessoa copiar.

## Stack e estrutura

- **Back** (equipe): .NET 8 · ASP.NET Core Web API · EF Core 8 + Pomelo · MySQL 8 · Clean Architecture
- **Front** (Claude): consome a API REST, que roda em `http://localhost:5262`

`back/` (01-Presentation · 02-Application · 03-Infrastructure · 04-Domain ·
05-Tests) · `front/` · `docs/ROADMAP.md` (o que falta fazer) · `docs/historico/`

## Regras de produto

Decisões de negócio já fechadas, não preferências.

- **Mestre enxerga quantidade, admin enxerga dinheiro.** O mestre nunca vê valor
  de contrato, mão de obra, salário, preço ou margem. Esconder na tela não
  basta: se vier no JSON, o DevTools mostra. Por isso **DTO separado por
  perfil** — nunca um DTO único com campo nulo, que falha aberto quando alguém
  adiciona um campo e esquece de anular.
- **Perfis:** `admin`, `engineer`, `foreman`, `safety_technician`,
  `warehouse_keeper`, `purchasing`. Cliente e empreiteiro **não têm login**
  (cliente acessa por link com token, `project_link`). Não há auto-cadastro: o
  admin cria o usuário com senha provisória.
- **Estoque:** material de consumo tem saldo **por obra** (não há depósito); EPI
  vai do depósito ao funcionário; ferramenta vai e **volta**. Saldo é sempre
  somado de `stock_movement`, em C#, nunca lido de coluna.
- **Compras:** o mestre solicita só quantidade; a administração cota, compra e
  registra preço e nota.
- **"Atrasada" não é status gravado.** É data prevista comparada com hoje, em
  C#. Nunca ler o `status` cru para falar de atraso.
- **O banco não valida tudo.** A migration sai só do comando, então não há
  CHECK, `DEFAULT 0.00`, seed nem usuário inicial — isso é resolvido em C#. O
  banco garante FK, unique, `NOT NULL`, os defaults e as colunas calculadas do
  `AppDbContext`. Se algo precisa existir no banco, declare no `AppDbContext`
  (`HasCheckConstraint`, `HasDefaultValue`, `HasData`) e deixe o comando gerar.

## Convenções

- **Idioma:** código, banco e valor gravado em **inglês** (`'pending'`, não
  `'pendente'`). Texto de tela em **inglês**; commit em **português**. Sigla brasileira não
  se traduz (`cno`, `crea_rt`, `ca`, `inss`). Glossário: obra = `Project` ·
  etapa = `Stage` · ficha de EPI = `PpeIssue` · empreitada = `Subcontract` ·
  setor = `Department` · função = `JobRole` · papel = `Role` · ponto =
  `TimeEntry` · fornecedor = `Supplier`.
- **Nomes:** banco `snake_case`, C# `PascalCase`, convertidos pelo
  `UseSnakeCaseNamingConvention()`. Renomear a propriedade renomeia a coluna.
- **Commits:** `tipo(escopo): descrição`, uma branch por feature, nunca direto
  na `main`.

## Back — C# e Clean Architecture

Vale para todo código novo. O código existente não é referência: o que foge
destas regras vai para a refatoração; o que é escrito agora já nasce certo.

| Camada | Mora aqui | Não mora |
|---|---|---|
| `04-Domain` | entidades, enums, regra de negócio da entidade | EF, ASP.NET, DTO |
| `02-Application` | DTOs, services, interfaces que a infra implementa | `AppDbContext`, SQL |
| `03-Infrastructure` | `AppDbContext`, configuração do EF, migrations | regra de negócio |
| `01-Presentation` | controllers finos (recebem, chamam, devolvem), `Program.cs` | regra, cálculo |

- **Regra de negócio no domínio** — método da entidade (`project.IsLate(today)`),
  nunca no DTO nem no controller.
- **Evitar classe `static`.** Se parece precisar, está no lugar errado: valor
  fixo é `enum`, regra é método da entidade, comportamento é serviço injetado.
  Exceção: métodos de extensão.
- **Um tipo por arquivo**, com o nome do tipo.
- **Valor fixo é `enum`**, nunca string solta. A conversão para texto no banco
  e no JSON é configurada uma vez (`AppDbContext`, `Program.cs`).
- **DTO só carrega dado** e é imutável. Entrada (`CreateProjectRequest`)
  separada da saída (`ProjectDto`).
- **"Hoje" entra por parâmetro** na regra, nunca `DateTime.Now` dentro dela.
- **Nome diz o que é.** Nada de letra solta, sigla ou abreviação: `project =>`,
  não `p =>`; `appDbContext`, não `db`; `HeaderCell`, não `Th`. Vale para
  parâmetro de lambda, variável, campo e componente. Exceção: `_` para o que
  não é usado.
- **Código que se explica sozinho.** Nome bom no lugar de comentário. Comentário
  só onde o código não diz o porquê — uma regra, uma decisão não óbvia, uma
  armadilha — ou onde quem mexer depois provavelmente travaria. Explicação
  didática vai na conversa, não no arquivo.
- Dependência pelo construtor · leitura com `AsNoTracking()` · `async` até o
  banco · erro de validação como `ValidationProblem` com o campo certo.
- **Hoje os controllers acessam o `AppDbContext` direto** — em Clean
  Architecture passaria por um service. Está na refatoração.

**Quando o Claude passa código do back:** ele orienta, não edita. Um passo por
vez, na ordem do fluxo (domínio → banco/JSON → DTO → controller): qual arquivo,
em que pasta, o código e o porquê de cada método. Só avança quando a pessoa
disser que **entendeu** — dúvida e crítica vêm antes do próximo passo.

## Ao trabalhar no front

- Ler `docs/ROADMAP.md` antes de começar.
- Conferir o contrato real em `back/01-Presentation/Controllers/` antes de
  assumir formato de resposta. Não inventar endpoint.
- Não commitar credencial, token ou connection string (ela fica em User
  Secrets, nunca no `appsettings.json`).
- O Claude não roda migration. Referência:
  `dotnet ef database update --project back/03-Infrastructure --startup-project back/01-Presentation`

## Skills do projeto

Ficam em `.claude/skills/` e valem para o Claude Code de todos.

- `/revisar-back`: confere o back contra as regras deste arquivo e aponta,
  sem corrigir. Rodar na própria fatia antes de abrir o PR.
- `/revisar-tela`: confere a tela contra o padrão do front (modelo: `projects/`
  e `stages/`) e o básico de acessibilidade.
- `depurar`: o método para caçar defeito (reproduzir, medir, corrigir a causa).
