using AutoMapper;
using Newsy.Abstractions.Models;
using Newsy.API.DTOs.Requests;
using Newsy.API.DTOs.Responses;
using Newsy.Core.Models;
using Newsy.Persistence.Models;

namespace Newsy.API.Mapper;

public class AuthorProfile : Profile
{
    public AuthorProfile()
    {
        CreateMap<UpsertAuthorDto, UpsertAuthorServiceModel>();
        CreateMap<Author, BasicAuthorModel>()
            .ForMember(d => d.Name, opt => opt.MapFrom(src => src.ApplicationUser.FullName));
        CreateMap<Author, AuthorViewModel>()
            .ForMember(d => d.Name, opt => opt.MapFrom(src => src.ApplicationUser.FullName));
    }
}
