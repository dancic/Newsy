namespace Newsy.API.DTOs.Requests;

public sealed record GridDto(int PageNumber = 1, int PageSize = 10);
