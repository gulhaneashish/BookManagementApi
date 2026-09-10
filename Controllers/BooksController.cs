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
namespace BookStoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly BookService _bookService;
    private readonly DapperBookService _dapperBookService;
    private readonly NHibernateBookService _nhibernateBookService;
    private readonly IMediator _mediator;
    public BooksController(
    BookService bookService,
    DapperBookService dapperBookService,
    NHibernateBookService nhibernateBookService,
     IMediator mediator)
    {
        _bookService = bookService;
        _dapperBookService = dapperBookService;
        _nhibernateBookService = nhibernateBookService;
        _mediator = mediator;
    }

    //[HttpGet]
    //[Authorize(AuthenticationSchemes = "Bearer")]
    //public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks(
    //  string? search,
    //  int? categoryId,
    //  decimal? minPrice,
    //  decimal? maxPrice,
    //  string? sort)
    //{
    //    var books = await _bookService.GetBooksAsync(
    //        search,
    //        categoryId,
    //        minPrice,
    //        maxPrice,
    //        sort);

    //    return Ok(books);
    //}

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

    //[HttpGet("{id}")]
    //public async Task<ActionResult<BookDto>> GetBook(int id)
    //{
    //    var book = await _bookService.GetBookByIdAsync(id);

    //    if (book == null)
    //    {
    //        return NotFound();
    //    }

    //    return Ok(book);
    //}

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

    //[HttpPost]
    //public async Task<ActionResult<BookDto>> CreateBook(
    // CreateBookDto dto)
    //{
    //    var result = await _bookService.CreateBookAsync(dto);

    //    if (!result.Success)
    //    {
    //        return BadRequest(result.ErrorMessage);
    //    }

    //    return CreatedAtAction(
    //        nameof(GetBook),
    //        new { id = result.Data!.Id },
    //        result.Data);
    //}
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
    //[HttpPut("{id}")]
    //public async Task<ActionResult<BookDto>> UpdateBook(
    // int id,
    // UpdateBookDto dto)
    //{
    //    var result = await _bookService.UpdateBookAsync(id, dto);

    //    if (result.NotFound)
    //    {
    //        return NotFound();
    //    }

    //    if (!result.Success)
    //    {
    //        return BadRequest(result.ErrorMessage);
    //    }

    //    return Ok(result.Data);
    //}

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

    //[HttpDelete("{id}")]
    //[Authorize(
    // AuthenticationSchemes = "Bearer",
    // Roles = "Admin")]
    //public async Task<IActionResult> DeleteBook(int id)
    //{
    //    var deleted = await _bookService.DeleteBookAsync(id);

    //    if (!deleted)
    //    {
    //        return NotFound();
    //    }

    //    return NoContent();
    //}
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