using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace BlazorSurveys.Server.Hubs;

public class SurveyHub : Hub<ISurveyHub>
{
    public async Task JoinSurveyGroup(Guid  surveyId) => await Groups.AddToGroupAsync(Context.ConnectionId, surveyId.ToString());
    public async Task LeaveSurveyGroup(Guid surveyId) => await Groups.RemoveFromGroupAsync(Context.ConnectionId, surveyId.ToString());
}