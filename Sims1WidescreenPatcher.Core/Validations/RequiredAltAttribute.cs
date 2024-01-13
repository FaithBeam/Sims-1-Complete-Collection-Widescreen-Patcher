using System.ComponentModel.DataAnnotations;

namespace Sims1WidescreenPatcher.Core.Validations;

/// <summary>
/// My hack to get around the Required attribute from popping on the first edit which will come from the FindSimsPathService
/// </summary>
public class RequiredAltAttribute : RequiredAttribute
{
    private static bool Visited;
    
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (!Visited)
        {
            Visited = true;
            return ValidationResult.Success;
        }

        return base.IsValid(value, validationContext);
    }
}