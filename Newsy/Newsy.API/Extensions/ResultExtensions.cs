using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Newsy.Abstractions;

namespace Newsy.API.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T, TViewModel>(this Result<T> result, Func<T, TViewModel>? mapper = null)
    {
        if (result.IsSuccess)
        {
            return new OkObjectResult(mapper != null ? mapper(result.Value) : result.Value);
        }

        return GetErrorObjectResult(result.Errors);
    }

    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return new OkObjectResult(result.Value);
        }

        return GetErrorObjectResult(result.Errors);
    }

    public static IActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess)
        {
            return new OkResult();
        }

        return GetErrorObjectResult(result.Errors);
    }

    private static ObjectResult GetErrorObjectResult(List<IError> errors)
    {
        var firstError = errors.FirstOrDefault();
        if (firstError != null && firstError.Metadata.TryGetValue(Constants.StatusCodeMetadataName, out var statusCode) && statusCode is int statusCodeInt)
        {
            return new ObjectResult(firstError.Message) { StatusCode = statusCodeInt };
        }

        return new ObjectResult("Unhandeled exception.") { StatusCode = 500 };
    }
}
