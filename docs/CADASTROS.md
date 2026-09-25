# Cadastros — o que falta e quem faz

Guia para montar o Trello. Cada linha de tabela vira um card.

Regra de leitura: **cadastro** é a tela que cria a coisa. **Movimento** é a tela
que usa a coisa (lançar estoque, apontar ponto, pedir compra). Este documento
cobre só os cadastros — sem eles, nenhuma tela de movimento tem o que escolher.

---

## 1. Quem depende de quem

Só existem quatro níveis. Quem está num nível só precisa de quem está acima.

```
Nível 0   Setor   Regime de contratação   Categoria de estoque
          Tipo de treinamento   Cliente   Fornecedor
             │        │                │
Nível 1   Função   Equipe          Grupo de estoque      Funcionário
             │        │                │                     │
Nível 2   Item de estoque      Obra      Usuário ✅      Membro de equipe
                                │
Nível 3                   Etapa da obra        Treinamento do funcionário
```

**Respondendo a dúvida direto:** sim, cliente vem antes de obra — mas só para os
**dados**. Para o **código**, não: a tela de obra pode ser escrita hoje mesmo,
com o `<select>` de cliente vindo vazio da API. Ela só não dá para *usar* de
verdade antes de existir um cliente cadastrado.

Isso muda o planejamento inteiro: ninguém fica parado esperando ninguém. O que a
ordem de dependência define é **a ordem de testar**, não a ordem de programar.

---

## 2. Os cadastros, um por um

Campos da **primeira versão** — o mínimo para a tela existir. O resto entra
depois; a entidade já tem todos os campos, ninguém precisa de migration nova.

### Nível 0 — não dependem de nada

| Cadastro | Entidade | Campos da 1ª versão | Permissão |
|---|---|---|---|
| Setor | `Department` | `name` | `manage_employees` |
| Regime de contratação | `EmploymentRegime` | `label`, `unit`, `monthlyHours`, `description` | `manage_employees` |
| Categoria de estoque | `StockCategory` | `name`, `tracksProjectBalance`, `requiresReturn` | `manage_stock` |
| Tipo de treinamento | `TrainingType` | `code`, `name`, `validityMonths`, `minWorkloadHours`, `requiresInPerson` | `manage_safety` |
| Cliente | `Client` | `name`, `personType`, documento, `email`, `contactName`, `phone`, endereço | `manage_projects` |
| Fornecedor | `Supplier` | `name`, documento, `contactName`, `phone`, `email`, `city`, `state`, `active` | `manage_purchases` |

### Nível 1

| Cadastro | Entidade | Campos da 1ª versão | Depende de | Permissão |
|---|---|---|---|---|
| Função | `JobRole` | `name`, `departmentId` | Setor | `manage_employees` |
| Equipe | `Team` | `name`, `departmentId` | Setor | `manage_employees` |
| Grupo de estoque | `StockGroup` | `name`, `stockCategoryId` | Categoria | `manage_stock` |
| Funcionário | `Employee` | `registrationNumber`, `name`, `jobRoleId`, `employmentRegimeId`, `payRate`, `additionalPercentage`, `hireDate`, `phone`, documento, `active` | Função, Regime | `manage_employees` |

### Nível 2

| Cadastro | Entidade | Campos da 1ª versão | Depende de | Permissão |
|---|---|---|---|---|
| Item de estoque | `StockItem` | `stockGroupId`, `code`, `name`, `unit`, `minQuantity`, `referencePrice`, `ca`, `caExpiryDate`, `active` | Grupo | `manage_stock` |
| Obra | `Project` | `code`, `clientId`, `name`, `cno`, endereço, `contractAmount`, `status`, `supervisorId`, `technicalManager`, `creaRt`, `startDate`, `forecastDate` | Cliente, Funcionário | `manage_projects` |
| Membro de equipe | `EmployeeTeam` | `employeeId`, `startDate`, `endDate` — **dentro da tela de Equipe**, não é tela própria | Equipe, Funcionário | `manage_employees` |
| Usuário | `User` | — | — | ✅ pronto |

### Nível 3

