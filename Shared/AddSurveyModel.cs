using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlazorSurveys.Shared;

public class AddSurveyModel: IValidatableObject
{
    [Required]
    [MaxLength(50)]
    public string Title { get; set; }

    [Required]
    public int? Minutes { get; set; }
    
    [ValidateComplexType]
    public List<OptionCreateModel> Options { get; init; } = new();

    public void RemoveOption(OptionCreateModel option) => Options.Remove(option);

    public void AddOption() => Options.Add(new OptionCreateModel());

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Options.Count < 2 )
        {
            yield return new ValidationResult("A survey requires at least 2 options.");
        }
    }
}