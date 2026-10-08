---
name: revisar-tela
description: Revisar telas do front do NoPrumo (React, Tailwind) contra o padrão de tela do projeto e o básico de acessibilidade — cabeçalho, estados de carregando/erro/vazio, formulário, paginação, erros da API no campo, permissão. Usar quando pedirem "revisa minha tela", "confere o front", "está no padrão?", antes de abrir PR.
disable-model-invocation: true
---

# Revisar a tela

Revisar e **apontar**. Quem corrige é a pessoa dona da tela.

O modelo de tela é `front/app/view/screens/projects/` (lista e formulário) e
`front/app/view/screens/stages/`. O que fugir deles sem motivo é apontado.

## Como

1. Perguntar quais telas revisar, se não foi dito.
2. Rodar `npm run lint` dentro de `front/`: erro de lint entra como prioridade alta.
3. Ler a tela, o diálogo de formulário e o gateway dela.
4. Responder no formato `arquivo:linha — problema — o que fazer`, do mais
   grave para o menos grave, e fechar com o que está bom, numa linha.

## O que conferir

**Funciona (prioridade alta)**
- Props erradas em componente compartilhado. `Pagination` recebe `page`,
  `totalPages`, `total`, `onChange` e `noun` (o que está sendo contado).
- Erro da API lido no lugar errado. O cliente HTTP entrega `error.isValidation`
  e `error.fieldErrors` (já em camelCase, por campo); o resto vai para
  `error.message`. `error.payload` não existe.
- Erro escondido em `console.error`, sem nada na tela.
- Campo de dinheiro (salário, preço, contrato) aparecendo sem
  `can('view_finance')`, ou mandado à API por quem não tem a permissão.

**Padrão de tela**
- Cabeçalho: `label` "Master data", título `h1`, busca com rótulo (`sr-only`)
  e botão "New …" à direita.
- Busca com debounce (350 ms) e `AbortController` cancelando a requisição
  anterior. Sem isso, cada tecla é uma chamada.
- Tabela dentro do cartão com borda, células com `px-4 py-3`, cabeçalho com
  `HeaderCell`.
- Os três estados: carregando (esqueleto), erro (mensagem + "Try again") e
  vazio (com a busca, se houver).
- Ativar e desativar com botão e estado de carregando, nunca `confirm()`.
  Registro inativo mostra "Activate".

**Formulário**
- `<form>` com `p-6`: o `Dialog` não tem espaçamento interno.
- `TextField` e `SelectField` (rótulo ligado ao campo, erro com
  `aria-describedby`), nunca `<select>` ou `<label>` feitos à mão.
- Botão de salvar com `busy` enquanto envia; cancelar desabilitado no meio.
- Validação de regra fica na API; a tela só mostra o erro no campo.

**Acessibilidade básica**
- Todo campo com rótulo visível; placeholder não substitui rótulo.
- Botão é `<button>`, link é `<Link>`; nada clicável em `<div>`.
- Cor nunca é o único sinal: o `Badge` sempre tem a palavra.

**Código**
- Nomes inteiros: `employee =>`, não `e =>` ou `emp =>`. `HeaderCell`, não `Th`.
- `import React` sem uso (o projeto usa o JSX automático).
- Comentário só onde o código não diz o porquê.
- Texto de tela em inglês.
