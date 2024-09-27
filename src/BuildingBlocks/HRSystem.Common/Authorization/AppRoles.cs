using System.Collections.ObjectModel;

namespace HRSystem.Common.Authorization
{
    /// <summary>
    /// this is for seed a database with a default roles
    /// </summary>
    public static class AppRoles
    {
        public const string Admin = nameof(Admin);
        public const string Guest = nameof(Guest);

        public static IReadOnlyList<string> DeafultRoles => new List<string> { Admin, Guest };

        public static bool IsDeafult(string roleName) =>
            DeafultRoles.Any(r => r == roleName);
    }
}
