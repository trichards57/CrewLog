using BlazorStrap;
using CrewLog.Client;
using CrewLog.Client.Auth;
using CrewLog.Client.Services;
using CrewLog.Client.Services.Interfaces;
using CrewLog.Client.Stores;
using CrewLog.Shared.Model;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Security.Claims;

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
    .AddScoped<IStore<Incident>, IncidentStore>()
    .AddScoped<ISubItemStore<Incident>, IncidentStore>()
    .AddScoped<IStore<Reflection>, ReflectionStore>()
    .AddScoped<IStore<Role>, RoleStore>()
    .AddScoped<IStore<Shift>>(s => s.GetRequiredService<IShiftStore>())
    .AddScoped<IShiftStore, ShiftStore>()
    .AddScoped<IStore<Skill>>(s => s.GetRequiredService<ISkillStore>())
    .AddScoped<ISkillStore, SkillStore>()
    .AddScoped<ISettingsStore, SettingsStore>();

await builder.Build().RunAsync();
