using AutoMapper;
using BookStoreApi.Data;
using BookStoreApi.DTOs;
using BookStoreApi.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApi.Features.Books.Commands.UpdateBook;

public class UpdateBookCommandHandler
    : IRequestHandler<UpdateBookCommand, BookDto?>
{
    private readonly BookStoreDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateBookCommandHandler> _logger;

    public UpdateBookCommandHandler(
        BookStoreDbContext context,
        IMapper mapper,
        ILogger<UpdateBookCommandHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<BookDto?> Handle(
        UpdateBookCommand request,
        CancellationToken cancellationToken)
    {
        var book = await _context.Books
            .FirstOrDefaultAsync(
                b => b.Id == request.Id,
                cancellationToken);

        if (book == null)
        {
            return null;
        }

        var authorExists = await _context.Authors
            .AnyAsync(
                a => a.Id == request.AuthorId,
                cancellationToken);

        if (!authorExists)
        {
            throw new BadRequestException(
    "Author does not exist.");
        }

        var categoryExists = await _context.Categories
            .AnyAsync(
                c => c.Id == request.CategoryId,
                cancellationToken);

        if (!categoryExists)
        {
            throw new BadRequestException(
    "Category does not exist.");
        }

        book.Title = request.Title;
        book.Price = request.Price;
        book.AuthorId = request.AuthorId;
        book.CategoryId = request.CategoryId;

        await _context.SaveChangesAsync(
            cancellationToken);

        _logger.LogInformation(
            "Book {BookId} updated successfully",
            book.Id);

        var updatedBook = await _context.Books
            .Include(b => b.Author)
            .Include(b => b.Category)
            .FirstAsync(
                b => b.Id == book.Id,
                cancellationToken);

        return _mapper.Map<BookDto>(updatedBook);
    }
}