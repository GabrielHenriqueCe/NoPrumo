# NoPrumo

**Sistema de gestão de obras para construtoras de médio e grande porte.**

O NoPrumo centraliza informações operacionais de uma construtora em uma única aplicação: obras e etapas, equipes e funcionários, estoque, compras, treinamentos e segurança do trabalho. O projeto é desenvolvido como um monorepositório, com uma aplicação web em React e uma API REST em .NET.

<p align="center">
  <img alt=".NET 8" src="https://img.shields.io/badge/.NET-8-512BD4?logo=dotnet&logoColor=white">
  <img alt="React 19" src="https://img.shields.io/badge/React-19-61DAFB?logo=react&logoColor=black">
  <img alt="MySQL 8" src="https://img.shields.io/badge/MySQL-8-4479A1?logo=mysql&logoColor=white">
  <img alt="Status" src="https://img.shields.io/badge/status-em%20desenvolvimento-orange">
</p>

## O problema

Numa construtora, a informação de uma obra fica espalhada: o ponto está numa planilha, o material que chegou está na nota do almoxarife, o EPI entregue está numa ficha de papel e o custo só aparece no fim do mês, quando já não dá para corrigir. Quem está no canteiro não sabe o que foi comprado; quem está no escritório não sabe o que foi usado.

O NoPrumo junta tudo isso em torno da **obra**. Cada obra tem uma página com abas (resumo, etapas, equipe, material, EPI e financeiro), e cada pessoa entra no sistema vendo só o que o trabalho dela pede.

## Como o sistema pensa

- **A obra é a pasta.** O menu é da empresa (pessoas, materiais, comercial, configurações); tudo o que é de uma obra só fica dentro da página dela.
- **O mestre enxerga quantidade, a administração enxerga dinheiro.** O mestre de obras vê etapas, equipe e material, mas nunca valor de contrato, salário, preço ou margem. Isso não é só esconder na tela: a API devolve um DTO diferente para cada perfil, então o valor nem chega ao navegador.
- **Saldo é soma, nunca coluna.** O estoque de cada material numa obra é a soma das entradas e saídas registradas. Ninguém edita o saldo; um lançamento errado se corrige com outro lançamento.
- **O custo é o do dia.** O ponto guarda o valor-hora e os encargos do dia em que foi registrado, e a entrada de material guarda o preço da nota. Mudar o salário ou o preço de referência hoje não reescreve o custo das obras antigas.
- **"Atrasada" é calculado, não gravado.** Uma obra ou etapa está atrasada quando a data prevista já passou e ela não terminou. Isso é conta feita na hora, nunca um status salvo no banco.

## Perfis

Não há auto-cadastro: o administrador cria cada usuário com uma senha provisória, trocada no primeiro acesso.

| Perfil | O que faz no sistema |
|---|---|
| Administrador | Tudo, incluindo usuários, financeiro e configurações |
| Engenheiro | Cadastra obras, etapas e funcionários; vê o financeiro das obras |
| Mestre de obras | Acompanha obras e etapas e lança material; não vê valores |
| Técnico de segurança | Treinamentos de NR, vencimentos e fichas de EPI |
| Almoxarife | Entrada e saída de material e ferramentas |
| Compras | Aprova, cota e registra as compras pedidas pela obra |

O cliente da obra não tem login: ele acompanha o andamento por um link com token, gerado na página da obra.

## Funcionalidades

- **Gestão de obras:** cadastro de clientes, obras, etapas, equipes responsáveis e prazos.
- **Pessoas e equipes:** funcionários, departamentos, funções, regimes de contratação, composição de equipes e ponto.
- **Estoque:** catálogo de materiais, categorias, grupos, fornecedores, movimentações e saldo por obra.
- **Compras:** a obra pede a quantidade; compras aprova, registra preço e nota, e a compra dá entrada no estoque da obra.
- **Segurança do trabalho:** treinamentos e vencimentos, além da ficha de entrega de EPIs com assinatura.
- **Agenda:** compromissos e vencimentos de treinamento.
- **Custos da obra:** custo de material (pelas notas) e de mão de obra (pelo ponto, com encargos), comparados com o contrato e com o avanço das etapas, dia a dia.
- **Relatório e painel:** fechamento de todas as obras com custo e margem, e um painel inicial que mostra a cada perfil só o que é dele (obras e etapas atrasadas, estoque abaixo do mínimo, treinamentos vencendo, fichas de EPI sem assinatura).
- **Acesso por perfil e permissão:** autenticação com JWT e autorização baseada em permissões.
- **Portal do cliente:** andamento da obra por link, sem conta.

> O projeto está em desenvolvimento. A disponibilidade de cada fluxo depende do estado atual das telas e dos endpoints.

## Tecnologias

