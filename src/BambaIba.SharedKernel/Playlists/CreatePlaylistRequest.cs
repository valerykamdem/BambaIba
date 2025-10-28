using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BambaIba.SharedKernel.Playlists;
public record CreatePlaylistRequest
{
    [Required, MaxLength(100)]
    public string Name { get; init; }

    [MaxLength(1000)]
    public string Description { get; init; }

    [Range(0, 1)]
    public bool IsPublic { get; init; }
}
