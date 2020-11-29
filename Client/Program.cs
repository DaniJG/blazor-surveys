using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Http;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components;
using BlazorSurveys.Shared;

namespace BlazorSurveys.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            var baseAddress = new Uri(builder.HostEnvironment.BaseAddress);
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = baseAddress });
            // Configure a typed HttpClient with all the survey API endpoints
            // so the razor pages/components dont need to use the raw HttpClient
            builder.Services.AddHttpClient<SurveyHttpClient>(client => client.BaseAddress = baseAddress);

            // Register a preconfigure SignalR hub connection.
            // Note the connection isnt yet started, this will be done as part of the App.razor component
            // to avoid blocking the application startup in case the connection cannot be established
            builder.Services.AddSingleton<HubConnection>(sp => {
                var navigationManager = sp.GetRequiredService<NavigationManager>();
                return new HubConnectionBuilder()
                  .WithUrl(navigationManager.ToAbsoluteUri("/surveyhub"))
                  .WithAutomaticReconnect()
                  .Build();
            });

            var host = builder.Build();
            await host.RunAsync();

            // As an alternative, we could create the hubConnection here and connect to it
            // then continue with the host startup process.
            // However not this would delay rendering the app untol the signalR connection is established
            // and would prevent the app from rendering at all if an error is raised.
            // var navigationManager = host.Services.GetRequiredService<NavigationManager>();
            // var hubConnection = new HubConnectionBuilder()
            //   .WithUrl(navigationManager.ToAbsoluteUri("/surveyhub"))
            //   .WithAutomaticReconnect()
            //   .Build();
            // await hubConnection.StartAsync();

            // We could launch a task that will keep retrying indefinitely as per:
            //  https://docs.microsoft.com/en-us/aspnet/core/signalr/dotnet-client?view=aspnetcore-5.0&tabs=visual-studio#handle-lost-connection
            // We should also handle Closed connection to attempt running this code again

            // some kind of initialization?
            // maybe check on hubConnection.State == HubConnectionState.Disconnected
            // Task.Delay(20000).ContinueWith(t => Task.Run(() => hubConnection.StartAsync()));
            // Task.Run(() => hubConnection.StartAsync());

            // builder.Services.AddSingleton<HubConnection>(hubConnection);
            // await host.RunAsync();
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
