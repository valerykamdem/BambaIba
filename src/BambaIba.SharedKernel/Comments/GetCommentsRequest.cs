using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BambaIba.SharedKernel.Comments;
public sealed record GetCommentsRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
