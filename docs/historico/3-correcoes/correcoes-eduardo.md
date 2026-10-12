# Correções: fatia 4 (Tipo de treinamento · Equipe · Treinamento), Eduardo

## Como usar este documento

1. Espere entrarem na `main` o PR da fatia 5 e o PR de correções do Gabriel: eles trazem as regras novas do `CLAUDE.md` e as skills `/revisar-back` e `/revisar-tela`.
2. Atualize a `main` e crie a sua branch: `git checkout main`, `git pull`, `git checkout -b fix/correcoes-eduardo`.
3. Passe este arquivo para o seu Claude Code e peça para corrigir **um item por vez**, na ordem, seguindo o `CLAUDE.md`.
4. A cada item: `dotnet build back/01-Presentation` e, no front, `npm run lint`. Teste a tela.
5. Um commit por item ou grupo pequeno. No fim, abra o PR.
6. Antes do PR, rode `/revisar-back` e `/revisar-tela` no Claude Code: as skills do projeto conferem se as correções foram feitas e pegam o que este documento deixou passar.

A fatia 4 está bem feita:
- cada controller tem a sua permissão;
- a paginação tem ordem e desempate;
- os membros da equipe guardam histórico de entrada e saída;
- a validação cai no campo certo.

O que falta é levar a regra do vencimento para o domínio e o padrão novo do `CLAUDE.md`.

---

## Prioridade alta

### 1. O "hoje" do vencimento vem de `DateTime.Now` dentro do controller
`EmployeeTrainingsController.cs:295`

`private static DateOnly Today() => DateOnly.FromDateTime(DateTime.Now);` fere a regra "hoje entra por parâmetro" e não dá para testar. Na fatia 5, o "hoje" vem do `TimeProvider` injetado, com a extensão `timeProvider.Today()` (`02-Application/Extensions/TimeProviderExtensions.cs`). Receber o `TimeProvider` no construtor e usar a extensão. O `TimeProvider` já está registrado no `Program.cs`.

### 2. A regra de status do treinamento está numa classe `static` dentro do arquivo do DTO
`EmployeeTrainingDto.cs:52` (`TrainingStatus`) e `:72` (`TrainingModality`)

- **Onde a regra deve ficar:** "vencido, a vencer ou válido" é regra do treinamento. Ela vai para o domínio, como método da entidade, por exemplo `employeeTraining.StatusOn(today)`, igual ao `project.IsLate(today)` da fatia 5.
- **Valores fixos viram `enum`:** os de `TrainingStatus` e `TrainingModality` são fixos, então viram `enum` no domínio (`04-Domain/Enums/`). A conversão para texto fica no `AppDbContext` (`SnakeCaseEnumConverter`) e no JSON (já configurado no `Program.cs`).
- **O que muda no DTO:** ele passa a carregar `Status` e `Modality` como enum, não `string`.

---

## Padrão do projeto (back)

3. **Um tipo por arquivo.** `EmployeeTrainingDto.cs` tem o DTO e duas classes estáticas. `TeamDto.cs` tem o DTO e dois requests. Separar: DTOs em `DTOs/`, requests em `Requests/`, enums em `04-Domain/Enums/`.
4. **DTO só carrega dado.** Os `FromEntity` dentro dos DTOs são comportamento, e o de treinamento ainda calcula o status. A conversão vai para um método privado no controller (ou no service), e o status sai do método de domínio do item 2.
5. **`TeamsController` com 371 linhas.** Opcional nesta rodada: a gestão de membros (entrar, sair, histórico) tem regra suficiente para virar um service, como a fatia 5 fez com obra e etapa. Se preferir não mexer agora, deixe anotado para a refatoração.

## Padrão do projeto (front)

6. **Dois arquivos `format.js` parecidos.** `teams/format.js` e `employeeTrainings/format.js` têm o mesmo `formatDate`. Manter um só, num lugar compartilhado, e importar nos dois.

## Vale para todos

7. **Nome sem abreviação.** Trocar `db` por `appDbContext`, e as lambdas `t =>`, `m =>`, `d =>` e `e =>` por nomes inteiros (`team =>`, `membership =>`, `training =>`, `department =>`, `employee =>`). Inclui o parâmetro do `FromEntity` (`EmployeeTraining t`) e o `ToDto` do `TeamsController`.
8. **Comentários mínimos.** As telas de equipe e de treinamento têm muitos blocos de comentário (36 em `teams/`, 18 em `employeeTrainings/`). Ficam só os que explicam uma regra ou uma armadilha. Saem os que descrevem o que o código já mostra.
9. **Relógio.** `TeamsController.cs:183` grava `DeletedAt` com `DateTime.UtcNow`, 3 horas à frente do `created_at`. Na fatia 5 o padrão passou a ser horário local via `TimeProvider` (`timeProvider.Now()`). Confirmar com o grupo antes de trocar.
10. **Controllers acessando o `AppDbContext` direto:** não precisa mudar agora. Está na refatoração.
