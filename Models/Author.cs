namespace BookStoreApi.Models;

public class Author
{
    public virtual int Id { get; set; }

    public virtual string Name { get; set; } = string.Empty;

    public virtual ICollection<Book> Books { get; set; }
        = new List<Book>();
}