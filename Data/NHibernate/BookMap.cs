using FluentNHibernate.Mapping;
using BookStoreApi.Models;

namespace BookStoreApi.Data.NHibernate;

public class BookMap : ClassMap<Book>
{
    public BookMap()
    {
        Table("Books");

        Id(x => x.Id)
            .Column("Id")
            .GeneratedBy.Identity();

        Map(x => x.Title)
            .Column("Title");

        Map(x => x.Price)
            .Column("Price");

        Map(x => x.AuthorId)
            .Column("AuthorId");

        Map(x => x.CategoryId)
            .Column("CategoryId");
    }
}