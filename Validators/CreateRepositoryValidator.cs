using FluentValidation;
using MetricsAPI.DTOs;


namespace MetricsAPI.Validators;

public class CreateRepositoryValidator : AbstractValidator<CreateRepositoryDto>
{
    public CreateRepositoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

        RuleFor(x => x.Url)
            .NotEmpty().WithMessage("URL is required")
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("URL must be a valid URI");

        RuleFor(x => x.Language)
            .NotEmpty().WithMessage("Language is required");
    }
}