| Cadastro | Entidade | Campos da 1ª versão | Depende de | Permissão |
|---|---|---|---|---|
| Etapa da obra | `Stage` | `projectId`, `name`, `sortOrder`, `teamId`, `supervisorId`, `plannedDate`, `percentage`, `status` | Obra, Equipe, Funcionário | `manage_projects` |
| Treinamento do funcionário | `EmployeeTraining` | `employeeId`, `trainingTypeId`, `issueDate`, `expiryDate`, `workloadHours`, `modality`, `instructor` | Funcionário, Tipo de treinamento | `manage_safety` |

### Não são cadastro — ficam para depois

`StockMovement`, `TimeEntry`, `PurchaseRequest`, `PpeIssue`, `Subcontract`,
`SubcontractMeasurement`, `AccountPayable`, `AccountReceivable`, `Payment`,
`Appointment`, `AuditLog`, `ProjectAmendment`, `ProjectLink`, `UserProject`,
`TeamProject`, `PayrollChargeRate`.

São movimento, ligação ou tabela de apoio do back. Nenhum deles bloqueia os 14
cadastros acima.

---

## 3. Divisão para 5 pessoas

### Primeiro: contar tela engana

"Setor" é um campo (`name`). "Obra" são quinze, com endereço, cliente,
supervisor e contrato. Quem divide por quantidade de tela entrega uma fatia de
quatro telas triviais e outra de duas telas monstruosas, e acha que empatou.

O peso abaixo é `campos + chaves estrangeiras + regra de negócio`. Escala
arbitrária, serve só para comparar uma tela com a outra:

| Tela | Peso | Por quê |
|---|---|---|
| Setor | 1 | um campo |
| Regime de contratação | 1,5 | 4 campos, na prática é seed (CLT · horista · diarista) |
| Função | 1,5 | 1 campo + 1 FK |
| Categoria de estoque | 1,5 | 1 campo + 2 booleanos |
| Grupo de estoque | 1,5 | 1 campo + 1 FK |
| Tipo de treinamento | 2 | 6 campos |
| Equipe | 3 | 1 campo + 1 FK + **aba de membros** (mestre-detalhe) |
| Item de estoque | 3 | 9 campos, CA e validade do CA |
| Fornecedor | 3 | 10 campos, documento |
| Treinamento do funcionário | 3 | 7 campos, 2 FKs, cálculo de vencimento |
| Cliente | 4 | 15 campos, endereço completo, PF e PJ com máscara diferente |
| Etapa | 4 | 8 campos, 3 FKs, percentual, ordenação |
| Funcionário | 4,5 | 10 campos, 2 FKs, CPF, `payRate` atrás de `view_finance` |
| Obra | 5 | 15 campos, endereço, 2 FKs, `contractAmount` atrás de `view_finance` |

Total **38,5** — dá ~7,7 por pessoa.

### Segundo: a tabelinha de apoio fica com quem a usa

É isso que resolve os dois problemas de uma vez. Setor e Função não são fatia de
ninguém: são o que o cadastro de Funcionário precisa para existir. Jogando as
duas para dentro da fatia de Funcionário, a espera some — vira dependência
interna, que a própria pessoa resolve na ordem que quiser.

Mesma coisa com Categoria → Grupo → Item: é uma cadeia de três, mas é uma cadeia
fechada dentro de uma pessoa só.

### A divisão

| # | Fatia | Telas | Peso | Espera dado de |
|---|---|---|---|---|
| 1 | **Parceiros** | Cliente · Fornecedor | 7 | ninguém |
| 2 | **Funcionário** | Setor · Função · Regime · Funcionário | 8,5 | ninguém |
| 3 | **Estoque** | Categoria · Grupo · Item | 6 | ninguém |
| 4 | **Equipe e segurança** | Equipe (+membros) · Tipo de treinamento · Treinamento | 8 | fatia 2 |
| 5 | **Obras** | Obra · Etapa | 9 | fatias 1 e 2 |

Faixa de 6 a 9 em vez de 4 telas contra 2. A fatia 2 tem quatro telas, mas três
delas são formulário de um campo — é a fatia mais **larga** e não a mais pesada.

**A fatia 3 é a mais leve de propósito.** Cai bem para quem está menos seguro em
React: a cadeia é curta, não depende de ninguém e as três telas são quase a mesma
tela três vezes — é onde o padrão gruda mais rápido.

