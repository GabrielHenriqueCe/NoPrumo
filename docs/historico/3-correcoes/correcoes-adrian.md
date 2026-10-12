# Correções: fatia 2 (Setor · Função · Regime · Funcionário), Adrian

## Como usar este documento

1. Espere entrarem na `main` o PR da fatia 5 e o PR de correções do Gabriel: eles trazem as regras novas do `CLAUDE.md` e as skills `/revisar-back` e `/revisar-tela`.
2. Atualize a `main` e crie a sua branch: `git checkout main`, `git pull`, `git checkout -b fix/correcoes-adrian`.
3. Passe este arquivo para o seu Claude Code e peça para corrigir **um item por vez**, na ordem, seguindo o `CLAUDE.md`.
4. A cada item: `dotnet build back/01-Presentation` e, no front, `npm run lint`. Teste a tela.
5. Um commit por item ou grupo pequeno (`fix(back): ...`, `fix(front): ...`). No fim, abra o PR.
6. Antes do PR, rode `/revisar-back` e `/revisar-tela` no Claude Code: as skills do projeto conferem se as correções foram feitas e pegam o que este documento deixou passar.

A seção **Prioridade alta** tem bugs e falhas de segurança. O resto é padrão do projeto.

---

## Prioridade alta

### 1. Qualquer usuário logado acessa e altera funcionários, setores, funções e regimes
`EmployeesController.cs:12`, `DepartmentsController.cs:12`, `JobRolesController.cs:12`, `EmploymentRegimesController.cs:12`

Os quatro usam `[Authorize]` sem policy. Na prática, um mestre ou um almoxarife consegue criar funcionário e mudar salário pelo Postman. O resto do projeto usa permissão: `[Authorize(Policy = "manage_employees")]`.

### 2. A criação de funcionário devolve a entidade inteira, com salário
`EmployeesController.cs:104`

`CreatedAtAction(..., employee)` serializa a entidade `Employee`: vão `PayRate`, `AdditionalPercentage` e tudo o que a tabela tiver, para qualquer perfil. Fere a regra "mestre enxerga quantidade, admin enxerga dinheiro". Devolver o DTO certo para o perfil (`EmployeeDto` ou `EmployeeFinancialDto`), igual à listagem.

### 3. A conferência de permissão financeira está errada
`EmployeesController.cs:48`

`User.Claims.Any(c => c.Value == "view_finance" || c.Value == "admin")` olha o **valor** de qualquer claim, de qualquer tipo. Um usuário com username `admin` passa no teste, mesmo sem a permissão. O certo é `User.HasClaim("permission", "view_finance")`, como no `StockItemsController`.

### 4. Quem não pode ver salário apaga o salário ao editar
`EmployeesController.cs:117` e `EmployeeFormDialog.jsx:16, 54, 101`

- **No back:** o `Update` grava `PayRate` e `AdditionalPercentage` vindos do request, sem olhar a permissão.
- **No front:** o campo de salário aparece para todo mundo. Quem não tem `view_finance` recebe a lista sem `payRate`, o campo abre vazio, e o formulário manda `0`.
- **Correção:** o back só aplica os dois campos se `view_finance`. O front só mostra os campos com `can('view_finance')` e, sem a permissão, nem manda o campo. Ver como faz o `ProjectFormDialog.jsx` (`canSeeAmount`).

### 5. O CPF do funcionário não é cifrado
`EmployeesController.cs:97, 126`

O código só grava uma máscara feita à mão (`"***.***." + últimos 4`). O documento não vai cifrado para `DocumentEncrypted`, não gera `DocumentHash` (então não dá para detectar CPF repetido), e a máscara sai em formato errado. O projeto já tem o `DocumentProcessor` do Everson: usar `IsValid` e `Process`, igual ao `ClientsController`, com o erro no campo `document`.

### 6. A paginação não funciona em nenhuma das quatro telas
`DepartmentsScreen.jsx`, `EmployeesScreen.jsx`, `EmploymentRegimesScreen.jsx`, `JobRolesScreen.jsx`

`<Pagination current={page} total={totalPages} onPageChange={setPage} />` usa props que o componente não tem. O `Pagination.jsx` espera `page`, `totalPages`, `total` e `onChange`. Hoje a troca de página não chega à tela.

### 7. Os erros de validação da API nunca aparecem no formulário
`EmployeeFormDialog.jsx:69`, `DepartmentFormDialog.jsx:25`, `JobRoleFormDialog.jsx:53`, `EmploymentRegimeFormDialog.jsx:35`

O código lê `error.payload.errors`, mas o cliente HTTP entrega `error.fieldErrors` (já com o nome do campo em camelCase) e `error.isValidation`. Qualquer outro erro some calado. Ver o padrão em `ProjectFormDialog.jsx`: `fieldErrors` por campo e `formError` para o resto.

### 8. A paginação do back não tem ordem nem limite
`*Controller.cs:36/37` (os quatro)

- **Sem ordem:** o `Skip` vem sem `OrderBy` antes. O MySQL pode devolver em qualquer ordem, e a página 2 repete ou pula registros. Use `OrderBy(...)` e, para desempate, `ThenBy(... => ....Id)`.
- **Sem limite:** sem `page = Math.Max(1, page)` e `size = Math.Clamp(size, 1, 100)`, um `size=100000` traz a tabela inteira.

