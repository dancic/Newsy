namespace Newsy.Persistence.Extensions;

public static class IQueryableExtensions
{
    public static IQueryable<T> Paginate<T>(this IQueryable<T> campaigns, int page, int pageSize)
    {
        return campaigns.Skip((page - 1) * pageSize).Take(pageSize);
    }
}
