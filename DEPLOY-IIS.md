# Deploy da solução no IIS

Este guia descreve o deploy da solução no IIS com:

- `ConsumeDinnFront`: frontend Blazor WebAssembly publicado como arquivos estáticos.
- `ConsumeDinnApi`: backend ASP.NET Core em .NET 9 executado pelo IIS através do ASP.NET Core Module.
- MySQL como banco de dados da API.

Os exemplos abaixo usam estes endereços:

- Frontend: `https://app.seudominio.com`
- API: `https://api.seudominio.com`
- Pasta de publicação: `C:\Sites\ConsumeDinnFront` e `C:\Sites\ConsumeDinnApi`

Substitua os domínios e caminhos pelos valores do ambiente real.

## 1. Pré-requisitos no servidor

1. Use um Windows Server compatível com o .NET 9.
2. Instale o IIS em **Server Manager > Add Roles and Features > Web Server (IIS)**.
3. Instale o **ASP.NET Core Hosting Bundle para .NET 9** no servidor:
   [Download do .NET 9](https://dotnet.microsoft.com/download/dotnet/9.0).
4. Reinicie o servidor após a instalação do Hosting Bundle.
5. Instale ou disponibilize um servidor MySQL acessível pelo servidor IIS.
6. Libere no firewall apenas as portas necessárias, normalmente `443` (HTTPS) e, se necessário, `80` para redirecionamento para HTTPS.
7. No IIS, confira se o recurso **URL Rewrite** está instalado. Ele é recomendado para o fallback de rotas do Blazor WebAssembly.

Valide a instalação do runtime no PowerShell:

```powershell
dotnet --info
dotnet --list-runtimes
```

O servidor deve mostrar o runtime `Microsoft.AspNetCore.App 9.x`.

## 2. Preparar o banco de dados

Crie o banco e um usuário próprio para a aplicação. Não use `root` em produção.

Exemplo no MySQL:

```sql
CREATE DATABASE ConsumeDinnDb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE USER 'consume_dinn_app'@'%' IDENTIFIED BY 'SENHA_FORTE_AQUI';
GRANT ALL PRIVILEGES ON ConsumeDinnDb.* TO 'consume_dinn_app'@'%';
FLUSH PRIVILEGES;
```

A string de conexão usada pela API deve apontar para o servidor MySQL de produção, por exemplo:

```text
Server=mysql-prod;Port=3306;Database=ConsumeDinnDb;User=consume_dinn_app;Password=SENHA_FORTE_AQUI;
```

A aplicação contém migrations do Entity Framework Core. Aplique-as no banco antes do primeiro uso. Se o ambiente de desenvolvimento tiver o SDK e as ferramentas configuradas, execute na pasta do projeto da API:

```powershell
dotnet ef database update --project .\ConsumeDinnApi\ConsumeDinnApi.csproj
```

Alternativamente, gere um script SQL para revisão e execução controlada pelo DBA:

```powershell
dotnet ef migrations script --project .\ConsumeDinnApi\ConsumeDinnApi.csproj --output .\database\ConsumeDinn.sql
```

> Execute migrations em produção com uma conta controlada e faça backup do banco antes da alteração.

## 3. Configurar a API para produção

A API lê `ConnectionStrings:DefaultConnection`, `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience` e as configurações de `DIn`.

Não publique a chave JWT de exemplo que está no `appsettings.json`. Gere uma chave aleatória com pelo menos 32 caracteres, preferencialmente com mais de 64 caracteres.

Crie ou ajuste `ConsumeDinnApi\appsettings.Production.json` sem adicionar esse arquivo ao controle de versão com segredos reais:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=mysql-prod;Port=3306;Database=ConsumeDinnDb;User=consume_dinn_app;Password=SENHA_FORTE_AQUI;"
  },
  "Jwt": {
    "Key": "CHAVE-ALEATORIA-FORTE-E-EXCLUSIVA-DE-PRODUCAO",
    "Issuer": "ConsumeDinnApi",
    "Audience": "ConsumeDinnFront",
    "ExpireMinutes": 480
  },
  "DIn": {
    "Ambiente": "PRD",
    "UrlQa": "https://apps-new-qa.agcoonline.com.br/dfs/save",
    "UrlPrd": "https://app.agcoonline.com.br/dfs/save"
  }
}
```

Defina o ambiente da aplicação como `Production`. Isso pode ser feito nas variáveis de ambiente do site no IIS:

```text
ASPNETCORE_ENVIRONMENT=Production
```

Depois de alterar essa variável, recicle o application pool da API.

Uma opção mais segura é configurar os valores no IIS como variáveis de ambiente da aplicação. Em ASP.NET Core, use `__` para separar níveis:

```text
ConnectionStrings__DefaultConnection
Jwt__Key
Jwt__Issuer
Jwt__Audience
Jwt__ExpireMinutes
DIn__Ambiente
DIn__UrlQa
DIn__UrlPrd
```

No IIS, essas variáveis podem ser definidas em **Configuration Editor > system.webServer/aspNetCore > environmentVariables**, ou durante a automação do servidor. Restrinja as permissões de leitura da pasta e dos arquivos de configuração.

## 4. Ajustar a URL da API no frontend

O `ConsumeDinnFront\Program.cs` atualmente usa `https://localhost:7151` como `BaseAddress`. Antes de publicar, altere para a URL pública da API:

