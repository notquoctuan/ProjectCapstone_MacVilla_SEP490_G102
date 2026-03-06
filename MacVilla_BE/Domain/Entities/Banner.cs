using System;

namespace Domain.Entities;

public partial class Banner
{
    public long BannerId { get; set; }
    public string? Title { get; set; }
    public string? ImageUrl { get; set; }
    public string? LinkUrl { get; set; }
    public int? DisplayOrder { get; set; }
    public bool? IsActive { get; set; }
}
