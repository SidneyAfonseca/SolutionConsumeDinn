# SolutionConsumeDinn

Aplicação para envio do balancete contábil de concessionárias para a **API D-IN da AGCO**
(programa também conhecido como DFS/DEF), com administração de usuários e rastreabilidade
de quem disparou cada envio.

A implementação segue o `Documents/Guia_API_D-IN_v4.1.md` (Guia de implementação da API D-IN,
versão 4.1 — 19/04/2021), incluindo o plano de contas, os departamentos e as regras de
validação do item 6 do guia.

## Projetos

| Projeto | Tipo | Descrição |
|---|---|---|
| `ConsumeDinnApi` | ASP.NET Core Web API (net9.0) | Autenticação JWT, administração de usuários, validações do D-IN, persistência (MySQL + EF Core) e chamada à API da AGCO |
| `ConsumeDinnFront` | Blazor WebAssembly (net9.0) | Telas de login, administração de usuários, relatório de envios e consulta de departamentos/plano de contas |

Organização da API:

```
ConsumeDinnApi/
├── Controllers/    AuthController, UsersController, DInController, ConfigController
├── Domain/         Entities.cs (EF) e Models.cs (DTOs e contratos do D-IN)
├── Repository/     AppDbContext (mapeamento + seeds) e DInRepository (HttpClient da AGCO)
├── Service/        DInService (validações e registro de envio), TokenService, PasswordHasher
└── Migrations/     migrações EF Core
```

## Pré-requisitos

- .NET SDK 9 ou superior
- MySQL acessível (por padrão `localhost`, banco `ConsumeDinnDb`)
- `dotnet-ef` para trabalhar com migrações: `dotnet tool install --global dotnet-ef`

## Configuração

`ConsumeDinnApi/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ConsumeDinnDb;User=root;Password=root;"
  },
  "Jwt": {
    "Key": "troque-esta-chave-por-uma-secreta-de-no-minimo-32-caracteres",
    "Issuer": "ConsumeDinnApi",
    "Audience": "ConsumeDinnFront",
    "ExpireMinutes": 480
  },
  "DIn": {
    "Ambiente": "QA",
    "UrlQa": "https://apps-new-qa.agcoonline.com.br/dfs/save",
    "UrlPrd": "https://app.agcoonline.com.br/dfs/save"
  }
}
```

- `Jwt:Key` — chave de assinatura do token (mínimo 32 caracteres). **O valor versionado é um
  placeholder**: use um segredo real via variável de ambiente, `dotnet user-secrets` ou cofre
  de segredos antes de qualquer ambiente compartilhado.
- `DIn:Ambiente` — `QA` (também aceita `Homolog`/`HML`) usa `UrlQa`; qualquer outro valor usa
  `UrlPrd`. O guia prevê apenas os ambientes QA e PD, e recomenda permitir que o
  concessionário alterne o destino de forma autônoma.
- A API do D-IN não exige autenticação (item 3.2.2 do guia).

O front aponta para a API em `ConsumeDinnFront/Program.cs` (`https://localhost:7151`).

## Banco de dados

Criar/atualizar o schema:

```bash
dotnet ef database update --project ConsumeDinnApi
```

Migrações existentes:

| Migração | Conteúdo |
|---|---|
| `InitialCreateWithSeeds` | Tabelas `Users`, `DInReturns`, `Departments`, `AccountPlans` + seed dos 6 departamentos e das 284 contas do plano de contas AGCO |
| `AddUserRolesAndEnvioAutoria` | Perfil/situação do usuário (`Role`, `IsActive`, `CreatedAt`, username único), autoria do envio (`DInReturns.UserId` e `Username`) e o usuário administrador inicial |
| `SeedUsuarioNormal` | Usuário inicial de perfil UserNormal |

### Dados semeados

- **Departamentos** (item 7 do guia): 1101 Maquinas Novas, 1102 Maquinas Usadas, 1201 Peças,
  1301 Serviços, 1401 Administrativo, 1501 Outros Negócios.
- **Plano de contas** (item 8 do guia): 284 contas, de `1111` a `9613` — Ativo (1xxx), Passivo e
  Patrimônio Líquido (2xxx), Receitas (3xxx), Custos (4xxx), Despesas Operacionais (5xxx),
  Resultado Financeiro (6xxx), Outras Receitas e Despesas (7xxx), Impostos sobre o Lucro (8xxx)
  e Contas Gerenciais (9xxx).
- **Usuários iniciais**:

| Usuário | Senha | Perfil | Situação |
|---|---|---|---|
| `admin` | `admin123` | Admin | Ativo |
| `usuario` | `usuario123` | UserNormal | Ativo |

> As senhas são semeadas como hash PBKDF2/SHA256 com salt fixo (necessário para o seed da
> migração ser determinístico) e o hash está versionado no código. **Troque as duas senhas no
> primeiro acesso** — pela tela de administração de usuários ou por
> `PUT /api/users/{id}/password`.

## Executando

```bash
# API — https://localhost:7151 (Swagger em /swagger) e http://localhost:5018
dotnet run --project ConsumeDinnApi --launch-profile https

# Front — http://localhost:5021
dotnet run --project ConsumeDinnFront
```

No Swagger, use o botão **Authorize** e informe o token devolvido por `POST /api/auth/login`.

