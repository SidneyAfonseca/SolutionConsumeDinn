using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ConsumeDinnFront;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Apontando para o backend ConsumeDinnApi
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7151") });

await builder.Build().RunAsync();
