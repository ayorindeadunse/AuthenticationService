using AuthenticationService.Business.Models.DTOs;
using FluentResults;

namespace AuthenticationService.Business.Extensions;

public static class ResultDtoExtensions
{
    public static ResultDTO<TResponse> ToResultDto<TResponse>(this Result<TResponse> result)
    {
        var errorMessages = result.Errors?.Select(error => error.Message);
        return new ResultDTO<TResponse>(result.IsSuccess, result.ValueOrDefault, errorMessages);
    }
}