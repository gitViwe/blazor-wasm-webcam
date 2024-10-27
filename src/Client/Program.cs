using Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using SharedKernel.Realtime.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddMudServices()
    .AddSingleton<IRealtimeClient>(_ => new RealtimeClient(builder.Configuration))
    .AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.Configuration["ApiGatewayBaseUrl"]!) });

await builder.Build().RunAsync();
