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
| `AddEmpresaEDePara` | Cadastro da empresa (`Dealers`, `DealerBranches`) e o De/Para (`AccountMappings`, `DepartmentMappings`) |

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
| Consultar empresa e De/Para | ✔ | ✔ |
| Validar e enviar balancete ao D-IN | ✔ | ✔ |
| Ver os próprios envios | ✔ | ✔ |
| Ver os envios de **todos** os usuários | ✔ | ✘ |
| Cadastrar/editar empresa e filiais | ✔ | ✘ |
| Manter o De/Para de contas e centros de custo | ✔ | ✘ |
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
| GET | `/api/dealers` | autenticado | Empresas com filiais e totais de De/Para |
| POST | `/api/dealers` | Admin | Cadastra a empresa (razão social, CNPJ matriz, departamento padrão) |
| PUT | `/api/dealers/{id}` | Admin | Atualiza a empresa |
| POST | `/api/dealers/{id}/branches` | Admin | Adiciona filial |
| PUT | `/api/dealers/branches/{id}/status` | Admin | Ativa/inativa filial |
| DELETE | `/api/dealers/branches/{id}` | Admin | Remove filial |
| GET | `/api/dealers/{id}/account-mappings` | autenticado | De/Para de contas |
| POST | `/api/dealers/{id}/account-mappings` | Admin | Cria/atualiza o De/Para de uma conta |
| DELETE | `/api/dealers/account-mappings/{id}` | Admin | Remove o De/Para da conta |
| GET | `/api/dealers/{id}/department-mappings` | autenticado | De/Para de centros de custo |
| POST | `/api/dealers/{id}/department-mappings` | Admin | Cria/atualiza o De/Para de um centro de custo |
| DELETE | `/api/dealers/department-mappings/{id}` | Admin | Remove o De/Para do centro de custo |
| POST | `/api/din/balancete/validar` | autenticado | Aplica o De/Para e devolve payload + problemas, sem enviar |
| POST | `/api/din/balancete/enviar` | autenticado | Valida e envia o balancete, registrando o autor |
| POST | `/api/din` | autenticado | Envia um payload D-IN já montado (uso técnico/testes) |
| GET | `/api/din/envios` | autenticado | Relatório de envios (Admin vê todos; UserNormal, só os próprios) |
| GET | `/api/config/departments` | autenticado | Departamentos |
| GET | `/api/config/accountplans` | autenticado | Plano de contas |

## Cadastro da empresa

O envio depende do cadastro em **Empresa** (`/empresa`):

- **Razão social** e **CNPJ da matriz** — o CNPJ vai como `dealerHeadOfficeCode` e precisa ser o
  cadastrado junto à AGCO. O guia lembra que há grupos cujo CNPJ 0001-XX não é a operação ligada
  à montadora; nesses casos, cadastre como matriz o CNPJ que a AGCO reconhece.
- **Filiais** — todos os CNPJs ativos ligados à matriz. O guia obriga o envio de todos na mesma
  requisição e proíbe o envio individual: se faltar alguma filial ativa no balancete, o envio é
  bloqueado. Sem filiais cadastradas, o envio usa apenas a matriz com
  `HeadOffice = BranchCode`, como o guia prevê para operações de uma única unidade.
- **Departamento padrão** — `sectionCode` usado nas linhas sem centro de custo, já que o D-IN
  exige o departamento em todo item.

## De/Para

Duas relações, mantidas por empresa em **De/Para** (`/depara`):

| Relação | Da origem | Para o D-IN | Tabela |
|---|---|---|---|
| Contas | conta contábil da concessionária (ex.: `1.1.01.001`) | `codeDFSPlan` (ex.: `1111`) | `AccountMappings` |
| Centros de custo | centro de custo da origem (ex.: `ADM`) | `sectionCode` (ex.: `1401`) | `DepartmentMappings` |

Regras aplicadas (`BalanceteService`):

- O destino é validado contra o plano de contas e os departamentos semeados do guia — não é
  possível mapear para um código inexistente.
