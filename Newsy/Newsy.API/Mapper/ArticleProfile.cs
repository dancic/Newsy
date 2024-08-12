using AutoMapper;
using Newsy.API.DTOs.Requests;
using Newsy.API.DTOs.Responses;
using Newsy.Core.Models;
using Newsy.Persistence.Models;

namespace Newsy.API.Mapper;

public class ArticleProfile : Profile
{
    public ArticleProfile()
    {
        CreateMap<UpsertArticleDto, UpsertArticleServiceModel>();
        CreateMap<Article, BasicArticleViewModel>()
            .ForMember(d => d.AuthorName, opt => opt.MapFrom(src => src.Author.ApplicationUser.FullName));
        CreateMap<Article, ArticleViewModel>()
            .ForMember(d => d.AuthorName, opt => opt.MapFrom(src => src.Author.ApplicationUser.FullName));
    }
}