| Camada | Tecnologias |
|---|---|
| Frontend | React 19, Vite, React Router, Tailwind CSS 4 |
| Backend | C# / .NET 8, ASP.NET Core Web API |
| Persistência | Entity Framework Core 8, Pomelo, MySQL 8 |
| Autenticação | JWT e BCrypt |
| Documentação da API | Swagger / OpenAPI |
| Testes | xUnit |

## Organização do repositório

```text
NoPrumo/
├── back/
│   ├── 01-Presentation/      # API, controllers e configuração
│   ├── 02-Application/       # DTOs, serviços e contratos
│   ├── 03-Infrastructure/    # Entity Framework, banco e migrations
│   ├── 04-Domain/            # Entidades e regras do domínio
│   ├── 05-Tests/             # Testes automatizados
│   └── NoPrumo.slnx
├── front/
│   ├── app/
│   │   ├── data/             # Comunicação HTTP e gateways
│   │   └── view/             # Telas, rotas e componentes
│   ├── .env.example
│   └── package.json
├── docs/
│   ├── CADASTROS.md
│   └── ROADMAP.md
├── seed.sql
└── dev.bat
```

## Pré-requisitos

Antes de iniciar, instale:

- [.NET SDK 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js LTS e npm](https://nodejs.org/)
- [MySQL 8](https://dev.mysql.com/downloads/)

Os comandos abaixo usam PowerShell no Windows. Em outros sistemas, adapte os comandos de cópia de arquivos e configuração do ambiente.

## Configuração do ambiente

### 1. Clone o projeto

```powershell
git clone https://github.com/GabrielHenriqueCe/NoPrumo.git
cd NoPrumo
```

### 2. Crie o banco de dados

No MySQL, crie o banco utilizado pela aplicação:

```sql
CREATE DATABASE noprumodb
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_0900_ai_ci;
```

Configure um usuário MySQL com permissão para acessar esse banco. Você utilizará esses dados na string de conexão da próxima etapa.

### 3. Configure os segredos da API

As credenciais e chaves criptográficas devem ficar fora do repositório, usando **.NET User Secrets**. Execute no PowerShell:

```powershell
cd back/01-Presentation

# Gerar chaves aleatórias de 256 bits, codificadas em Base64
$jwtKey = [Convert]::ToBase64String([System.Security.Cryptography.RandomNumberGenerator]::GetBytes(32))
$encryptionKey = [Convert]::ToBase64String([System.Security.Cryptography.RandomNumberGenerator]::GetBytes(32))
$hmacKey = [Convert]::ToBase64String([System.Security.Cryptography.RandomNumberGenerator]::GetBytes(32))

# Ajuste usuário e senha para os dados locais do seu MySQL
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Port=3306;Database=noprumodb;User ID=noprumo_app;Password=SUBSTITUA_PELA_SENHA;"

# Configuração do JWT
dotnet user-secrets set "Jwt:Issuer" "NoPrumo"
dotnet user-secrets set "Jwt:Audience" "NoPrumo"
dotnet user-secrets set "Jwt:Key" $jwtKey

# Proteção de documentos
dotnet user-secrets set "Documents:EncryptionKey" $encryptionKey
dotnet user-secrets set "Documents:HmacKey" $hmacKey

cd ../..
```

Substitua `SUBSTITUA_PELA_SENHA` pela senha do usuário MySQL configurado. O projeto já possui um `UserSecretsId`; não é necessário executar `dotnet user-secrets init`.

### 4. Aplique as migrations

Na raiz do repositório, execute:

```powershell
dotnet tool install --global dotnet-ef --version 8.0.30
dotnet ef database update --project back/03-Infrastructure --startup-project back/01-Presentation
```

Se o comando `dotnet-ef` já estiver instalado, não é necessário instalá-lo novamente. As migrations criam a estrutura do banco; não use o `seed.sql` como substituto delas.

### 5. Configure e execute o frontend

```powershell
Copy-Item front/.env.example front/.env
cd front
npm install
npm run dev
```

O frontend estará disponível em [http://localhost:5173](http://localhost:5173). A variável `VITE_API_URL` no arquivo `front/.env` define o endereço da API; o valor de desenvolvimento é `http://localhost:5262/api`.

### 6. Execute a API

Em outro terminal, na raiz do repositório:

```powershell
dotnet run --project back/01-Presentation --launch-profile http
```

A API ficará disponível em [http://localhost:5262](http://localhost:5262) e a documentação Swagger em [http://localhost:5262/swagger](http://localhost:5262/swagger).

Ao iniciar, a API semeia permissões, perfis e categorias iniciais de estoque. Em um banco sem usuários, também cria o primeiro administrador:

| Campo | Valor inicial |
|---|---|
| Usuário | `admin` |
| Senha | `admin123` |

No primeiro acesso, a troca da senha é obrigatória. Essa credencial é apenas para a inicialização local: troque-a imediatamente e nunca a reutilize em ambientes compartilhados ou de produção.

### Execução rápida no Windows

Depois de configurar o MySQL, os User Secrets e aplicar as migrations, você também pode executar `dev.bat` na raiz do repositório. O script abre terminais separados para a API e para o frontend.

## Dados de demonstração (opcional)

O banco nasce vazio, só com o administrador. O `seed.sql` na raiz monta uma construtora inteira para testar e apresentar:

| O que entra | Quanto |
|---|---|
| Setores e funções | 8 setores, 16 funções |
| Funcionários | 24, nos três regimes (mensalista, horista e diarista) |
| Clientes e fornecedores | 4 clientes, 5 fornecedores |
| Estoque | 13 itens: material de consumo, EPI (um com CA vencido) e ferramentas |
| Segurança | 5 treinamentos de NR, com certificados vencidos, vencendo e em dia |
| Equipes | 3 equipes, alocadas nas obras |
| Obras | 4 obras com etapas: uma concluída, duas em andamento (uma com etapa atrasada) e uma em planejamento |
| Movimento | ponto diário e entregas de material desde o início de cada obra, mais um aditivo de contrato |

As datas são contadas a partir do dia em que o script roda. Por isso os gráficos, os vencimentos e os atrasos estão sempre atuais, não importa quando você rode.

**Ordem para usar:**

1. Crie o banco e aplique as migrations (passos 2 e 4).
2. Suba a API uma vez (passo 6) e feche. Ela cria as permissões, os perfis, o administrador, as categorias de estoque e os regimes de contratação com os encargos, e o script usa tudo isso.
3. Rode o script, na raiz do repositório. No `cmd`:

   ```bat
   "C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe" -u root -p --default-character-set=utf8mb4 noprumodb < seed.sql
   ```

   No PowerShell o `<` não existe; use o `source`:

   ```powershell
   & "C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe" -u root -p --default-character-set=utf8mb4 noprumodb -e "source seed.sql"
   ```

   Troque `root` pelo seu usuário do MySQL e `noprumodb` pelo nome do seu banco. O `--default-character-set=utf8mb4` mantém os acentos ("Kühn", "Itajaí") certos.

4. Confira a tabela que o script mostra no fim: pontos, mão de obra e material de cada obra. Se a primeira linha (`regimes_cadastrados`) mostrar 0, a API ainda não tinha subido nenhuma vez: suba a API e rode o script de novo.

**Pode rodar de novo quantas vezes quiser.** O cadastro (setor, funcionário, obra...) só entra se ainda não existir, procurado pelo nome ou pelo código, e nada do que você cadastrou é apagado ou alterado. Só o movimento marcado como demonstração (ponto, entradas e saídas de material e o aditivo `DEMO-001`) é apagado e gerado de novo, com as datas de hoje.

**Para testar os perfis:** o script não cria usuários, porque a senha precisa passar pelo BCrypt da API. Entre como `admin`, crie um usuário por perfil na tela Users e, para o mestre de obras, escolha o funcionário **Valdir Kühn** (supervisor do Residencial Aurora) ou **Osni Bortolini** (Galpão Itoupava e Bela Vista). O mestre só enxerga as obras que supervisiona.

## Comandos úteis

Na pasta `front/`:

```bash
npm run dev      # servidor de desenvolvimento
npm run build    # gera a versão de produção em dist/
npm run preview  # visualiza localmente o build
npm run lint     # executa o ESLint
```

Na raiz do repositório:

```powershell
dotnet test back/05-Tests/05-Tests.csproj
```

## Segurança e dados locais

- Não versione senhas, tokens, chaves de JWT, chaves de criptografia ou strings de conexão reais.
- Mantenha configurações sensíveis em User Secrets durante o desenvolvimento.
- O arquivo `front/.env` contém apenas configuração do frontend, como o endereço da API; ele não deve guardar credenciais.
- O `seed.sql` só acrescenta: não apaga nem altera o que você cadastrou, exceto o movimento de demonstração que ele mesmo gera (veja [Dados de demonstração](#dados-de-demonstração-opcional)). Mesmo assim, é para banco local de desenvolvimento, nunca de produção.

## Documentação do projeto

- [Roadmap do MVP](docs/ROADMAP.md)
- [Guia de cadastros](docs/CADASTROS.md)
- [Frontend: instruções específicas](front/README.md)

---

Desenvolvido como Projeto Integrador do Entra21 (Blumenau, 2026) por Adrian Gazzani, Eduardo Adão Locks, Everson Oliveira, Gabriel Henrique Cé e José Rodrigues.
