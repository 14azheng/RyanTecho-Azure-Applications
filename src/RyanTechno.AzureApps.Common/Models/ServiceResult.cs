namespace RyanTechno.AzureApps.Common.Models
{
    public class ServiceResult
    {
        public bool IsCompleted { get; init; }

        public string Error { get; init; }

        public static ServiceResult FromSuccess() => new ServiceResult { IsCompleted = true };

        public static ServiceResult FromFailure(string error) => new ServiceResult
        { 
            IsCompleted = false,
            Error = error,
        };
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T Result { get; init; }

        public static ServiceResult<T> FromSuccess(T result) => new ServiceResult<T> 
        { 
            Result = result,
            IsCompleted = true,
        };

        public static ServiceResult<T> FromFailure(T result, string error) => new ServiceResult<T>
        {
            IsCompleted = false,
            Error = error,
            Result = result,
        };
    }
}