**A fatia 5 é a mais pesada de propósito.** Obra é onde mora quase toda regra de
produto do sistema: o mestre não pode ver `contractAmount`, `atrasada` é
calculado e não gravado, `closedAt` trava lançamento. É a fatia de quem já
conhece o código — e quem fez a tela de usuários também leva o **commit de
preparação** da seção 4, que é repetitivo mas é o que destrava as outras quatro
pessoas.

Se quem fez o usuário preferir não carregar preparação + Obras, a troca natural é
com a fatia 3: ele fica com Estoque e o pessoal da 3 pega Obras.

### Ninguém espera código, só dado

As fatias 4 e 5 esperam **dado**, nunca código. A tela de Obra se escreve hoje,
com o `<select>` de cliente vindo vazio da API. Só o teste ponta a ponta é que
depende de existir um cliente cadastrado — e para isso basta uma linha no banco.

---

## 4. Como não dar conflito de merge

Esta é a parte que decide se o merge vai ser tranquilo ou um inferno.

### O problema

Toda tela nova mexe nos **mesmos 4 arquivos**:

| Arquivo | O que a tela acrescenta |
|---|---|
| `front/app/container.js` | uma linha registrando o gateway |
| `front/app/view/routes.jsx` | um `<Route>` |
| `front/app/view/shell/AppShell.jsx` | um item no `MENU` |
| `back/01-Presentation/Program.cs` | a policy da permissão |

Cinco pessoas × 4 arquivos = **4 conflitos garantidos em todo merge**, sempre nas
mesmas linhas. E conflito em `routes.jsx` é do tipo chato: o Git não tem como
saber se as duas rotas devem ficar ou se uma substitui a outra — quem resolve
decide na mão, e é aí que some código.

### A solução: um commit de preparação, antes de todo mundo começar

Uma pessoa faz **um commit só**, que abre o espaço das 14 telas de uma vez:

1. os 14 gateways em `front/app/data/gateways/`, no mesmo formato do
   `userGateway.js` — com o contrato do endpoint escrito no comentário do topo
2. as 14 linhas no `container.js`
3. as 14 rotas no `routes.jsx`, apontando para telas vazias
4. os 14 itens no `MENU` do `AppShell.jsx`, agrupados
5. uma pasta `front/app/view/screens/<módulo>/` por tela, com um placeholder que
   só escreve o nome da tela
6. as 8 policies no `Program.cs` — as 8 permissões **já existem no seed**,
   falta só registrar

Depois desse commit, **cada pessoa edita só a pasta do módulo dela**. Arquivo
novo de gente diferente nunca colide. Merge limpo, sempre.

Isso já está previsto no `ROADMAP.md` §5: *"Páginas vazias navegáveis (esqueleto
antes do conteúdo)"*. É exatamente para isso que serve.

### No back

Cada pessoa cria o `XController.cs` e os DTOs dela — arquivos novos, sem
conflito. O único compartilhado é o `Program.cs`, e ele já foi resolvido no
passo 6.

### Menu depois da preparação

Os 14 entram **todos debaixo de Cadastros**, junto com Usuários. Nenhum grupo
novo — é a regra do `ROADMAP.md` §5: *cadastro novo entra debaixo de Cadastros,
não solto na raiz do menu*.

```
Cadastros (Master data)
    Users
    Departments · Job roles · Employment regimes · Employees · Teams
    Clients · Suppliers
    Stock categories · Stock groups · Stock items
    Projects · Stages
    Training types · Trainings
```

São 15 itens numa lista só, e é para ser assim. O menu é agrupado por **tipo de
trabalho**, não por assunto: Cadastros é a gaveta de onde se registra a coisa,
uma vez e pronto. Os grupos Estoque, Obras e Segurança nascem depois, com as
telas de **movimento** — lançar entrada, apontar ponto, pedir compra, emitir
ficha de EPI. Essas sim são o dia a dia, e é nelas que a pessoa volta todo dia.

Separar por assunto agora deixaria "Estoque > Categorias" no mesmo nível de
"Estoque > Lançar saída", que é uma tela que alguém abre uma vez por ano e uma
que abre dez vezes por dia.

