namespace MacVilla_Web.Models
{
    public class BannerVM
    {
        public long BannerId { get; set; }
        public string? Title { get; set; }
        public string? ImageUrl { get; set; }
        public string? LinkUrl { get; set; }
        public int DisplayOrder { get; set; }
    }
}
