using System;

namespace BlazorSurveys.Shared;

public record SurveyAnswer
{
    public Guid   Id       { get; init; } = Guid.NewGuid();
    public Guid   SurveyId { get; init; }
    public string Option   { get; init; }
}