using System.ComponentModel.DataAnnotations;

namespace Newsy.API.Attributes;

public class RequiredIfAttribute : ValidationAttribute
{
    private readonly string _booleanPropertyName;

    public RequiredIfAttribute(string booleanPropertyName)
    {
        _booleanPropertyName = booleanPropertyName;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Find the IsAuthor property on the model
        var booleanProperty = validationContext.ObjectType.GetProperty(_booleanPropertyName);

        if (booleanProperty == null)
        {
            return new ValidationResult($"Unknown property: {_booleanPropertyName}");
        }

        // Get the value of the IsAuthor property
        var isAuthor = (bool)booleanProperty.GetValue(validationContext.ObjectInstance);

        // Check if the Bio should be required
        if (isAuthor && string.IsNullOrEmpty(value?.ToString()))
        {
            return new ValidationResult($"{validationContext.DisplayName} is required when {_booleanPropertyName} is true.");
        }

        return ValidationResult.Success;
    }
}
