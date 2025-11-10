namespace BambaIba.SharedKernel;

public record ApiResult<T>(bool IsSuccess, T? Data, string[] Errors);
