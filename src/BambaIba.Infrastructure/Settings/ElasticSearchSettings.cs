using Elastic.Clients.Elasticsearch.QueryDsl;

namespace BambaIba.Infrastructure.Settings;

public class ElasticSearchSettings
{
    public const string SectionName = "ElasticSearch";

    public string Url { get; set; } = string.Empty;
    public string Index { get; set; } = string.Empty;
}
