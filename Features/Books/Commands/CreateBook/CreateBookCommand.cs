using BookStoreApi.DTOs;
using MediatR;

namespace BookStoreApi.Features.Books.Commands.CreateBook;

public record CreateBookCommand(
    string Title,
    decimal Price,
    int AuthorId,
    int CategoryId
) : IRequest<BookDto>;