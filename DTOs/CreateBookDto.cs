namespace BookStoreApi.DTOs;

public class CreateBookDto
{
    public string Title { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int AuthorId { get; set; }

    public int CategoryId { get; set; }
}