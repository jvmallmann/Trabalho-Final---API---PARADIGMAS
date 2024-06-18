using FluentValidation.Results;
using System.Collections.Generic;
using System;

namespace API_TF.Services.Exceptions
{
    public class InvalidDataException : Exception
    {
        public IEnumerable<ValidationFailure> ValidationErrors { get; }

        public InvalidDataException(string message, IEnumerable<ValidationFailure> validationErrors)
            : base(message)
        {
            ValidationErrors = validationErrors;
        }
    }
}
