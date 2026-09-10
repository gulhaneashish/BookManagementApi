using AutoMapper;
using BookStoreApi.Data;
using BookStoreApi.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApi.Features.Books.Queries.GetBookById;

public class GetBookByIdQueryHandler
    : IRequestHandler<GetBookByIdQuery, BookDto?>
{
    private readonly BookStoreDbContext _context;
    private readonly IMapper _mapper;

    public GetBookByIdQueryHandler(
        BookStoreDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BookDto?> Handle(
        GetBookByIdQuery request,
        CancellationToken cancellationToken)
    {
        var book = await _context.Books
            .Include(b => b.Author)
            .Include(b => b.Category)
            .FirstOrDefaultAsync(
                b => b.Id == request.Id,
                cancellationToken);

        if (book == null)
        {
            return null;
        }

        return _mapper.Map<BookDto>(book);
    }
}