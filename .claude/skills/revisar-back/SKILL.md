---
name: revisar-back
description: Revisar o back do NoPrumo (C#, .NET, EF Core) contra as regras do CLAUDE.md — camadas, permissão, DTO por perfil, nomes, classe static, um tipo por arquivo, enum, pastas, relógio. Só quando pedirem: "revisa meu back", "confere minha fatia", "está certo?", antes de abrir PR.
disable-model-invocation: true
---

# Revisar o back

Revisar e **apontar, nunca corrigir**. O back é o aprendizado da equipe: quem
corrige é a pessoa, item por item, e só pede ajuda no item que travar.

O `CLAUDE.md` do projeto é a fonte das regras. O que ele decidiu diferente
desta lista não é defeito.

## Como

1. Perguntar o que revisar, se não foi dito: a fatia inteira (os controllers,
   DTOs e requests dela) ou o que mudou (`git diff main...HEAD -- back`).
2. Ler cada arquivo inteiro. Pasta, nome do arquivo e namespace também são
   revisão.
3. Responder no formato abaixo, do mais grave para o menos grave, separando
   **Prioridade alta** (segurança, dinheiro exposto, bug) do **padrão do
   projeto**:

   ```text
   EmployeesController.cs:12 — [Authorize] sem policy: qualquer usuário logado altera salário.
     Correção: [Authorize(Policy = "manage_employees")].
   ```

4. Fechar com o que está bom, numa linha.

## O que conferir

**Segurança e dinheiro (prioridade alta)**
- Controller ou ação sem `[Authorize(Policy = "...")]`. `[Authorize]` sozinho
  deixa qualquer perfil entrar.
- Permissão conferida por valor solto (`c.Value == "admin"`), em vez de
  `User.HasClaim("permission", "...")`.
- Entidade devolvida direto na resposta (`Ok(employee)`,
  `CreatedAtAction(..., employee)`): vaza todos os campos.
- Valor em dinheiro (salário, preço, contrato, margem) para quem não tem
  `view_finance`. A regra é DTO separado por perfil, nunca um DTO com campo
  nulo. Na gravação, campo de dinheiro aplicado sem conferir a permissão.
- CPF/CNPJ gravado sem o `DocumentProcessor` (cifrado, hash e máscara).

**Bugs comuns**
- `Skip`/`Take` sem `OrderBy` antes (a página repete ou pula registros) e sem
  limite de `page`/`size` (`Math.Max(1, page)`, `Math.Clamp(size, 1, 100)`).
- Coluna com índice único gravada sem perguntar antes (`AnyAsync`): estoura 500.
- Chave estrangeira gravada sem conferir se o registro existe.
- Texto sem limite de tamanho (`[MaxLength]` ou validação): estoura na coluna.

**Camadas**
- Regra de negócio fora do domínio: no controller, no DTO ou na tela. Regra é
  método da entidade (`project.IsLate(today)`).
- DTO com lógica: `FromEntity`, cálculo, consulta. DTO só carrega dado.
- Controller acessando o `AppDbContext` direto **não** é defeito nesta fase:
  está na refatoração. A fatia 5 (obra e etapa) é o modelo com service e
  repositório.

**Tipos e pastas**
- Classe `static` que não seja de métodos de extensão.
- Mais de um tipo por arquivo, ou arquivo com nome diferente do tipo.
- DTO mutável (`{ get; set; }`): o padrão é `record` com `init`.
- Valor fixo como `string` (`"company"`, `"expired"`): vira `enum` no domínio,
  com conversão no `AppDbContext` (`SnakeCaseEnumConverter`).
- Pasta que não bate com o sufixo: `Dto` em `DTOs/`, `Request` em `Requests/`,
  `Response` em `Responses/`, `Result` em `Results/`, interface em
  `Interfaces/`, serviço em `Services/`. Namespace igual à pasta.

**Nomes**
- Letra solta, sigla ou abreviação: `e =>`, `c =>`, `db`, `_context`, `emp`,
  `docSettings`. Nome inteiro: `employee =>`, `appDbContext`.

**Comportamento**
- `DateTime.Now`/`UtcNow` dentro de regra ou controller. O "hoje" e o "agora"
  vêm do `TimeProvider` injetado (`timeProvider.Today()`, `timeProvider.Now()`).
- Leitura sem `AsNoTracking()`.
- Erro de validação que não volta como `ValidationProblem` com o campo certo
  (`BadRequest(new ProblemDetails { Detail = ... })` não diz o campo).
- `CancellationToken` recebido e não repassado.

**Comentários**
- Comentário que repete o código, conta histórico ("ajustado para…") ou ensina
  ("AsNoTracking: é só leitura…"). Didática vai na conversa.
- Armadilha sem comentário: o que quebra em silêncio se alguém "melhorar".
