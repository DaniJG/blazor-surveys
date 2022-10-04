using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using BlazorSurveys.Server.Hubs;
using BlazorSurveys.Shared;

namespace BlazorSurveys.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SurveyController : ControllerBase
{
    private readonly IHubContext<SurveyHub, ISurveyHub> hubContext;

    private static readonly ConcurrentBag<Survey> Surveys = new()
    {
        new Survey
        {
            Id        = Guid.Parse("b00c58c0-df00-49ac-ae85-0a135f75e01b"),
            Title     = "Are you excited about .NET 5.0?",
            ExpiresAt = DateTime.Now.AddMinutes(10),
            Options   = new List<string> {"Yes", "Nope", "meh", "PS5 just came out...", "None of the above, I got a series X"},
            Answers = new List<SurveyAnswer>
            {
                new() {Option = "Yes"},
                new() {Option = "Yes"},
                new() {Option = "Yes"},
                new() {Option = "Nope"},
                new() {Option = "meh"},
                new() {Option = "meh"},
                new() {Option = "PS5 just came out..."},
                new() {Option = "None of the above, I got a series X"}
            }
        },
        new Survey
        {
            Id        = Guid.Parse("7e467e51-9999-427e-bf81-015076b9f24c"),
            Title     = "What's the best food in the world?",
            ExpiresAt = DateTime.Now.AddMinutes(2),
            Options   = new List<string> {"Cheese", "Payoyo goat's cheese", "Tortilla", "Jamón", "Soylent!"},
            Answers = new List<SurveyAnswer>
            {
                new() {Option = "Cheese"},
                new() {Option = "Cheese"},
                new() {Option = "Payoyo goat's cheese"},
                new() {Option = "Payoyo goat's cheese"},
                new() {Option = "Payoyo goat's cheese"},
                new() {Option = "Payoyo goat's cheese"},
                new() {Option = "Tortilla"},
                new() {Option = "Jamón"},
                new() {Option = "Jamón"},
                new() {Option = "Jamón"}
            }
        }
    };

    public SurveyController(IHubContext<SurveyHub, ISurveyHub> surveyHub) => hubContext = surveyHub;

    [HttpGet]
    public IEnumerable<SurveySummary> GetSurveys() => Surveys.Select(s => s.ToSummary());

    [HttpGet("{id}")]
    public ActionResult GetSurvey(Guid id)
    {
        var survey = Surveys.SingleOrDefault(t => t.Id == id);
        if (survey == null) return NotFound();

        return new JsonResult(survey);
    }
        
    [HttpPut]
    public async Task<Survey> AddSurvey([FromBody] AddSurveyModel addSurveyModel)
    {
        var survey = new Survey{
            Title     = addSurveyModel.Title,
            ExpiresAt = DateTime.Now.AddMinutes(addSurveyModel.Minutes.Value),
            Options   = addSurveyModel.Options.Select(o => o.OptionValue).ToList()
        };

        Surveys.Add(survey);
        await hubContext.Clients.All.SurveyAdded(survey.ToSummary());
        return survey;
    }

    [HttpPost("{surveyId}/answer")]
    public async Task<ActionResult> AnswerSurvey(Guid surveyId, [FromBody] SurveyAnswer answer)
    {
        var survey = Surveys.SingleOrDefault(t => t.Id == surveyId);
        if (survey == null) return NotFound();
        if (((IExpirable)survey).IsExpired) return StatusCode(400, "This survey has expired");
            
        survey.Answers.Add(new SurveyAnswer{
            SurveyId = surveyId,
            Option   = answer.Option
        });
            
        await hubContext.Clients.Group(surveyId.ToString()).SurveyUpdated(survey);

        return new JsonResult(survey);
    }
}