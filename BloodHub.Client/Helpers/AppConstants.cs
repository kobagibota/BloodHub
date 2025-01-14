namespace BloodHub.Client.Helpers
{
    public static class AppConstants
    {
        // Key lưu trữ AccessToken và RefreshToken trong LocalStorage
        public const string AccessTokenKey = "AccessToken";
        public const string RefreshTokenKey = "RefreshToken";

        // Endpoint API
        public static class ApiEndpoints
        {
            public const string Login = "api/auth/login";
            public const string RefreshToken = "api/auth/refresh";
            public const string CurrentUser = "api/auth/me";
            public const string ResetPassword = "api/auth/reset-password";
            public const string ChangePassword = "api/auth/change-password";
        }
    }
}
