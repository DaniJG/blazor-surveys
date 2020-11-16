using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using BlazorSurveys.Server.Hubs;
using BlazorSurveys.Shared;

namespace BlazorSurveys.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyController : ControllerBase
    {
        private readonly IHubContext<SurveyHub, ISurveyHub> hubContext;
        private static ConcurrentBag<Survey> surveys = new ConcurrentBag<Survey> {
          new Survey {
                Id = Guid.Parse("b00c58c0-df00-49ac-ae85-0a135f75e01b"),
                Title = "Are you excited about .NET 5.0?",
                ExpiresAt = DateTime.Now.AddMinutes(10),
                Options = new List<string>{ "Yes!", "Nope", "meh", "PS5 just came out...", "None of the above, I got a series X" },
                Answers = new List<SurveyAnswer>{
                  new SurveyAnswer { Option = "Yes" },
                  new SurveyAnswer { Option = "Yes" },
                  new SurveyAnswer { Option = "Yes" },
                  new SurveyAnswer { Option = "Nope" },
                  new SurveyAnswer { Option = "meh" },
                  new SurveyAnswer { Option = "meh" },
                  new SurveyAnswer { Option = "PS5 just came out..." },
                  new SurveyAnswer { Option = "None of the above, I got a series X" }
                }
            },
          new Survey {
                Id = Guid.Parse("7e467e51-9999-427e-bf81-015076b9f24c"),
                Title = "What's the best food in the world?",
                ExpiresAt = DateTime.Now.AddMinutes(2),
                Options = new List<string>{ "Cheese", "Payoyo goat's cheese", "Tortilla", "Jamón", "Soylent!" },
                Answers = new List<SurveyAnswer>{
                  new SurveyAnswer { Option = "Cheese" },
                  new SurveyAnswer { Option = "Cheese" },
                  new SurveyAnswer { Option = "Payoyo goat's cheese" },
                  new SurveyAnswer { Option = "Payoyo goat's cheese" },
                  new SurveyAnswer { Option = "Payoyo goat's cheese" },
                  new SurveyAnswer { Option = "Payoyo goat's cheese" },
                  new SurveyAnswer { Option = "Tortilla" },
                  new SurveyAnswer { Option = "Jamón" },
                  new SurveyAnswer { Option = "Jamón" },
                  new SurveyAnswer { Option = "Jamón" }
                }
            },
        };

        public SurveyController(IHubContext<SurveyHub, ISurveyHub> surveyHub)
        {
            this.hubContext = surveyHub;
        }

        [HttpGet()]
        public IEnumerable<Survey> GetSurveys()
        {
            return surveys;
        }

        [HttpGet("{id}")]
        public ActionResult GetSurvey(Guid id)
        {
            var survey = surveys.SingleOrDefault(t => t.Id == id);
            if (survey == null) return NotFound();

            return new JsonResult(survey);
        }

        [HttpPost()]
        public async Task<Survey> AddSurvey([FromBody]Survey survey)
        {
            survey.Id = Guid.NewGuid();
            survey.Answers = new List<SurveyAnswer>();
            surveys.Add(survey);
            await this.hubContext.Clients.All.SurveyAdded(survey);
            return survey;
        }

        [HttpPost("{surveyId}/answer")]
        public async Task<ActionResult> AddAnswerAsync(Guid surveyId, [FromBody]SurveyAnswer answer)
        {
            var survey = surveys.SingleOrDefault(t => t.Id == surveyId);
            if (survey == null) return NotFound();
            if (survey.IsExpired) return StatusCode(400, "This survey has expired");

            answer.Id = Guid.NewGuid();
            answer.SurveyId = surveyId;
            // warning, this isnt thread safe since we store answers in a List
            survey.Answers.Add(answer);

            // Notify anyone connected to the survey group
            // ofc sending the entire survey all the time is inefficient, but enough in this tutorial
            await this.hubContext.Clients.Group(surveyId.ToString()).SurveyUpdated(survey);

            return new JsonResult(answer);
        }
    }
}