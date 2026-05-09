namespace Domain.Exceptions
{
    public sealed class ValidationException : Exception
    {
        public IEnumerable<string> Errors { get; set; } = [];
        public ValidationException(IEnumerable<string> messages) :base("Validation failed")
        {
            Errors = messages;
        }
    }
}