```csharp
builder.Services.AddScoped(sp => new HttpClient(new BearerTokenHandler(sp.GetRequiredService<TokenStore>()))
{
    BaseAddress = new Uri("https://api.seudominio.com")
});
```

Recompile o frontend depois dessa alteração. O Blazor WebAssembly não usa a URL da API do `appsettings.json` automaticamente; portanto, essa URL fica incorporada nos arquivos publicados.

> Se frontend e API forem publicados no mesmo domínio, uma alternativa é usar um reverse proxy e configurar uma rota como `/api`. Nesse caso, ajuste o `BaseAddress` e a configuração do IIS de acordo com essa arquitetura.

## 5. Configurar CORS na API

Se frontend e API estiverem em domínios diferentes, a origem do frontend deve ser permitida pela API. A configuração atual usa `AllowAnyOrigin`, que não é recomendada para produção.

Substitua a política no `ConsumeDinnApi\Program.cs` por uma origem específica:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins("https://app.seudominio.com")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

E altere a chamada correspondente:

```csharp
app.UseCors("Production");
```

Publique novamente a API após essa alteração. Não use `AllowAnyOrigin` junto com credenciais.

## 6. Publicar a API

Execute os comandos na raiz da solução, em uma máquina com o SDK .NET 9:

```powershell
Remove-Item .\publish\api -Recurse -Force -ErrorAction SilentlyContinue

dotnet restore .\ConsumeDinnApi\ConsumeDinnApi.csproj

dotnet publish .\ConsumeDinnApi\ConsumeDinnApi.csproj `
  --configuration Release `
  --output .\publish\api `
  --no-restore
```

Copie o conteúdo de `publish\api` para:

```text
C:\Sites\ConsumeDinnApi
```

A pasta publicada deve conter o `.dll` da API e o `web.config` gerado pelo publish. Não aponte o IIS para a pasta do código-fonte.

## 7. Criar o site da API no IIS

1. Abra o **IIS Manager**.
2. Crie a pasta `C:\Sites\ConsumeDinnApi` e conceda ao grupo `IIS_IUSRS` permissão de leitura e execução.
3. Em **Application Pools**, crie um pool chamado `ConsumeDinnApi`.
4. Configure o pool com:
   - **.NET CLR Version**: `No Managed Code`.
   - **Managed pipeline mode**: `Integrated`.
   - **Identity**: `ApplicationPoolIdentity`.
5. Em **Sites**, escolha **Add Website**.
6. Configure:
   - **Site name**: `ConsumeDinnApi`.
   - **Physical path**: `C:\Sites\ConsumeDinnApi`.
   - **Binding**: `https`.
   - **Host name**: `api.seudominio.com`.
   - Certificado TLS válido para o domínio.
7. Se o DNS ainda não estiver configurado, crie um registro `A` ou `CNAME` apontando `api.seudominio.com` para o servidor.
8. Inicie o site e o application pool.

O `web.config` publicado normalmente contém o handler `AspNetCoreModuleV2` e aponta para o DLL da API. Não remova esse arquivo. Se ele não for gerado, verifique a instalação do Hosting Bundle antes de criar um manualmente.

Teste a API localmente no servidor:

```powershell
Invoke-WebRequest https://api.seudominio.com/swagger/index.html
```

O Swagger está condicionado ao ambiente de desenvolvimento no código atual. Para produção, prefira testar endpoints funcionais, como o endpoint de login, e não habilite Swagger publicamente sem autenticação.

## 8. Publicar o frontend Blazor WebAssembly

Execute:

```powershell
Remove-Item .\publish\front -Recurse -Force -ErrorAction SilentlyContinue

