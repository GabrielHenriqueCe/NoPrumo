# Correções: fatia 0, login/usuários e fatia 5 (Obra · Etapa), Gabriel

## Como usar este documento

Mesma rotina dos outros: branch `fix/correcoes-gabriel` a partir da `main` atualizada (depois do merge da fatia 5), um item por vez, build e lint a cada item, um PR no fim.

---

## Prioridade alta

### 1. A paginação escreve "user/users" em todas as telas
`front/app/view/ui/Pagination.jsx:13, 30`

Na tela de clientes aparece "1 user". O componente é compartilhado, então precisa receber o nome do que está contando (ex.: `noun="project"`) e cada tela passa o seu. É o componente da fatia 0. Avisar o grupo antes, porque as telas deles vão ganhar a prop nova.

### 2. `CLAUDE.md` desatualizado
`CLAUDE.md:64`

Ainda diz "texto de tela em português". A decisão de 17/09 é tela em inglês.

---

## Padrão do projeto (back)

3. **`PasswordGenerator` é classe `static`.** Vira serviço injetado (`PasswordGenerator`), registrado no `Program.cs` e recebido pelo `UsersController`.
4. **`DatabaseSeeder` é classe `static`.** Mesmo caso: vira classe registrada e chamada pelo `Program.cs`.
5. **`ValidationProblemFrom` repetido.** Existe igual no `ProjectsController` e no `StagesController`. Quando aparecer o terceiro controller com service, extrair para uma base (`ApiControllerBase`).
6. **Aviso 20601 do EF.** `Project.Status` e `Stage.Status` têm default no banco e o primeiro valor do enum é o "vazio" do C#. Hoje o resultado é o mesmo; se incomodar, configurar o sentinel.

## Padrão do projeto (front)

7. **Comentários longos na tela de obras.** `ProjectsScreen.jsx` e `ProjectFormDialog.jsx` ainda têm os blocos de explicação de antes da regra de comentários mínimos.

## Vale para todos

8. **Nome sem abreviação.**
   - **Nos meus controllers:** `AuthController`, `UsersController` e `RolesController` usam `db`, `tokens` e lambdas `u =>` e `r =>`.
   - **No `AppDbContext`:** o scaffold usa `e =>`, `d =>` e `p =>` nas configurações.
9. **Relógio.** O `AuthController.cs:35` grava `LastLoginAt` com `DateTime.UtcNow`. Trocar por `timeProvider.Now()`, o mesmo padrão que eu defini na fatia 5.

## Para levar ao grupo (decisão, não correção)

10. **`active_key` e código reaproveitável.** Sete tabelas usam o índice `(campo, active_key)`, que deixa reaproveitar o código de um registro desativado. Na obra eu decidi que o código é único para sempre (só no C#). Vale discutir se o mesmo vale para as outras.
11. **Relógio único.** Propor ao grupo: tudo em horário local via `TimeProvider`, igual ao `CURRENT_TIMESTAMP` do MySQL. Os documentos de cada um já citam isso como "confirmar com o grupo".
12. **`active` × `deleted_at`.** Setor, função e regime (fatia 2) ganharam coluna `active`. As outras tabelas desativam por `deleted_at`. Escolher um padrão.
13. **Organização das pastas.** O sufixo diz a pasta (`DTOs/`, `Requests/`, `Responses/`, `Results/`, `Interfaces/`, `Services/`, `Settings/`), sem subpasta por assunto. Os documentos dos colegas já pedem a separação dos requests.
14. **Visual das telas.** Propor um guia curto de padrão de tela, com antes e depois numa tela da fatia 5, para cada um aplicar na sua.
15. **Fórmula do custo do dia.** Continua pendente e trava código (ver CTX).
