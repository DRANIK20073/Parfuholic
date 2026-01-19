namespace Parfuholic.Services
{
    public static class AuthService
    {
        public static int? CurrentUserId { get; private set; }
        public static string CurrentUserLogin { get; private set; }

        public static bool IsLoggedIn => CurrentUserId.HasValue;

        public static void Login(int userId, string login)
        {
            CurrentUserId = userId;
            CurrentUserLogin = login;
        }

        public static void Logout()
        {
            CurrentUserId = null;
            CurrentUserLogin = null;
        }
    }
}
