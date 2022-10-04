using System.ComponentModel.DataAnnotations;

namespace BlazorSurveys.Shared;

public class OptionCreateModel
{
    [Required]
    [MaxLength(20)]
    public string OptionValue { get; set; }
}