## Autenticação e perfis

A API usa **JWT Bearer**. O login devolve o token, o username, o perfil e o vencimento; o front
guarda o token no `localStorage` e o envia em todas as chamadas. Um token expirado é descartado
e a sessão cai para anônima.

Existem exatamente **dois perfis**:

| Ação | Admin | UserNormal |
|---|:---:|:---:|
| Autenticar (se estiver ativo) | ✔ | ✔ |
| Consultar departamentos e plano de contas | ✔ | ✔ |
| Enviar dados ao D-IN | ✔ | ✔ |
| Ver os próprios envios | ✔ | ✔ |
| Ver os envios de **todos** os usuários | ✔ | ✘ |
| Criar usuário | ✔ | ✘ |
| Trocar perfil de usuário | ✔ | ✘ |
| Habilitar/desabilitar usuário | ✔ | ✘ |
| Redefinir senha de usuário | ✔ | ✘ |

Regras de proteção aplicadas no backend:

- Usuário desabilitado não autentica (`403`, com mensagem orientando a procurar um administrador).
- Um administrador **não pode desabilitar o próprio usuário** nem remover o próprio perfil de Admin.
- O sistema sempre mantém **ao menos um administrador ativo**.
- Username é único (`409` em caso de duplicidade) e a senha tem no mínimo 6 caracteres.

## Endpoints

| Método | Rota | Acesso | Descrição |
|---|---|---|---|
| POST | `/api/auth/login` | anônimo | Autentica e devolve o token JWT |
| GET | `/api/auth/me` | autenticado | Dados do usuário do token |
| GET | `/api/users` | Admin | Lista usuários com perfil, situação, total de envios e último envio |
| POST | `/api/users` | Admin | Cria usuário (`username`, `password`, `role`) |
| PUT | `/api/users/{id}/status` | Admin | Habilita/desabilita (`{ "isActive": false }`) |
| PUT | `/api/users/{id}/role` | Admin | Troca o perfil (`1` = Admin, `2` = UserNormal) |
| PUT | `/api/users/{id}/password` | Admin | Redefine a senha |
| POST | `/api/din` | autenticado | Valida e envia o balancete ao D-IN, registrando o autor |
| GET | `/api/din/envios` | autenticado | Relatório de envios (Admin vê todos; UserNormal, só os próprios) |
| GET | `/api/config/departments` | autenticado | Departamentos |
| GET | `/api/config/accountplans` | autenticado | Plano de contas |

## Envio ao D-IN e relatório de envios

Antes de chamar a AGCO, o `DInService` aplica as validações do guia: payload não vazio, soma dos
saldos igual a zero, `dealerHeadOfficeCode`, `codeDFSPlan`, `dealerBranchCode` e `sectionCode`
obrigatórios, `periodMonth` entre 1 e 12 e `periodYear` com 4 dígitos.

Todo envio é gravado em `DInReturns` — **tanto o sucesso quanto a falha** — com:

| Coluna | Conteúdo |
|---|---|
| `CreatedAt` | data/hora do envio (UTC) |
| `Payload` | JSON enviado à AGCO |
| `Response` | retorno da AGCO ou a mensagem de erro |
| `Success` | resultado do envio |
| `UserId` | FK para `Users` — **id do usuário logado que fez o envio** |
| `Username` | login gravado junto ao envio, preservando a autoria mesmo se o usuário for removido |

A FK usa `ON DELETE SET NULL`: ao remover um usuário, o histórico continua existindo com o
`Username` preservado.

O relatório (`GET /api/din/envios` e a tela **Envios D-IN**) é filtrado pelo perfil do token:
**Admin recebe os envios de todos os usuários** e **UserNormal recebe apenas os próprios**. O
filtro é feito no backend a partir das claims do JWT, não no front.

## Telas do front

| Rota | Acesso | Descrição |
|---|---|---|
| `/login` | anônimo | Autenticação |
| `/` | anônimo | Home com atalhos conforme o perfil |
| `/envios` | autenticado | Relatório de envios, com o responsável por cada envio e detalhe do payload/retorno |
| `/departments` | autenticado | Departamentos do guia |
| `/accountplans` | autenticado | Plano de contas do guia |
| `/users` | **Admin** | Administração de usuários: criar, trocar perfil, redefinir senha e habilitar/desabilitar |

O item **Usuários** só aparece no menu para Admin, e o acesso direto pela URL por um UserNormal
cai na página "Acesso negado" (`AuthorizeRouteView`) além de receber `403` da API. Na tela de
administração, o botão **Desabilitar** fica bloqueado na própria linha do usuário logado.

## Migrações

```bash
# nova migração
dotnet ef migrations add NomeDaMigracao --project ConsumeDinnApi

# aplicar
dotnet ef database update --project ConsumeDinnApi

# reverter a última (ainda não aplicada)
dotnet ef migrations remove --project ConsumeDinnApi
```

## Referências

- `Documents/Guia_API_D-IN_v4.1.md` — guia de implementação (regras de negócio, contrato de
  dados, códigos de retorno, departamentos e plano de contas).
- `Documents/GuiadeimplementacaoAPID-INv4_1.pdf` — versão original em PDF.
- Swagger da AGCO: <https://api-qa.sa.agcocorp.com/swagger/?urls.primaryName=D-IN>
