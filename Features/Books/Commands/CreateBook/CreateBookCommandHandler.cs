using AutoMapper;
using BookStoreApi.Data;
using BookStoreApi.DTOs;
using BookStoreApi.Exceptions;
using BookStoreApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApi.Features.Books.Commands.CreateBook;

public class CreateBookCommandHandler
    : IRequestHandler<CreateBookCommand, BookDto>
{
    private readonly BookStoreDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateBookCommandHandler> _logger;

    public CreateBookCommandHandler(
        BookStoreDbContext context,
        IMapper mapper,
        ILogger<CreateBookCommandHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<BookDto> Handle(
        CreateBookCommand request,
        CancellationToken cancellationToken)
    {
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

        var book = new Book
        {
            Title = request.Title,
            Price = request.Price,
            AuthorId = request.AuthorId,
            CategoryId = request.CategoryId
        };

        _context.Books.Add(book);

        await _context.SaveChangesAsync(
            cancellationToken);

        _logger.LogInformation(
            "Book {BookId} created successfully with title {Title}",
            book.Id,
            book.Title);

        var createdBook = await _context.Books
            .Include(b => b.Author)
            .Include(b => b.Category)
            .FirstAsync(
                b => b.Id == book.Id,
                cancellationToken);

        return _mapper.Map<BookDto>(createdBook);
    }
}