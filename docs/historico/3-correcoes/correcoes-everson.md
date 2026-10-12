# Correções: fatia 1 (Cliente · Fornecedor), Everson

## Como usar este documento

1. Espere entrarem na `main` o PR da fatia 5 e o PR de correções do Gabriel: eles trazem as regras novas do `CLAUDE.md` e as skills `/revisar-back` e `/revisar-tela`.
2. Atualize a `main` e crie a sua branch: `git checkout main`, `git pull`, `git checkout -b fix/correcoes-everson`.
3. Passe este arquivo para o seu Claude Code e peça para corrigir **um item por vez**, na ordem, seguindo o `CLAUDE.md`.
4. A cada item: `dotnet build back/01-Presentation` e, no front, `npm run lint`. Teste a tela.
5. Um commit por item ou grupo pequeno. No fim, abra o PR.
6. Antes do PR, rode `/revisar-back` e `/revisar-tela` no Claude Code: as skills do projeto conferem se as correções foram feitas e pegam o que este documento deixou passar.

A fatia 1 está bem feita: tem validação no campo certo, documento cifrado, paginação com ordem e limite, e `AsNoTracking`. O que falta é segurança das chaves e o padrão novo do `CLAUDE.md`.

---

## Prioridade alta

### 1. A chave de criptografia é gerada com um gerador que não é seguro
`README.md:18, 22`

`Get-Random` não é gerador criptográfico. Trocar o comando por um que use `RandomNumberGenerator`, por exemplo:

```powershell
[Convert]::ToBase64String([System.Security.Cryptography.RandomNumberGenerator]::GetBytes(32))
```

### 2. A API sobe com chave de tamanho errado
`Program.cs`, bloco das chaves de documento

O código lê as chaves em base64, mas não confere se elas têm 32 bytes. Uma chave curta só estoura na primeira gravação de CPF. Validar o tamanho ao subir e falhar com mensagem clara.

### 3. CNPJ alfanumérico é recusado
`DocumentProcessor.cs`

O processador apaga tudo que não é dígito antes de validar, então o CNPJ com letras do formato novo da Receita é recusado. Confirmar a data em que o formato entra em vigor e aceitar letras na validação e no hash.

---

## Padrão do projeto (back)

4. **`DocumentProcessor` é classe `static`.** O `CLAUDE.md` pede para evitar: comportamento vira serviço injetado. Transformar em classe normal (`DocumentProcessor`), registrada no `Program.cs` e recebida pelo construtor, com o `DocumentSettings` também injetado. Assim os controllers não precisam passar as chaves a cada chamada.
5. **Um tipo por arquivo.** `ClientDtos.cs` e `SupplierDtos.cs` têm o DTO e os dois requests juntos. Separar: `DTOs/ClientDto.cs`, `Requests/CreateClientRequest.cs`, `Requests/UpdateClientRequest.cs` (idem fornecedor). O `ProcessedDocument`, que está no mesmo arquivo do `DocumentProcessor`, também ganha arquivo próprio.
6. **DTO só carrega dado.** O `FromEntity` dentro do `ClientDto` e do `SupplierDto` é comportamento. A conversão vai para um método privado no controller (ou no service, quando houver).
7. **`PersonType` como texto solto.** `"company"` e `"individual"` são valores fixos, então viram `enum PersonType` no domínio, com a conversão para texto configurada no `AppDbContext`. Ver como a fatia 5 fez com `ProjectStatus` (`SnakeCaseEnumConverter`). Não gera migration.
8. **`CreatedAt` e `UpdatedAt` preenchidos à mão.** O banco já preenche os dois (`CURRENT_TIMESTAMP` e `ValueGeneratedOnAddOrUpdate`). Hoje o C# sobrescreve com UTC, 3 horas à frente do horário que o banco usa. Pode tirar as linhas.
9. **Descrição do Swagger.** `Program.cs:115` diz "pequenas empresas". O projeto é para construtoras de médio e grande porte.

## Vale para todos

10. **Nome sem abreviação.** Trocar `db` por `appDbContext`, `docSettings` por `documentSettings`, `processedDoc` por `processedDocument`, `pType` por `personType`, `encKeyB64` por `encryptionKeyBase64`, e as lambdas `c =>` e `s =>` por `client =>` e `supplier =>`.
11. **Comentários mínimos.** Revisar se algum comentário só repete o que o código diz.
12. **Relógio.** O `DeletedAt` é gravado com `DateTime.UtcNow`, 3 horas à frente do `created_at`. Na fatia 5 o padrão passou a ser horário local via `TimeProvider` (`timeProvider.Now()`). Confirmar com o grupo antes de trocar.
13. **Controllers acessando o `AppDbContext` direto:** não precisa mudar agora. Está na refatoração.
14. **Opcional:** `ClientsController` e `SuppliersController` são quase cópias um do outro. Quando houver service, dá para compartilhar a validação de endereço e documento.
