using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BambaIba.Application.Settings;
public class RabbitMqOptions
{
    public const string SectionName = "RabbitMQ";

    public string Host { get; set; } = "";
    public string User { get; set; } = "";
    public string Pass { get; set; } = "";
}

