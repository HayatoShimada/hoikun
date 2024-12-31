namespace hoikun.Services
{
    public class UserStateService
    {
        public static string Email { get; set; } = string.Empty;
        public static string Username { get; set; } = string.Empty;
        public static string Role { get; set; } = string.Empty;
        public static string Id { get; set; } = string.Empty;
        public static string PostalCode { get; set; } = string.Empty;
        public static string State { get; set; } = string.Empty;
        public static string City { get; set; } = string.Empty;
        public static string Street { get; set; } = string.Empty;

        public string AccessToken { get; set; } = "";
        public static bool IsLoggedIn => !string.IsNullOrEmpty(Username);
    }
}
