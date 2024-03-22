using FluentValidation;

namespace CozyFileService.Application.Features.ManageFiles.Commands.CreateFile
{
    public class CreateFileCommandValidator : AbstractValidator<CreateFileCommand>
    {
        public CreateFileCommandValidator()
        {
            RuleFor(p => p.FileName)
                .NotNull().WithMessage("{PropertyName} is required.");

            RuleFor(p => p.FileType)
                .NotNull().WithMessage("{PropertyName} is required.");

            RuleFor(p => p.FileSize)
                .GreaterThan(0).WithMessage("{PropertyName} is required.");

        }
    }
}
