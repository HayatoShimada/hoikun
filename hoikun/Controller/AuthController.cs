namespace hoikun.Controller
{
    using hoikun.Data; // IDbContextService のある名前空間
    using hoikun.Services;

    using Microsoft.AspNetCore.Mvc;

    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text.Json;
    using System.Threading.Tasks;


    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IDbContextService _dbContextService;
        private readonly UserStateService userStateService;

        public AuthController(HttpClient httpClient, IDbContextService dbContextService)
        {
            _httpClient = httpClient;
            _dbContextService = dbContextService;
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("No code provided");
            }

            Dictionary<string, string> tokenRequest = new()
            {
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", "https://localhost:5000/auth/callback" }, // LINE Developers に登録したものと一致するように
                { "client_id", "2006662452" },
                { "client_secret", "411631a9659ad46f3d28421323fd4bb9" }
            };

            HttpResponseMessage response = await _httpClient.PostAsync("https://api.line.me/oauth2/v2.1/token",
                new FormUrlEncodedContent(tokenRequest));

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Failed to get access token");
            }

            string responseContent = await response.Content.ReadAsStringAsync();
            LineTokenResponse? tokenResponse = JsonSerializer.Deserialize<LineTokenResponse>(responseContent);

            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.access_token))
            {
                return BadRequest("Invalid token response");
            }

            string accessToken = tokenResponse.access_token;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string userProfileResponse = await _httpClient.GetStringAsync("https://api.line.me/v2/profile");

            LineUserProfile? userProfile = JsonSerializer.Deserialize<LineUserProfile>(userProfileResponse);

            if (userProfile == null || string.IsNullOrEmpty(userProfile.userId))
            {
                return BadRequest("Invalid user profile response");
            }

            // 既存のユーザーを取得（Emailや他の識別情報を利用）
            List<User>? users = await _dbContextService.GetUserAsync(null);
            User? existingUser = users?.FirstOrDefault(u => u.UserId == UserStateService.ModelId); // ユーザー識別方法を適宜変更

            if (existingUser != null)
            {

                // 更新後、DBの変更を適用
                existingUser.LineId = userProfile.userId;
                await _dbContextService.UpdateUserAsync(users => users.Where(u => u.UserId == existingUser.UserId));
            }

            // LINE連携成功ページへリダイレクト
            return Redirect("/auth/success");
        }

        [HttpGet("success")]
        public IActionResult Success()
        {
            return Content("LINE連携が成功しました！");
        }
    }

    // LINE トークンレスポンス用クラス
    public class LineTokenResponse
    {
        public string access_token { get; set; } = string.Empty;
        public int expires_in { get; set; }
        public string id_token { get; set; } = string.Empty;
        public string refresh_token { get; set; } = string.Empty;
        public string scope { get; set; } = string.Empty;
        public string token_type { get; set; } = string.Empty;
    }

    // LINE ユーザープロフィールレスポンス用クラス
    public class LineUserProfile
    {
        public string userId { get; set; } = string.Empty;
        public string displayName { get; set; } = string.Empty;
        public string pictureUrl { get; set; } = string.Empty;
    }
}
