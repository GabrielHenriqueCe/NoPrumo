# ROADMAP — MVP

Escopo do MVP. O que está fora está na seção final.

Legenda: `[x]` feito · `[ ]` a fazer · `[!]` decisão pendente

---

## ✅ Pronto

- [x] Modelo de dados — 37 tabelas (36 entidades + a junção `role_permission`)
- [x] 36 entidades EF em `back/04-Domain/Entities`, em inglês e no singular
- [x] Migration `InitialCreate` gerada **só pelo comando**, sem SQL escrito à mão
- [x] Credenciais fora do repositório (User Secrets)
- [x] Estrutura monorepo `back/` + `front/`

---

## 0. O que saiu do banco e virou código

A migration deixou de ter SQL manual. Com isso, três categorias de coisa que o
banco resolvia sozinho passaram a ser responsabilidade do C# — e nenhuma delas
está feita. Enquanto não estiverem, o banco sobe vazio e sem trava nenhuma.

- [ ] **Validação** — os 40 CHECK constraints (status válido, `amount > 0`,
      data fim ≥ data início, quantidade positiva) viram validação na entidade
      ou no service
- [ ] **Seed inicial** — papéis, permissões, tipos de NR/ASO, setores, funções,
      categorias e grupos de estoque
- [ ] **Primeiro usuário** — não existe `admin` no banco. Decidir como ele nasce:
      rotina de seed no start da API ou INSERT manual documentado no README.
      Sem isso, ninguém entra no sistema
- [ ] **`atrasada`** — calcular comparando a data prevista com hoje, em `project`
      e em `stage`
- [ ] **Saldo de estoque** — somar `stock_movement`, por obra e por depósito
- [ ] **Alerta de NR vencendo** — `employee_training.expiry_date` contra hoje
- [ ] **Fechamento por obra** — contrato − material − mão de obra, com margem

Os quatro últimos eram views no banco. Foram removidas de propósito; a conta
agora é feita em C#, uma vez só, no lugar que alimenta tela e relatório.

### [!] Qual fórmula de custo do dia vale?

Três fontes do projeto divergem, e o cálculo ainda não existe em nenhum lugar:

| Fonte | CLT | Horista | Diarista |
|---|---|---|---|
| protótipo (`js/equipes.js`) | `valor / 22`, sem encargo | `valor × 8,8` | `valor` |
| `Documentacao.pdf` §7 | `valor × 1,75 / 21` | `valor × 8` | `valor` |
| seed que existia no banco | +71,80% (SINAPI) | +115,60% (SINAPI) | 0% |

A regra registrada abaixo assume a terceira. Fechar isso antes de implementar.

---

## 1. Refino das entidades

O scaffold gerou classes cruas: setter público, sem construtor, sem validação.
Refinar **só onde há regra de negócio** — as tabelas de apoio (`Department`,
`JobRole`, `EmploymentRegime`, `StockCategory`, `StockGroup`) podem ficar como
estão.

- [ ] `bool?` → `bool` em `Supplier`, `Employee`, `StockItem`, `User`
      (o scaffold gerou nulável por causa do `DEFAULT TRUE`)
- [ ] `Project` — construtor, validação de datas, aditivo não pode zerar contrato
- [ ] `Employee` — validação de `pay_rate` e `additional_percentage`
- [ ] `TimeEntry` — cálculo do custo, snapshots obrigatórios
- [ ] `StockMovement` — quantidade sempre positiva, sinal vem do tipo
- [ ] `User` — nunca expor `password_hash`
- [ ] `PpeIssue` / `PpeIssueItem` — assinatura obrigatória para valer como comprovante
- [ ] `Subcontract` / `SubcontractMeasurement`

### Regras que não podem se perder no refino

**Custo da hora trabalhada** — nesta ordem, o adicional incide sobre o salário e
os encargos incidem sobre o total já com o adicional:

```
custo_hora = valor_hora × (1 + additional_percentage) × (1 + encargos)
```

Eletricista tem **30% de periculosidade** (Lei 12.740/2012), com natureza
salarial — reflete em 13º, férias, FGTS e INSS.

**Encargos** — `payroll_charge_rate`, congelado em 13/09/2026:
SINAPI Florianópolis/SC, ref. dez/2025, sem desoneração.
Horista 115,60% · Mensalista 71,80%.

**Estoque** — `stock_movement` é a única fonte de saldo. Toda entrada e saída
passa por lá. Saldo é sempre calculado, nunca guardado em coluna. Custo da obra
sai das **entradas** (material é comprado para a obra).

**Empreitada** — não passa pelo ponto e não gera encargo CLT. O custo vem de
`subcontract_measurement`.

**Obra fechada** — `closed_at` trava lançamento. `completion_date` é entrega
física, `closed_at` é fechamento contábil. São coisas diferentes: nota de
material ainda chega 30-60 dias depois da entrega.

---

