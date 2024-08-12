using Newsy.Abstractions.Models;

namespace Newsy.API.DTOs.Requests;

public class GridDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public List<Filter> Filters { get; set; } = new List<Filter>();
}
