using Microsoft.Extensions.DependencyInjection;
using Newsy.Core.Contracts.Services;
using Newsy.Core.Services;

namespace Newsy.Core;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddCoreLayer(this IServiceCollection services)
    {
        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<IArticleService, ArticleService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
