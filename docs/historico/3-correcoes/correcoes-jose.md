# Correções: fatia 3 (Categoria · Grupo · Item de estoque), José

## Como usar este documento

1. Espere entrarem na `main` o PR da fatia 5 e o PR de correções do Gabriel: eles trazem as regras novas do `CLAUDE.md` e as skills `/revisar-back` e `/revisar-tela`.
2. Atualize a `main` e crie a sua branch: `git checkout main`, `git pull`, `git checkout -b fix/correcoes-jose`.
3. Passe este arquivo para o seu Claude Code e peça para corrigir **um item por vez**, na ordem, seguindo o `CLAUDE.md`.
4. A cada item: `dotnet build back/01-Presentation` e, no front, `npm run lint`. Teste a tela.
5. Um commit por item ou grupo pequeno. No fim, abra o PR.
6. Antes do PR, rode `/revisar-back` e `/revisar-tela` no Claude Code: as skills do projeto conferem se as correções foram feitas e pegam o que este documento deixou passar.

A fatia 3 está bem feita:
- tem permissão separada para ver e para mexer (`view_stock` × `manage_stock`);
- o preço fica num DTO separado por perfil;
- a validação cai no campo certo;
- a paginação tem ordem e limite.

O que falta é o padrão novo do `CLAUDE.md`.

---

## Prioridade alta

### 1. Erro de lint no item de estoque
`StockItemFormDialog.jsx:1`

`useMemo` é importado e não é usado. Rode `npm run lint`.

---

## Padrão do projeto (back)

2. **Um tipo por arquivo.** `StockItemDto.cs`, `StockGroupDto.cs` e `StockCategoryDto.cs` têm DTO e requests juntos. Separar: o DTO em `DTOs/`, e cada request em `Requests/` com o nome do tipo (ex.: `Requests/CreateStockItemRequest.cs`). O `StockItemWithPriceDto` vai para `DTOs/StockItemWithPriceDto.cs`.
3. **DTO só carrega dado.** O `FromEntity` dentro dos DTOs é comportamento. A conversão vai para um método privado no controller (ou no service, quando houver).
4. **`StockItemWithPriceDto` repete todos os campos do `StockItemDto`.** Opcional: dá para herdar e só acrescentar o `ReferencePrice`, como o `ProjectFinanceDto` faz na fatia 5 (`record` + `[JsonDerivedType]`). Assim um campo novo no item não precisa ser lembrado em dois lugares.

## Vale para todos

5. **Nome sem abreviação.** Trocar `db` por `appDbContext`, e as lambdas `c =>`, `g =>` e `i =>` por `category =>`, `group =>` e `item =>`. Inclui os parâmetros dos `FromEntity` (`StockItem i`, `StockGroup g`).
6. **Comentários mínimos.** Comentário só onde o código não diz o porquê. Saem os didáticos, como `// AsNoTracking: é só leitura, o EF não precisa guardar estado.` (`StockCategoriesController.cs:32`) e `// o mínimo para abrir a tela`. Ficam os que explicam uma regra, como o do preço no `StockItemsController`.
7. **Controllers acessando o `AppDbContext` direto:** não precisa mudar agora. Está na refatoração.