## 2. Autenticação

- [ ] Hash de senha com **BCrypt.Net-Next, cost 12** — nunca SHA256
- [ ] Endpoint de login + JWT
- [ ] Rate limit no login (`AddRateLimiter` do .NET 8)
- [ ] Lockout por tentativas (`failed_attempts` e `locked_until` já existem)
- [ ] Primeiro acesso obriga troca de senha provisória
- [ ] Criar o usuário `admin` (ver seção 0 — hoje não existe nenhum no banco)

### Autorização

- [ ] Policies baseadas em **permissão**, não em nome de perfil
      — `[Authorize(Policy = "view_finance")]`, não `if (role == "admin")`
- [ ] Handler de recurso para "esta obra é minha?" (escopo via `user_project`)
- [ ] **404 em vez de 403** quando o mestre pede obra que não é dele
      — 403 confirma que o recurso existe e permite enumerar IDs
- [ ] DTOs separados por perfil — `ProjectResumoDto` (todos) + `ProjectFinanceiroDto` (admin)

### Link público do cliente

- [ ] Token gerado com `RandomNumberGenerator`, **nunca** sequencial
- [ ] Guardar **hash** do token no banco, igual senha
- [ ] Respeitar `expires_at` e `revoked_at`
- [ ] Cliente vê: andamento, etapas, datas. **Nunca**: custo, nome de funcionário, material

---

## 3. LGPD e dados pessoais

- [ ] CPF cifrado via Value Converter do EF (`document_encrypted`)
- [ ] Blind index para busca e unicidade (`document_hash`)
- [ ] Exibir só `document_masked` na tela
- [ ] Auditoria: campo sensível (CPF, salário, senha) **nunca** entra em
      `old_data` / `new_data` — só o nome do campo em `changed_fields`.
      Vale para INSERT, UPDATE e DELETE, sem exceção
- [ ] Política de retenção da auditoria (sugestão: 24 meses) + job de expurgo
- [ ] Anonimização em vez de exclusão (`anonymized_at`) — LGPD dá direito à
      eliminação, mas a lei trabalhista obriga a guardar o histórico. Anonimizar
      resolve os dois
- [ ] Documentar base legal: dado de funcionário é **execução de contrato +
      obrigação legal**, nunca consentimento

---

## 4. API

- [ ] **CORS** — sem isso o front não conversa com a API. Primeiro bloqueio prático
- [ ] Middleware global de exceção (`IExceptionHandler`) no lugar de try/catch por action
- [ ] Paginação nos endpoints de lista — `stock_movement` e `time_entry` crescem rápido
- [ ] Repositories e services **só para o que tem tela**. Não gerar 37 CRUDs
- [ ] Mover entidades para o Domain já foi feito; manter Domain sem referência a EF

---

## 5. Front

- [ ] Tela de login
- [ ] Primeiro acesso / troca de senha
- [ ] Layout base com menu por perfil
- [ ] Páginas vazias navegáveis (esqueleto antes do conteúdo)
- [ ] Gestão de usuários (admin cria, não existe auto-cadastro)
- [ ] Portal do cliente por link (sem login)

---

## 6. Documentação

- [ ] `README.md` com o passo a passo de setup — quem clona não adivinha os
      3 passos do banco + User Secrets
- [ ] Decidir o destino do `NoPrumo_MySQL.sql`: hoje a fonte da verdade é a
      migration. Se ficar, marcar como referência para ninguém rodar por engano

---

## [!] Decisões pendentes

| Assunto | Quem responde |
|---|---|
| Qual fórmula de custo do dia vale (ver seção 0) | equipe |
| FAP da empresa e regime tributário (Simples Anexo IV muda o encargo em ~5,8 p.p.) | contador |
| Periodicidade real das NRs — as normas mudam | técnico de segurança |
| Quantos mestres por obra (se for mais de um, escopo sai só de `user_project`) | equipe |
| Framework do front | equipe |

---

## Fora do MVP

Decisões conscientes, não esquecimento:

- **Máquinas e equipamentos** — arrasta manutenção, horímetro, combustível e
  alocação. A `Documentacao.pdf` do protótipo descreve uma aba de máquinas com
  esses campos; ela não existe no banco, e é de propósito
- **Anexos e fotos** — obra vive de foto de andamento e PDF de contrato, mas fica para V2
- **Diário de obra (RDO)** — padrão do setor, V2
- **Histórico de remuneração** — o custo histórico já está protegido pelos snapshots
  em `time_entry`, mas não há rastro de quando o salário mudou
- **Multi-tenant** — não existe `company_id`. Correto para uma construtora.
  Se virar SaaS, a migração é cara. Registrado de propósito
- **Ponto eletrônico legal** — o que existe é *apontamento de efetivo* para custo.
  Ponto legal exige Portaria 671/2021: registro pelo próprio trabalhador,
  inviolável, com comprovante. É outro projeto
