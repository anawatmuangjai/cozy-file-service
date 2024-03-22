using FluentValidation.Results;

namespace CozyFileService.Application.Exceptions
{
    public class ValidationException : Exception
    {
        public List<string> ValidationErrors;

        public ValidationException(ValidationResult validationResult)
        {
            ValidationErrors = new List<string>();

            foreach (var validationError in validationResult.Errors)
            {
                ValidationErrors.Add(validationError.ErrorMessage);
            }
        }
    }
}
