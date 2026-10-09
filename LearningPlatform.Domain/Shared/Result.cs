namespace LearningPlatform.Domain.Shared
{
    public class Result
    {
        public bool IsSuccess { get; }
        public string? ErrorMessage { get; }
        public ErrorType ErrorType { get; }
        protected Result(bool isSuccess, ErrorType errorType = ErrorType.None, string? errorMessage = null)
        {
            IsSuccess = isSuccess;
            ErrorType = errorType;
            ErrorMessage = errorMessage;
        }
        public static Result Success()
        {
            return new Result(true, ErrorType.None);
        }

        public static Result Failure(string errorMessage, ErrorType errorType = ErrorType.Failure)
        {
            return new Result(false, errorType, errorMessage);
        }
    }
}
