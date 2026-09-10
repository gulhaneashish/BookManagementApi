namespace BookStoreApi.Models;

public class Book
{
    public virtual int Id { get; set; }

    public virtual string Title { get; set; } = string.Empty;

    public virtual decimal Price { get; set; }

    public virtual int AuthorId { get; set; }

    public virtual Author Author { get; set; } = null!;

    public virtual int CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;
}