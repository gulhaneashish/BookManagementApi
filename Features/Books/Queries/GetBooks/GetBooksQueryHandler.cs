using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookStoreApi.Data;
using BookStoreApi.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApi.Features.Books.Queries.GetBooks;

public class GetBooksQueryHandler
    : IRequestHandler<GetBooksQuery, List<BookDto>>
{
    private readonly BookStoreDbContext _context;
    private readonly IMapper _mapper;

    public GetBooksQueryHandler(
        BookStoreDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<BookDto>> Handle(
        GetBooksQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Books
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(b =>
                b.Title.Contains(request.Search) ||
                b.Author.Name.Contains(request.Search));
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(b =>
                b.CategoryId == request.CategoryId.Value);
        }

        if (request.MinPrice.HasValue)
        {
            query = query.Where(b =>
                b.Price >= request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(b =>
                b.Price <= request.MaxPrice.Value);
        }

        query = request.Sort?.ToLower() switch
        {
            "priceasc" =>
                query.OrderBy(b => b.Price),

            "pricedesc" =>
                query.OrderByDescending(b => b.Price),

            "titleasc" =>
                query.OrderBy(b => b.Title),

            "titledesc" =>
                query.OrderByDescending(b => b.Title),

            _ =>
                query.OrderBy(b => b.Id)
        };

        return await query
            .ProjectTo<BookDto>(
                _mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}