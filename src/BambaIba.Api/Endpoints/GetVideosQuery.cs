namespace BambaIba.Api.Endpoints;

internal class GetVideosQuery
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string Search { get; set; }
}