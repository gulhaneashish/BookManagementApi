using MediatR;

namespace BookStoreApi.Features.Books.Commands.DeleteBook;

public record DeleteBookCommand(
    int Id
) : IRequest<bool>;