Dentro do grupo, a ordem acima é por domínio: quem procura "Job roles" acha ao
lado de "Departments", não no meio de "Clients".

Grupo cujos itens a pessoa não pode ver **some inteiro** — o `AppShell` já faz
isso, é só declarar a permissão em cada item.

---

## 5. O que cada card precisa dizer

Modelo de card do Trello:

```
Cadastro de <nome>

Front   front/app/view/screens/<módulo>/
        - <X>Screen.jsx       lista + busca + paginação
        - <X>FormDialog.jsx   criar e editar

Back    back/01-Presentation/Controllers/<X>Controller.cs
        back/02-Application/DTOs/<X>Dto.cs, Create<X>Request.cs, Update<X>Request.cs
        GET /<x>?page&size&search · POST /<x> · PUT /<x>/{id}
        [Authorize(Policy = "<permissão>")]

Seed    se o cadastro tem valor padrão (NR-35, categorias de estoque, setores),
        acrescentar no DatabaseSeeder — a tela deixa acrescentar mais

Pronto quando   cria, edita, lista e pagina contra a API de verdade, sem mock
```

---

## 6. Cuidados que valem para todo cadastro

- **Não inventar endpoint.** Conferir o contrato real em
  `back/01-Presentation/Controllers/` antes de assumir formato de resposta.
- **Nada é apagado, é desativado.** `active` / `deletedAt`, igual usuário — o
  histórico tem que continuar de pé.
- **Paginação no mesmo formato** do `PagedResult<T>` de usuários:
  `{ items, page, size, total, totalPages }`.
- **Dinheiro é permissão separada.** `Employee.payRate` e
  `Project.contractAmount` exigem `view_finance`. Não basta esconder o campo na
  tela: se o valor vier no JSON, o DevTools mostra. DTO separado por perfil.
- **CPF/CNPJ:** a tela **envia o documento puro** (campo `document`) e **mostra**
  o mascarado que a API devolve (`documentMasked`, ex. `***.456.789-**`). Quem
  gera as três colunas é o back: `documentMasked` (máscara), `documentHash`
  (é o que o índice único usa para barrar documento repetido) e
  `documentEncrypted` (o número de verdade, cifrado). A tela nunca manda nada
  em `documentMasked` — senão o número inteiro vai parar em texto puro no banco.
  Cliente, Fornecedor e Funcionário usam o mesmo código para isso.
- **Texto de tela em inglês** (decisão de 17/09). Código, banco e valor gravado
  em inglês também — `status = 'pending'`, nunca `'pendente'`.
- **Categoria de estoque não é campo livre.** `tracksProjectBalance` e
  `requiresReturn` são o que define os três fluxos do sistema: material de
  consumo (saldo por obra), EPI (depósito → funcionário) e ferramenta (vai e
  volta). Essas três nascem do seed; a tela só deixa acrescentar.
- **`atrasada` não existe em `status`.** É calculado comparando a data prevista
  com hoje, no back. Nenhuma tela grava isso.

---

## 7. Ordem sugerida das sprints

Cada pessoa trabalha dentro da fatia dela; a sprint só diz **em que ordem** ela
faz as telas que já são suas, para que o dado de que a fatia seguinte precisa já
exista.

| Sprint | Fatia 1 | Fatia 2 | Fatia 3 | Fatia 4 | Fatia 5 |
|---|---|---|---|---|---|
| 0 | — | — | — | — | commit de preparação |
| 1 | Cliente | Setor · Função · Regime | Categoria · Grupo | Tipo de treinamento | Obra |
| 2 | Fornecedor | Funcionário | Item de estoque | Equipe (+membros) | Etapa |
| 3 | folga / movimento | folga / movimento | folga / movimento | Treinamento | ajuste fino |

A fatia 2 entrega as três tabelinhas já na sprint 1 — é o que deixa a fatia 4
montar Equipe e a fatia 5 escolher supervisor na sprint 2. Quem sobrar com folga
na sprint 3 puxa card do backlog de movimento (estoque, ponto, compras).

Ao fim da sprint 3 dá para cadastrar uma obra de verdade, com cliente, mestre,
equipe e etapas — que é o que abre as telas de movimento.