- **Conta sem De/Para bloqueia o envio**, com a lista das contas e o número da linha de origem,
  como o guia exige ("erro amigável que facilite a identificação do erro na origem"). O mesmo
  vale para centro de custo sem De/Para.
- Códigos e CNPJs são comparados sem máscara, espaços ou caixa: `12.345.678/0001-90` e
  `12345678000190` são equivalentes.
- Linhas que caem na mesma chave (`codeDFSPlan` + filial + período + `sectionCode`) são
  **somadas em um único item**, garantindo a regra 13 do guia (uma PK não pode repetir no mesmo
  envio). A mesma conta pode ir para vários departamentos, como o guia permite.
- Uma relação por conta de origem por empresa (índice único em `DealerId` + `SourceCode`).

## Envio do balancete

Tela **Enviar Balancete** (`/envio`): escolha a empresa e o período, cole o balancete (ou
selecione um arquivo CSV/TXT), clique em **Validar** e, se estiver tudo certo, em **Enviar ao
D-IN**. O botão de envio fica bloqueado enquanto houver erro de validação.

Formato de cada linha — separador `;`, `,` ou tabulação, cabeçalho opcional:

```
CNPJ da filial ; conta de origem ; centro de custo ; saldo
12.345.678/0001-90;1.1.01.001;ADM;1.000,00
12.345.678/0001-90;2.1.01.001;ADM;-1.000,00
```

O saldo aceita vírgula ou ponto como decimal. CNPJ em branco assume a matriz; centro de custo em
branco usa o departamento padrão da empresa.

A validação (`POST /api/din/balancete/validar`) devolve o payload gerado, os totais (linhas de
origem, itens, filiais, `totalValue`, tamanho do payload) e a lista de problemas:

| Verificação | Origem no guia |
|---|---|
| Conta ou centro de custo sem De/Para | item 3.1 — "impeça a tentativa de envio de contas sem a relação De/Para" |
| Filial ativa ausente no balancete | item 3.1 / regra 12 — envio individual proibido |
| CNPJ que não é filial ativa da empresa | item 3.1 |
| PK repetida no mesmo envio | regra 13 — resolvida agrupando e somando |
| Um único período por envio | regra 14 — o período é do envio, não da linha |
| `periodMonth` entre 1 e 12 e `periodYear` com 4 dígitos | regras 6 e 8 |
| Soma das contas de referencial (1 a 8) igual a zero | regra 15 — as 9xxx gerenciais ficam fora |
| Payload até 4 MB | regra 16 |

## Envio ao D-IN e relatório de envios

Antes de chamar a AGCO, o `DInService` aplica as validações do guia: payload não vazio, soma das
contas de referencial (códigos iniciados de 1 a 8) igual a zero, `dealerHeadOfficeCode`,
`codeDFSPlan`, `dealerBranchCode` e `sectionCode` obrigatórios, `periodMonth` entre 1 e 12 e
`periodYear` com 4 dígitos.

> **Divergência observada no ambiente de QA da AGCO:** o exemplo JSON do guia usa
> `"periodMonth": "06"`, mas o endpoint de QA recusa o mês com zero à esquerda —
> *"O campo periodMonth precisa estar entre 1 a 12. Encontrado: 06"*. Com `"6"` a validação
> passa. Por isso o `DInService` remove o zero à esquerda antes de enviar; as telas continuam
> exibindo o mês com dois dígitos.

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
| `/envio` | autenticado | Envio do balancete: De/Para, validação com totais e envio ao D-IN |
| `/envios` | autenticado | Relatório de envios, com o responsável por cada envio e detalhe do payload/retorno |
| `/departments` | autenticado | Departamentos do guia |
| `/accountplans` | autenticado | Plano de contas do guia |
| `/empresa` | **Admin** | Cadastro da empresa, filiais e departamento padrão |
| `/depara` | **Admin** | De/Para de contas e de centros de custo |
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
