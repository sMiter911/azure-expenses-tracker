namespace AKExpensesTracker.Shared.Responses
{
    public class ApiSuccessResponse<T> : ApiErrorResponse
    {
        public ApiSuccessResponse()
        {
            IsSuccess = true;
            ResponseDate = DateTime.UtcNow;
        }

        public ApiSuccessResponse(T? records) : this()
        {
            Records = records;
        }
        
        public ApiSuccessResponse(string message, T? records) : this()
        {
            Message = message;
            Records = records;
        }
        
        public ApiSuccessResponse(string message) : this()
        {
            Message = message;
        }

        public T? Records { get; set; }
    }
}
