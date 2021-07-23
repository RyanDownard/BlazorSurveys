using System;
using System.Collections.Generic;

public record Survey
{
    public Guid Id {get; init; } = Guid.NewGuid();
    public string Title { get; init; }
    public DateTime ExpiresAt { get; set; }
    public List<string> Options { get; set; }
    public List<SurveyAnswer> Answers {get; init; } = new List<SurveyAnswer>();

    public SurveySummary ToSummary() => new SurveySummary
    {
        Id = this.Id,
        Title = this.Title,
        Options = this.Options,
        ExpiresAt = this.ExpiresAt
    };
}

public record SurveyAnswer
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid SurveyId { get; set; }
    public string Option { get; set; }
}

public record SurveySummary
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public DateTime ExpiresAt { get; init; }
    public List<string> Options { get; set; }
}


public class AddSurveyModel
{
    public string Title { get; set; }
    public int? Minutes { get; set; }
    public List<OptionCreateModel> Options { get; init; } = new List<OptionCreateModel>();

    public void RemoveOption(OptionCreateModel option) => this.Options.Remove(option);
    public void AddOption() => this.Options.Add(new OptionCreateModel()); 
}

public class OptionCreateModel
{
    public string OptionValue { get; set; }
}