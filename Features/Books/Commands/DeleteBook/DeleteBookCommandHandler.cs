using BookStoreApi.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApi.Features.Books.Commands.DeleteBook;

public class DeleteBookCommandHandler
    : IRequestHandler<DeleteBookCommand, bool>
{
    private readonly BookStoreDbContext _context;
    private readonly ILogger<DeleteBookCommandHandler> _logger;
    public DeleteBookCommandHandler(
    BookStoreDbContext context,
    ILogger<DeleteBookCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(
        DeleteBookCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Find the book
        var book = await _context.Books
            .FirstOrDefaultAsync(
                b => b.Id == request.Id,
                cancellationToken);

        // 2. Book does not exist
        if (book == null)
        {
            return false;
        }

        // 3. Remove the entity
        _context.Books.Remove(book);

        // 4. Save changes
        await _context.SaveChangesAsync(
            cancellationToken);
        _logger.LogInformation(
    "Book {BookId} deleted successfully",
    request.Id);
        return true;
    }
}