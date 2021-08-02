using BlazorSurveys.Server.Hubs;
using BlazorSurveys.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorSurveys.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SurveyController : ControllerBase
    {
        private readonly IHubContext<SurveyHub, ISurveyHub> hubContext  ;

        public SurveyController(IHubContext<SurveyHub, ISurveyHub> surveyHub)
        {
            this.hubContext = surveyHub;
        }


        private static ConcurrentBag<Survey> surveys = new ConcurrentBag<Survey>
        {
            new Survey
            {
                Id = Guid.NewGuid(),
                Title = "Are you excited for Halo Infinite?",
                ExpiresAt = DateTime.Now.AddMinutes(10),
                Options = new List<string> { "Yes", "No", "Never heard of it" },
                Answers = new List<SurveyAnswer>
                {
                    new SurveyAnswer { Option = "Yes" },
                    new SurveyAnswer { Option = "Yes" },
                    new SurveyAnswer { Option = "No" },
                    new SurveyAnswer { Option = "Yes" },
                    new SurveyAnswer { Option = "Yes" },
                    new SurveyAnswer { Option = "No" },
                    new SurveyAnswer { Option = "Never heard of it" },
                    new SurveyAnswer { Option = "Yes" },
                    new SurveyAnswer { Option = "Yes" },

                }
            },
            new Survey
            {
                Id = Guid.NewGuid(),
                Title = "Are you excited for Battlefield 2042?",
                ExpiresAt = DateTime.Now.AddMinutes(10),
                Options = new List<string> { "Yes", "No", "Never heard of it" },
                Answers = new List<SurveyAnswer>
                {
                    new SurveyAnswer { Option = "Yes" },
                    new SurveyAnswer { Option = "Yes" },
                    new SurveyAnswer { Option = "No" },
                    new SurveyAnswer { Option = "Yes" },
                    new SurveyAnswer { Option = "Never heard of it" },
                    new SurveyAnswer { Option = "Yes" },
                    new SurveyAnswer { Option = "Yes" },
                    new SurveyAnswer { Option = "Yes" },
                    new SurveyAnswer { Option = "Yes" },

                }
            },
                        new Survey
            {
                Id = Guid.NewGuid(),
                Title = "How is life?",
                ExpiresAt = DateTime.Now.AddMinutes(10),
                Options = new List<string> { "Good", "Bad" },
                Answers = new List<SurveyAnswer>
                {
                    new SurveyAnswer { Option = "Bad" },
                    new SurveyAnswer { Option = "Good" },
                    new SurveyAnswer { Option = "Good" },
                    new SurveyAnswer { Option = "Bad" },
                    new SurveyAnswer { Option = "Bad" },
                    new SurveyAnswer { Option = "Good" },
                    new SurveyAnswer { Option = "Good" },
                    new SurveyAnswer { Option = "Good" },
                    new SurveyAnswer { Option = "good" },
                }
            }
        };
        [HttpGet()]
        public IEnumerable<SurveySummary> GetSurveys()
        {
            return surveys.Select(s => s.ToSummary());
        }

        [HttpGet("{id}")]
        public ActionResult GetSurvey(Guid id)
        {
            var survey = surveys.SingleOrDefault(i => i.Id == id);
            if (survey == null) return NotFound();
            return new JsonResult(survey);
        }

        [HttpPut()]
        public async Task<Survey> AddSurvey([FromBody] AddSurveyModel addSurveyModel)
        {
            var survey = new Survey
            {
                Title = addSurveyModel.Title,
                ExpiresAt = DateTime.Now.AddMinutes(addSurveyModel.Minutes.Value),
                Options = addSurveyModel.Options.Select(o => o.OptionValue).ToList()
            };
            surveys.Add(survey);
            await this.hubContext.Clients.All.SurveyAdded(survey.ToSummary());
            return survey;
        }

        [HttpPost("{surveyId}/answer")]
        public async Task<ActionResult> AnswerSurvey(Guid surveyId, [FromBody] SurveyAnswer answer)
        {
            
            var survey = surveys.SingleOrDefault(t => t.Id == surveyId);

            if (survey == null) return NotFound();
            if (((IExpirable)survey).IsExpired) return StatusCode(400, "This survey has expired");

            // WARNING: this isn’t thread safe since we store answers in a List!
            survey.Answers.Add(new SurveyAnswer
            {
                SurveyId = surveyId,
                Option = answer.Option
            });

            await this.hubContext.Clients.Group(surveyId.ToString()).SurveyUpdated(survey);
            return new JsonResult(survey);
        }
    }
}
