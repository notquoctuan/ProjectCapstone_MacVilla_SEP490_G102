namespace MacVilla_Web.Models
{
    public class UserAdminVM
    {
        public long UserId { get; set; }
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }
        public string? Position { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}