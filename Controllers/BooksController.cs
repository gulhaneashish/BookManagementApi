using BookStoreApi.Data;
using BookStoreApi.DTOs;
using BookStoreApi.Features.Books.Commands.CreateBook;
using BookStoreApi.Features.Books.Commands.DeleteBook;
using BookStoreApi.Features.Books.Commands.UpdateBook;
using BookStoreApi.Features.Books.Queries.GetBookById;
using BookStoreApi.Features.Books.Queries.GetBooks;
using BookStoreApi.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly BookStoreDbContext _context;
    private readonly BookService _bookService;
    private readonly DapperBookService _dapperBookService;
    private readonly NHibernateBookService _nhibernateBookService;
    private readonly IMediator _mediator;

    public BooksController(
        BookService bookService,
        DapperBookService dapperBookService,
        NHibernateBookService nhibernateBookService,
        BookStoreDbContext context,
        IMediator mediator)
    {
        _bookService = bookService;
        _dapperBookService = dapperBookService;
        _nhibernateBookService = nhibernateBookService;
        _context = context;
        _mediator = mediator;
    }

    [EnableRateLimiting("fixed")]
    [HttpGet]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks(
        string? search,
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        string? sort)
    {
        var query = new GetBooksQuery(
            search,
            categoryId,
            minPrice,
            maxPrice,
            sort);

        var books = await _mediator.Send(query);

        return Ok(books);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookDto>> GetBook(int id)
    {
        var query = new GetBookByIdQuery(id);

        var book = await _mediator.Send(query);

        if (book == null)
        {
            return NotFound();
        }

        return Ok(book);
    }

    [HttpPost]
    public async Task<ActionResult<BookDto>> CreateBook(
        CreateBookDto dto)
    {
        var command = new CreateBookCommand(
            dto.Title,
            dto.Price,
            dto.AuthorId,
            dto.CategoryId);

        var result = await _mediator.Send(command);

        return CreatedAtAction(
            nameof(GetBook),
            new { id = result.Id },
            result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BookDto>> UpdateBook(
        int id,
        UpdateBookDto dto)
    {
        var command = new UpdateBookCommand(
            id,
            dto.Title,
            dto.Price,
            dto.AuthorId,
            dto.CategoryId);

        var book = await _mediator.Send(command);

        if (book == null)
        {
            return NotFound();
        }

        return Ok(book);
    }

    [HttpDelete("{id}")]
    [Authorize(
        AuthenticationSchemes = "Bearer",
        Roles = "Admin")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var command = new DeleteBookCommand(id);

        var deleted = await _mediator.Send(command);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("dapper")]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetBooksUsingDapper(
        string? search,
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        string? sort)
    {
        var books = await _dapperBookService.GetBooksAsync(
            search,
            categoryId,
            minPrice,
            maxPrice,
            sort);

        return Ok(books);
    }

    [HttpGet("nhibernate")]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetBooksUsingNHibernate()
    {
        var books = await _nhibernateBookService.GetBooksAsync();

        return Ok(books);
    }
}