using System.ComponentModel.DataAnnotations;

namespace Sims1WidescreenPatcher.Core.Validations;

public class FileExistsAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var path = value as string;

        if (string.IsNullOrWhiteSpace(path))
        {
            return ValidationResult.Success;
        }

        return File.Exists(path)
            ? ValidationResult.Success
            : new ValidationResult($"Path to Sims.exe does not exist");
    }
}
