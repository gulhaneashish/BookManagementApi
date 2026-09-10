using BookStoreApi.DTOs;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BookStoreApi.Services;

public class DapperBookService
{
    private readonly IConfiguration _configuration;

    public DapperBookService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<IEnumerable<BookDto>> GetBooksAsync(
     string? search,
     int? categoryId,
     decimal? minPrice,
     decimal? maxPrice,
     string? sort)
    {
        var connectionString =
            _configuration.GetConnectionString("DefaultConnection");

        using var connection =
            new SqlConnection(connectionString);

        var sql = """
        SELECT
            b.Id,
            b.Title,
            b.Price,
            b.AuthorId,
            a.Name AS AuthorName,
            b.CategoryId,
            c.Name AS CategoryName
        FROM Books b
        INNER JOIN Authors a
            ON b.AuthorId = a.Id
        INNER JOIN Categories c
            ON b.CategoryId = c.Id
        WHERE 1 = 1
        """;

        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(search))
        {
            sql += """
            AND (
                b.Title LIKE @Search
                OR a.Name LIKE @Search
            )
            """;

            parameters.Add(
                "Search",
                $"%{search}%");
        }

        if (categoryId.HasValue)
        {
            sql += " AND b.CategoryId = @CategoryId ";

            parameters.Add(
                "CategoryId",
                categoryId.Value);
        }

        if (minPrice.HasValue)
        {
            sql += " AND b.Price >= @MinPrice ";

            parameters.Add(
                "MinPrice",
                minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            sql += " AND b.Price <= @MaxPrice ";

            parameters.Add(
                "MaxPrice",
                maxPrice.Value);
        }

        sql += sort?.ToLower() switch
        {
            "priceasc" => " ORDER BY b.Price ASC ",
            "pricedesc" => " ORDER BY b.Price DESC ",
            "titleasc" => " ORDER BY b.Title ASC ",
            "titledesc" => " ORDER BY b.Title DESC ",
            _ => " ORDER BY b.Id "
        };

        return await connection.QueryAsync<BookDto>(
            sql,
            parameters);
    }
}