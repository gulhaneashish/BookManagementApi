using BookStoreApi.Data;
using BookStoreApi.DTOs;
using BookStoreApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApi.Services;

public class BookService
{
    private readonly BookStoreDbContext _context;

    public BookService(BookStoreDbContext context)
    {
        _context = context;
    }

    public async Task<List<BookDto>> GetBooksAsync(
        string? search,
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        string? sort)
    {
        var query = _context.Books
            .Include(b => b.Author)
            .Include(b => b.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(b =>
                b.Title.Contains(search) ||
                b.Author.Name.Contains(search));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(b =>
                b.CategoryId == categoryId.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(b =>
                b.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(b =>
                b.Price <= maxPrice.Value);
        }

        query = sort?.ToLower() switch
        {
            "priceasc" => query.OrderBy(b => b.Price),
            "pricedesc" => query.OrderByDescending(b => b.Price),
            "titleasc" => query.OrderBy(b => b.Title),
            "titledesc" => query.OrderByDescending(b => b.Title),
            _ => query.OrderBy(b => b.Id)
        };

        return await query
            .Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Price = b.Price,
                AuthorId = b.AuthorId,
                AuthorName = b.Author.Name,
                CategoryId = b.CategoryId,
                CategoryName = b.Category.Name
            })
            .ToListAsync();
    }

    public async Task<BookDto?> GetBookByIdAsync(int id)
    {
        return await _context.Books
            .Include(b => b.Author)
            .Include(b => b.Category)
            .Where(b => b.Id == id)
            .Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Price = b.Price,
                AuthorId = b.AuthorId,
                AuthorName = b.Author.Name,
                CategoryId = b.CategoryId,
                CategoryName = b.Category.Name
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ServiceResult<BookDto>> CreateBookAsync(
    CreateBookDto dto)
    {
        var authorExists = await _context.Authors
            .AnyAsync(a => a.Id == dto.AuthorId);

        if (!authorExists)
        {
            return new ServiceResult<BookDto>
            {
                ErrorMessage = "Author does not exist."
            };
        }

        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == dto.CategoryId);

        if (!categoryExists)
        {
            return new ServiceResult<BookDto>
            {
                ErrorMessage = "Category does not exist."
            };
        }

        var book = new Book
        {
            Title = dto.Title,
            Price = dto.Price,
            AuthorId = dto.AuthorId,
            CategoryId = dto.CategoryId
        };

        _context.Books.Add(book);

        await _context.SaveChangesAsync();

        var result = await GetBookByIdAsync(book.Id);

        return new ServiceResult<BookDto>
        {
            Success = true,
            Data = result
        };
    }

    public async Task<ServiceResult<BookDto>> UpdateBookAsync(
    int id,
    UpdateBookDto dto)
    {
        var book = await _context.Books
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null)
        {
            return new ServiceResult<BookDto>
            {
                NotFound = true
            };
        }

        var authorExists = await _context.Authors
            .AnyAsync(a => a.Id == dto.AuthorId);

        if (!authorExists)
        {
            return new ServiceResult<BookDto>
            {
                ErrorMessage = "Author does not exist."
            };
        }

        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == dto.CategoryId);

        if (!categoryExists)
        {
            return new ServiceResult<BookDto>
            {
                ErrorMessage = "Category does not exist."
            };
        }

        book.Title = dto.Title;
        book.Price = dto.Price;
        book.AuthorId = dto.AuthorId;
        book.CategoryId = dto.CategoryId;

        await _context.SaveChangesAsync();

        var result = await GetBookByIdAsync(book.Id);

        return new ServiceResult<BookDto>
        {
            Success = true,
            Data = result
        };
    }

    public async Task<bool> DeleteBookAsync(int id)
    {
        var book = await _context.Books
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null)
        {
            return false;
        }

        _context.Books.Remove(book);

        await _context.SaveChangesAsync();

        return true;
    }
}