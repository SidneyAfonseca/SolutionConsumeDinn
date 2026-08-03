using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ConsumeDinnFront;
using ConsumeDinnFront.Auth;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<TokenStore>();
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<JwtAuthenticationStateProvider>());

// Apontando para o backend ConsumeDinnApi — o handler anexa o token JWT
builder.Services.AddScoped(sp => new HttpClient(new BearerTokenHandler(sp.GetRequiredService<TokenStore>()))
{
    BaseAddress = new Uri("https://localhost:7151")
});
builder.Services.AddScoped<DinnApiClient>();

await builder.Build().RunAsync();
