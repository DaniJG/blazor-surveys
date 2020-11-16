using System;
using System.Collections.Generic;

namespace BlazorSurveys.Shared
{
    public class Survey
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime ExpiresAt { get; set; }
        public List<string> Options { get; set; }
        public List<SurveyAnswer> Answers { get;set; }
        public bool IsExpired => DateTime.Now > ExpiresAt;
    }

    public class SurveyAnswer
    {
        public Guid Id { get; set; }

        public Guid SurveyId { get; set; }
        public string Option { get; set; }
    }
}
