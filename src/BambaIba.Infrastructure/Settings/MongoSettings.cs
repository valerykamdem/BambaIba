using System;
using System.Collections.Generic;
using System.Text;

namespace BambaIba.Infrastructure.Settings;

public sealed class MongoSettings
{
    public const string SectionName = "Mongo";

    public string ConnectionString { get; set; } = string.Empty;

    public string Database { get; set; }
}
