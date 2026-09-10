namespace BookStoreApi.Services;

public class ServiceResult<T>
{
    public bool Success { get; set; }

    public bool NotFound { get; set; }

    public string? ErrorMessage { get; set; }

    public T? Data { get; set; }
}