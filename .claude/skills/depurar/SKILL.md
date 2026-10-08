---
name: depurar
description: Usar ANTES de caçar a causa de um defeito no NoPrumo — "não funciona", "quebrou", "dá erro 500", tela em branco, lista vazia, número que não bate, comportamento que some quando se olha.
---

# Depurar

Quatro fases, e a ordem é o método. Pular direto para a correção conserta um
sintoma vizinho, e o defeito volta de outra forma.

## 1. Reproduzir

Achar o gatilho que dispara o defeito em segundos: uma chamada no Swagger
(`http://localhost:5262/swagger`), um clique numa tela, um comando.

- O que não se reproduz por código (layout, cor, "ficou estranho") é da pessoa
  conferir na tela. Pedir para ela olhar, não adivinhar.

## 2. Medir, nunca adivinhar

Antes de propor causa, olhar o estado no ponto exato.

- **API:** o corpo da resposta no Swagger ou na aba Rede do navegador
  (status, `errors` por campo, `detail`). Erro 500 traz a exceção no terminal
  da API.
- **Front:** o console do navegador e a requisição que saiu (URL, parâmetros,
  corpo).
- **Banco:** o que está gravado de fato, consultando a tabela (só leitura).
- **Ler o caminho, não o resultado.** Lista vazia faz o laço nunca rodar, e
  "não deu erro" não quer dizer "funcionou".

## 3. Corrigir a causa

- Consertar a causa, não o sintoma.
- **No `back/`, o Claude orienta e não edita** (regra do `CLAUDE.md`): passa o
  arquivo, o trecho e o porquê, e a pessoa aplica.
- Nunca mexer em migration nem rodar SQL que altere o banco.

## 4. Conferir e varrer

- `dotnet build back/01-Presentation`, `npm run lint` em `front/`, e repetir o
  gatilho da fase 1.
- Renomeou ou moveu algo? Procurar o nome velho no resto do código e nos
  documentos.

## Armadilhas que já custaram tempo aqui

| Sintoma | Causa |
|---|---|
| `[Range]` estoura com "is not a valid value for Decimal" | limite em texto lido no idioma da máquina (pt-BR usa vírgula): `ParseLimitsInInvariantCulture = true` |
| tudo compila, mas a API dá 500 na primeira chamada | service ou repositório sem registro no `Program.cs` |
| a paginação repete registros | `Skip` sem `OrderBy` antes |
| a data aparece um dia antes | `new Date('2026-10-02')` no front é meia-noite UTC, ainda dia 1 no Brasil |
| `git add back` não adiciona nada | o terminal está dentro de `back\`: voltar para a raiz do repositório |
| build falha sem motivo no Windows | a API está rodando e trava a `.dll`: parar e rodar de novo |
