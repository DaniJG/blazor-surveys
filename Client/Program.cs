using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components;

namespace BlazorSurveys.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddSingleton(sp => {
                var navigationManager = sp.GetRequiredService<NavigationManager>();
                return new HubConnectionBuilder()
                  .WithUrl(navigationManager.ToAbsoluteUri("/surveyhub"))
                  .WithAutomaticReconnect()
                  .Build();
            });

            var host = builder.Build();
            await host.RunAsync();

            var navigationManager = host.Services.GetRequiredService<NavigationManager>();
            var hubConnection = new HubConnectionBuilder()
              .WithUrl(navigationManager.ToAbsoluteUri("/surveyhub"))
              .WithAutomaticReconnect()
              .Build();

            // This will block the app rendering if any error is thrown
            // await hubConnection.StartAsync();

            // We could launch a task that will keep retrying indefinitely as per:
            //  https://docs.microsoft.com/en-us/aspnet/core/signalr/dotnet-client?view=aspnetcore-5.0&tabs=visual-studio#handle-lost-connection
            // We should also handle Closed connection to attempt running this code again

            // some kind of initialization?
            // maybe check on hubConnection.State == HubConnectionState.Disconnected
            // Task.Delay(20000).ContinueWith(t => Task.Run(() => hubConnection.StartAsync()));
            // Task.Run(() => hubConnection.StartAsync());
            builder.Services.AddSingleton<HubConnection>(hubConnection);

            await host.RunAsync();
        }

        // public static async Task<bool> ConnectWithRetryAsync(HubConnection connection, CancellationToken token)
        // {
        //     // Keep trying to until we can start or the token is canceled.
        //     while (true)
        //     {
        //         try
        //         {
        //             await connection.StartAsync(token);

        //             return true;
        //         }
        //         catch when (token.IsCancellationRequested)
        //         {
        //             return false;
        //         }
        //         catch
        //         {
        //             // Failed to connect, trying again in 5000 ms.
        //             await Task.Delay(5000);
        //         }
        //     }
        // }
    }
}
