﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BlazorSurveys.Shared
{
    public class SurveyHttpClient
    {
        private readonly HttpClient http;

        public SurveyHttpClient(HttpClient http)
        {
            this.http = http;
        }

        public async Task<SurveySummary[]> GetSurveys()
        {
            return await this.http.GetFromJsonAsync<SurveySummary[]>("api/survey");
        }

        public async Task<Survey> GetSurvey(Guid surveyId)
        {
            return await this.http.GetFromJsonAsync<Survey>($"api/survey/{surveyId}");
        }

        public async Task<HttpResponseMessage> AddSurvey(AddSurveyModel survey)
        {
            return await this.http.PutAsJsonAsync<AddSurveyModel>("api/survey", survey);
        } 

        public async Task<HttpResponseMessage> AnswerSurvey(Guid surveyId, SurveyAnswer answer)
        {
            return await this.http.PostAsJsonAsync<SurveyAnswer>($"api/survey/{surveyId}/answer", answer);
        }
    }
}
