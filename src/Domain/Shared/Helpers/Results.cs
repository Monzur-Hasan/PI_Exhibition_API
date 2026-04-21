namespace Domain.Shared.Helpers
{
    public class Result<T>
    {
        public bool Succeeded { get; }
        public T Value { get; }
        public IEnumerable<string> Errors { get; }

        private Result(bool succeeded, T value, IEnumerable<string> errors)
        {
            Succeeded = succeeded;
            Value = value;
            Errors = errors;
        }

        public static Result<T> Success(T value)
        {
            return new Result<T>(true, value, Enumerable.Empty<string>());
        }

        public static Result<T> Failure(IEnumerable<string> errors)
        {
            return new Result<T>(false, default, errors);
        }
    }

    // Optional: Non-generic version for void operations
    public class Result
    {
        public bool Succeeded { get; }
        public IEnumerable<string> Errors { get; }

        private Result(bool succeeded, IEnumerable<string> errors)
        {
            Succeeded = succeeded;
            Errors = errors;
        }

        public static Result Success()
        {
            return new Result(true, Enumerable.Empty<string>());
        }

        public static Result Failure(IEnumerable<string> errors)
        {
            return new Result(false, errors);
        }
    }
}
