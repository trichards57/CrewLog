using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorApp.Client;
using Microsoft.AspNetCore.Components.Authorization;
using BlazorApp.Client.Auth;
using BlazorStrap;
using BlazorApp.Client.Stores;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["API_Prefix"] ?? builder.HostEnvironment.BaseAddress) })
    .AddAuthorizationCore()
    .AddScoped<AuthenticationStateProvider, AuthStateProvider>();

builder.Services.AddBlazorStrap();

builder.Services
    .AddSingleton<IStatusStore,StatusStore>();

await builder.Build().RunAsync();
