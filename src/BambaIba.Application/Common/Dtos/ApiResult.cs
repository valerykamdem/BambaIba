namespace BambaIba.Application.Common.Dtos;

public record ApiResult<T>(bool IsSuccess, T? Data, string[] Errors);
