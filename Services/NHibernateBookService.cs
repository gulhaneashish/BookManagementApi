using BookStoreApi.Data.NHibernate;
using BookStoreApi.DTOs;
using NHibernate;

namespace BookStoreApi.Services;

public class NHibernateBookService
{
    private readonly NHibernateSessionFactory _sessionFactory;

    public NHibernateBookService(
        NHibernateSessionFactory sessionFactory)
    {
        _sessionFactory = sessionFactory;
    }

    public async Task<IEnumerable<BookDto>> GetBooksAsync()
    {
        using var session = _sessionFactory.OpenSession();

        const string sql = """
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
            ORDER BY b.Id
            """;

        var results = await session.CreateSQLQuery(sql)
            .SetResultTransformer(
                NHibernate.Transform.Transformers
                    .AliasToBean<BookDto>())
            .ListAsync<BookDto>();

        return results;
    }
}