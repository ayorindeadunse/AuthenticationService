namespace AuthenticationService.Business.Models.DTOs;

public class ResultDTO<TResponse>
{
    public bool IsSuccess { get; set; }
    
    public TResponse? Response { get; set; }

    public IEnumerable<string>? Errors { get; set; }

    public ResultDTO(bool isSuccess, TResponse response, IEnumerable<string>? errors)
    {
        IsSuccess = isSuccess;
        Response = response;
        Errors = errors;
    }
}