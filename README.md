# blazor-surveys

Example **.NET 5.0** application that explores integrating **SignalR** with a **Blazor WebAssembly** application, as well as some of the latest C# features like record types.

To do so, it implements a very simple site for creating and participating in surveys:

- Users can define _surveys_ which are listed in the home page. Any new survey added is notified to any other user via SignalR.
- Users can participate in a survey and their vote/answer is notified to any other user viewing the same survey in real time via SignalR

## Installation
Make sure you have the .NET 5.0 SDK installed (download [here](https://dotnet.microsoft.com/download)) and clone this repo.

You will see a solution with 3 folders/projects:

- The `Client` folder contains the Blazor WebAssembly project. This is the actual Blazor application.
- The `Server` folder contains a standard ASP.NET application with 2 purposes:
    - host the client-side Blazor application, by allowing the browser to load the client assets like JS, CSS, WebAssembly modules, etc
    - provide the backend API and SignalR hubs, with the logic to create and interact with the surveys. This is no different from a normal ASP.NET Web API
- The `Shared` folder contains a standard .NET library project. Sharing code between the client and server is one of possible benefits of using Blazor, this project contains models shared between the Client and Server projects.

One you have cloned the repo, use standard dotnet commands/tooling to build and run. For example, from the root folder:
```bash
# restore dependencies
dotnet restore
# run the project
dotnet watch -p Server run
```

## Interesting features

Since this project is mostly an exercise to explore some of Blazor WebAssembly, SignalR and C# 9 features, its worth highlighting a few interesting bits:

- Since the real time features are _non essential_ for the application, the initalization of the client SignalR connection in `App.razor` is non-blocking. This means if unable to establish the SignalR connection, the client app can still load and function via the regular HTTP API
- Both pages  `Index.razor` and `SurveyDetails.razor` will unsubscribe from SignalR events when navigating away
- The shared models use the new C# 9 record types where possible
- The model for adding a survey uses data annotation to implement validation both client and server side
- The `AddSurvey.razor` page shows how to validate a form (including nested models), and how to update blazor's defaults to use bootstrap styling
- The `IExpirable` interfaces uses C# 8 implemented properties to provide read-only properties with the minutes/seconds remaining. The interface is then used by various record types.
- The blazor component `SurveyExpiration.razor` displays the remaining time for a survey and uses a timer to refresh the UX every second

## Reference

- Blazor WA with SignalR: https://docs.microsoft.com/en-us/aspnet/core/tutorials/signalr-blazor-webassembly?view=aspnetcore-5.0&tabs=visual-studio
- Forms validation with Blazor: https://docs.microsoft.com/en-us/aspnet/core/blazor/forms-validation?view=aspnetcore-5.0
- General Blazor docs: https://docs.microsoft.com/en-us/aspnet/core/blazor/?view=aspnetcore-5.0
