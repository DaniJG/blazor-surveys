using System;
using System.Collections.Generic;

namespace BlazorSurveys.Shared;

public record Survey : IExpirable
{
    public Guid               Id        { get; init; } = Guid.NewGuid();
    public string             Title     { get; init; }
    public DateTime           ExpiresAt { get; init; }
    public List<string>       Options   { get; init; } = new();
    public List<SurveyAnswer> Answers   { get; init; } = new();

    public SurveySummary ToSummary() => new ()
    {
        Id        = Id,
        Title     = Title,
        Options   = Options,
        ExpiresAt = ExpiresAt
    };
}