dotnet restore .\ConsumeDinnFront\ConsumeDinnFront.csproj

dotnet publish .\ConsumeDinnFront\ConsumeDinnFront.csproj `
  --configuration Release `
  --output .\publish\front `
  --no-restore
```

Em um projeto Blazor WebAssembly standalone, os arquivos que devem ser publicados no IIS ficam normalmente em:

```text
publish\front\wwwroot
```

Copie o conteúdo de `wwwroot` para:

```text
C:\Sites\ConsumeDinnFront
```

Confirme se essa pasta contém `index.html`, `_framework` e os arquivos estáticos da aplicação.

## 9. Configurar fallback de rotas do Blazor

Ao acessar diretamente uma rota como `/envios` ou atualizar a página, o IIS precisa entregar `index.html` em vez de procurar um arquivo físico.

Na pasta `C:\Sites\ConsumeDinnFront`, crie um arquivo `web.config` com o seguinte conteúdo, caso o publish não tenha criado um:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <staticContent>
      <mimeMap fileExtension=".wasm" mimeType="application/wasm" />
    </staticContent>
    <rewrite>
      <rules>
        <rule name="Blazor WebAssembly Fallback" stopProcessing="true">
          <match url=".*" />
          <conditions logicalGrouping="MatchAll">
            <add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true" />
            <add input="{REQUEST_FILENAME}" matchType="IsDirectory" negate="true" />
          </conditions>
          <action type="Rewrite" url="index.html" />
        </rule>
      </rules>
    </rewrite>
  </system.webServer>
</configuration>
```

O recurso URL Rewrite precisa estar instalado no IIS para essa regra funcionar.

## 10. Criar o site do frontend no IIS

1. Conceda ao grupo `IIS_IUSRS` permissão de leitura e execução em `C:\Sites\ConsumeDinnFront`.
2. Em **Sites**, escolha **Add Website**.
3. Configure:
   - **Site name**: `ConsumeDinnFront`.
   - **Physical path**: `C:\Sites\ConsumeDinnFront`.
   - **Binding**: `https`.
   - **Host name**: `app.seudominio.com`.
   - Certificado TLS válido para o domínio.
4. O site pode usar um application pool com **No Managed Code**, pois o frontend é estático.
5. Inicie o site.

Para redirecionar HTTP para HTTPS, crie também um binding HTTP na porta `80` e use o recurso URL Rewrite ou uma regra no balanceador/reverse proxy.

## 11. Validação pós-deploy

Verifique nesta ordem:

1. Abra `https://app.seudominio.com`.
2. Acesse diretamente uma rota interna, por exemplo `https://app.seudominio.com/envios`, e confirme que o fallback funciona.
3. No navegador, abra **F12 > Network** e confirme que as requisições vão para `https://api.seudominio.com` e não para `localhost`.
4. Faça login e confirme que o token JWT é aceito pela API.
5. Confirme no console do navegador que não há erro de CORS.
6. Teste a API no servidor com `Invoke-WebRequest`.
7. Confira os logs do IIS e os logs do ASP.NET Core em caso de erro `500.30`, `502.5` ou falha de inicialização.
8. Verifique se a API consegue conectar ao MySQL e se as migrations foram aplicadas.

Para diagnosticar a API, habilite temporariamente o log stdout no `web.config` publicado, apontando para uma pasta gravável pelo application pool. Desative-o após o diagnóstico, pois os logs podem crescer e conter informações sensíveis.

## 12. Atualizações futuras

Para cada versão:

1. Faça backup do banco e das pastas atuais.
2. Publique frontend e API em pastas temporárias.
3. Aplique migrations compatíveis com a versão.
4. Pare ou coloque o site em manutenção durante a troca dos arquivos, quando necessário.
5. Substitua o conteúdo das pastas de publicação.
6. Recicle os application pools.
7. Execute a validação pós-deploy.
8. Remova arquivos temporários e logs antigos.

Mantenha certificados TLS, segredos JWT, credenciais do MySQL e arquivos de configuração de produção fora do repositório e com permissões restritas.
