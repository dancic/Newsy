using AutoMapper;
using Newsy.API.DTOs.Requests;
using Newsy.Core.Models;

namespace Newsy.API.Mapper;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<RegisterDto, RegisterServiceModel>();
    }
}
