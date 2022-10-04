using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components;
using BlazorSurveys.Shared;

namespace BlazorSurveys.Client;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");

        var baseAddress = new Uri(builder.HostEnvironment.BaseAddress);
        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = baseAddress });
        builder.Services.AddHttpClient<SurveyHttpClient>(client => client.BaseAddress = baseAddress);
        builder.Services.AddSingleton(sp => {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            return new HubConnectionBuilder()
                  .WithUrl(navigationManager.ToAbsoluteUri("/surveyhub"))
                  .WithAutomaticReconnect()
                  .Build();
        });

        var host = builder.Build();
        await host.RunAsync();
    }
}