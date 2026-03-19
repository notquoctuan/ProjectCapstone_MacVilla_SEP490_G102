namespace MacVilla_Web.DTOs
{
    public class LoginResponse
    {
        // Map from "accessToken" in API response
        public string? AccessToken { get; set; }
        
        // Also support "token" for backward compatibility
        public string? Token { get; set; }
        
        public long UserId { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Role { get; set; }
        
        // Helper property to get the token regardless of name
        public string TokenValue => !string.IsNullOrEmpty(AccessToken) 
            ? AccessToken 
            : (!string.IsNullOrEmpty(Token) ? Token : string.Empty);
    }
}
