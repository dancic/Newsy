using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newsy.Persistence.Contracts.Services;
using Newsy.Persistence.Models;
using Newsy.Persistence.Services;

namespace Newsy.Persistence;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddPersistanceLayer(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IArticleRepository, ArticleRepository>();

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}