### 9. Nome repetido estoura com erro 500
`DepartmentsController`, `JobRolesController`, `EmploymentRegimesController`

O banco tem índice único em `department.name`, `job_role (name, department_id)` e `employment_regime.label`. Hoje o código não pergunta antes de gravar, e o banco recusa com 500. Conferir com `AnyAsync` e devolver `ValidationProblem` no campo (`name` ou `label`). Na função, conferir também se o setor existe no `Update`, não só no `Create`.

---

## Padrão do projeto (back)

10. **DTO imutável e um tipo por arquivo.** `DepartmentDtos.cs`, `EmployeeDtos.cs`, `EmploymentRegimeDtos.cs` e `JobRoleDtos.cs` têm várias classes cada, com `{ get; set; }`. O padrão é `record` com `init`, um arquivo por tipo, e os requests na pasta `Requests/` (ex.: `Requests/CreateDepartmentRequest.cs`). O `UpdateEmployeeRequest` herdar de `CreateEmployeeRequest` também sai: são dois contratos independentes.
11. **Erro de validação no campo certo.** `BadRequest(new ProblemDetails { Detail = "Name is required." })` não diz qual campo. Use `ModelState.AddModelError("name", "...")` + `ValidationProblem(ModelState)`, ou anotações no request (`[Required]`, `[MaxLength]`). Sem `[MaxLength]`, texto longo estoura na coluna com 500.
12. **Ativar e desativar como nas outras telas.** Hoje é `DELETE` para desativar e `Active` no corpo do `PUT` para reativar. O projeto usa `PATCH /{id}/activate` e `PATCH /{id}/deactivate`, e o `PUT` não mexe no estado ativo.
13. **`GET /{id}` não existe.** O `CreatedAtAction(nameof(Get), new { id })` aponta para a listagem. Crie o `GetById` e aponte para ele.
14. **Listagem de funcionário com `PagedResult`.** `EmployeesController` devolve um objeto anônimo e monta a lista com `(object)`. Devolva `PagedResult<EmployeeDto>`. O JOIN manual (duas consultas extras) não é necessário: `Employee` já tem `JobRole` e `EmploymentRegime` como navegação, então basta `.Include`.
15. **Leitura com `AsNoTracking()`** nas listagens.
16. **Namespace dos controllers.** `NoPrumo.Presentation.Controllers` não bate com a pasta. O projeto usa `NoPrumo.Controllers`.
17. **`DbSet` duplicado e no plural.** No `AppDbContext` existem `Department` e `Departments` para a mesma tabela, e `Employees`, `JobRoles` e `EmploymentRegimes` estão no plural. O resto está no singular (`Project`, `Client`, `Team`). Manter um só, no singular, e trocar nos controllers. Não gera migration.
18. **Coluna `active` × `deleted_at`.** Setor, função e regime ganharam uma coluna `active` nova. As outras tabelas desativam por `deleted_at`. Não mude agora, porque mexe em migration: leve para o grupo decidir.

## Padrão do projeto (front)

19. **Lint: 8 erros e 4 avisos.** `import React` sem uso nos 8 arquivos e `useEffect` sem a dependência da função de carregar. Rode `npm run lint`.
20. **Margem do formulário.** O `Dialog` não tem espaçamento interno, então o `<form>` precisa de `p-6` (ver `ProjectFormDialog.jsx`). Hoje os campos encostam na borda.
21. **Estrutura da tela.** As quatro telas fogem do padrão das outras: cabeçalho com `label` "Master data" e título, busca com rótulo, tabela dentro do cartão com borda, estados de carregando, erro (com "Try again") e vazio, e células com `px-4 py-3`. O modelo é `ProjectsScreen.jsx` ou `ClientsScreen.jsx`.
22. **Busca sem debounce e sem cancelar.** Cada tecla dispara uma requisição. Use o debounce de 350 ms e o `AbortController` das outras telas.
23. **Erros escondidos no console.** `console.error` sem mostrar nada na tela. O usuário precisa ver o erro.
24. **`confirm()` nativo** para desativar. As outras telas usam o botão direto, com estado de carregando.
25. **Reativar.** Funcionário inativo continua mostrando "Deactivate". Precisa do botão "Activate" (item 12).
26. **Selects feitos à mão** no `EmployeeFormDialog`. Use o `SelectField` (rótulo ligado ao campo e erro).

## Vale para todos

27. **Nome sem abreviação.** Nada de `e =>`, `d =>`, `j =>`, `r =>`, `emp`, `_context` (use `appDbContext`). Vale para lambda, variável e campo.
28. **Comentários mínimos.** Saem os comentários que explicam o óbvio ou contam o histórico ("Ajustado para DateOnly?", "Adicione o método remove", "Garantindo que convertemos explicitamente o valor booleano").
29. **Relógio.** `DateTime.UtcNow` no `DeletedAt` (`EmployeesController.cs`) grava 3 horas à frente do `created_at`, que o MySQL preenche no horário local. Na fatia 5 o padrão passou a ser horário local via `TimeProvider` (`timeProvider.Now()`). Confirmar com o grupo antes de trocar.
30. **Controllers acessando o `AppDbContext` direto:** não precisa mudar agora. Está na refatoração.
