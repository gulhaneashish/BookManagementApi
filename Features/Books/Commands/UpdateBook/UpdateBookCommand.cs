using BookStoreApi.DTOs;
using MediatR;

namespace BookStoreApi.Features.Books.Commands.UpdateBook;

public record UpdateBookCommand(
    int Id,
    string Title,
    decimal Price,
    int AuthorId,
    int CategoryId
) : IRequest<BookDto?>;