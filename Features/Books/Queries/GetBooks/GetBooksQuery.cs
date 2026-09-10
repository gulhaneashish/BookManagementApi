using BookStoreApi.DTOs;
using MediatR;

namespace BookStoreApi.Features.Books.Queries.GetBooks;

public record GetBooksQuery(
    string? Search,
    int? CategoryId,
    decimal? MinPrice,
    decimal? MaxPrice,
    string? Sort
) : IRequest<List<BookDto>>;