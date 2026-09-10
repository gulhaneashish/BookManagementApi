using BookStoreApi.DTOs;
using MediatR;

namespace BookStoreApi.Features.Books.Queries.GetBookById;

public record GetBookByIdQuery(
    int Id
) : IRequest<BookDto?>;