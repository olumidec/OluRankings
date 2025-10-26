namespace OluRankings.Identity
{
    public static class AppRoles
    {
        public const string Admin   = "Admin";
        public const string Ranker  = "Ranker";
        public const string Coach   = "Coach";
        public const string Athlete = "Athlete";
        public const string Parent  = "Parent";
        public const string Viewer  = "Viewer";

        public static readonly string[] All = { Admin, Ranker, Coach, Athlete, Parent, Viewer };
    }
}
