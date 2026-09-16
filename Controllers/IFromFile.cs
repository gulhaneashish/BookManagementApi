namespace BookStoreApi.Controllers
{
    public interface IFromFile
    {
        int Length { get; }
        ReadOnlySpan<char> FileName { get; }
    }
}