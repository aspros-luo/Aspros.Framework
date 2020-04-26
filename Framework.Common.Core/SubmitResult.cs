namespace Framework.Common.Core
{
    public class SubmitResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public static SubmitResult Success()
        {
            return new SubmitResult { IsSuccess = true };
        }

        public static SubmitResult Success<T>(T data)
        {
            return new SubmitResult { IsSuccess = true, Data = data };
        }

        public static SubmitResult Fail(string message)
        {
            return new SubmitResult { IsSuccess = false, Message = message };
        }
    }
}
