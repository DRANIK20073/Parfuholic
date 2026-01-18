namespace Parfuholic.Services
{
    public static class UserSession
    {
        public static int UserId { get; set; }
        public static bool IsAuthorized => UserId > 0;
    }
}
