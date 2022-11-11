namespace RyanTechno.AzureApps.Services
{
    public class TaskResult
    {
        public bool IsCompleted { get; init; }

        public string Error { get; init; }

        public static TaskResult Success => new TaskResult { IsCompleted = true };
    }

    public class TaskResult<T> : TaskResult
    {
        public T Result { get; init; }
    }
}