using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorApp.Client;
using Microsoft.AspNetCore.Components.Authorization;
using BlazorApp.Client.Auth;
using BlazorStrap;
using BlazorApp.Client.Stores;
using BlazorApp.Client.Services.Interfaces;
using BlazorApp.Shared.Model;
using BlazorApp.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["API_Prefix"] ?? builder.HostEnvironment.BaseAddress) })
    .AddAuthorizationCore()
    .AddScoped<AuthenticationStateProvider, AuthStateProvider>();

builder.Services.AddBlazorStrap();

builder.Services
    .AddTransient<IServiceBase<Incident>, IncidentService>()
    .AddTransient<IServiceBase<Reflection>, ReflectionService>()
    .AddTransient<IServiceBase<Role>, RoleService>()
    .AddTransient<IServiceBase<Shift>, ShiftService>()
    .AddTransient<IServiceBase<Skill>, SkillService>()
    .AddSingleton<IStore<Incident>, IncidentStore>()
    .AddSingleton<IStore<Reflection>, ReflectionStore>()
    .AddSingleton<IStore<Role>, RoleStore>()
    .AddSingleton<IStore<Shift>, ShiftStore>()
    .AddSingleton<IStore<Skill>, SkillStore>();

await builder.Build().RunAsync();
