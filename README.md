# NoPrumo

**Sistema de gestão de obras para construtoras de médio e grande porte.**

O NoPrumo centraliza informações operacionais de uma construtora em uma única aplicação: obras e etapas, equipes e funcionários, estoque, compras, treinamentos e segurança do trabalho. O projeto é desenvolvido como um monorepositório, com uma aplicação web em React e uma API REST em .NET.

<p align="center">
  <img alt=".NET 8" src="https://img.shields.io/badge/.NET-8-512BD4?logo=dotnet&logoColor=white">
  <img alt="React 19" src="https://img.shields.io/badge/React-19-61DAFB?logo=react&logoColor=black">
  <img alt="MySQL 8" src="https://img.shields.io/badge/MySQL-8-4479A1?logo=mysql&logoColor=white">
  <img alt="Status" src="https://img.shields.io/badge/status-em%20desenvolvimento-orange">
</p>

## Funcionalidades

- **Gestão de obras:** cadastro de clientes, obras, etapas, equipes responsáveis e prazos.
- **Pessoas e equipes:** funcionários, departamentos, funções, regimes de contratação e composição de equipes.
- **Estoque:** catálogo de materiais, categorias, grupos, fornecedores e movimentações.
- **Compras:** solicitações de compra e acompanhamento do processo de aquisição.
- **Segurança do trabalho:** treinamentos e vencimentos, além do registro de entrega de EPIs.
- **Agenda:** compromissos e atividades relacionados à operação.
- **Acesso por perfil e permissão:** autenticação com JWT e autorização baseada em permissões.
- **Portal do cliente:** módulo previsto para compartilhar informações de andamento da obra por link, sem exigir uma conta tradicional.

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
- **Atenção:** `seed.sql` contém operações de limpeza de tabelas (`TRUNCATE`). Leia o script antes de executá-lo e utilize-o apenas no banco correto, preferencialmente em um ambiente local descartável.

## Documentação do projeto

- [Roadmap do MVP](docs/ROADMAP.md)
- [Guia de cadastros](docs/CADASTROS.md)
- [Frontend: instruções específicas](front/README.md)

---

Desenvolvido como projeto integrador, com foco em organizar os principais processos de uma construtora em uma aplicação única.
