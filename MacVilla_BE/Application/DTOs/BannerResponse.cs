namespace Application.DTOs;

public class BannerResponse
{
    public long BannerId { get; set; }
    public string? Title { get; set; }
    public string? ImageUrl { get; set; }
    public string? LinkUrl { get; set; }
    public int DisplayOrder { get; set; }
}
