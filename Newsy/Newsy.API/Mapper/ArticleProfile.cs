using AutoMapper;
using Newsy.Abstractions.Models;
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
        CreateMap<Article, BasicArticleModel>();
        CreateMap<Article, ArticleViewModel>();
    }
}
