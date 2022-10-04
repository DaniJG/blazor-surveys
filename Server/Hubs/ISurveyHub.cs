using BlazorSurveys.Shared;
using System.Threading.Tasks;

namespace BlazorSurveys.Server.Hubs;

public interface ISurveyHub
{
    Task SurveyAdded(SurveySummary survey);
    Task SurveyUpdated(Survey      survey);
}