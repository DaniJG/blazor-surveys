using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorSurveys.Shared;

public class SurveyHttpClient
{
    private readonly HttpClient http;

    public SurveyHttpClient(HttpClient http)
    {
        this.http = http;
    }

    public async Task<SurveySummary[]> GetSurveys()
    {
        return await http.GetFromJsonAsync<SurveySummary[]>("api/survey");
    }

    public async Task<Survey> GetSurvey(Guid surveyId)
    {
        return await http.GetFromJsonAsync<Survey>($"api/survey/{surveyId}");
    }

    public async Task<HttpResponseMessage> AddSurvey(AddSurveyModel survey)
    {
        return await http.PutAsJsonAsync("api/survey", survey);
    }

    public async Task<HttpResponseMessage> AnswerSurvey(Guid surveyId, SurveyAnswer answer)
    {
        return await http.PostAsJsonAsync($"api/survey/{surveyId}/answer", answer);
    }
}