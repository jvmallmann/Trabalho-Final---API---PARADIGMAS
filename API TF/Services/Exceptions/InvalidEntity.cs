using System;

namespace API_TF.Services.Exceptions
{
    public class InvalidEntity : Exception
    {
        public InvalidEntity() { }

        public InvalidEntity(string message) : base(message) { }

        public InvalidEntity(string message, Exception inner) : base(message, inner) { }
    }
}