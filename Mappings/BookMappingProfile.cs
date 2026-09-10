using AutoMapper;
using BookStoreApi.DTOs;
using BookStoreApi.Models;

namespace BookStoreApi.Mappings;

public class BookMappingProfile : Profile
{
    public BookMappingProfile()
    {
        CreateMap<Book, BookDto>()
            .ForMember(
                dest => dest.AuthorName,
                opt => opt.MapFrom(
                    src => src.Author.Name))
            .ForMember(
                dest => dest.CategoryName,
                opt => opt.MapFrom(
                    src => src.Category.Name));
    }
}