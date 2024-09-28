namespace HRSystem.Common.Authorization;
/// <summary>
/// seed appllication permessions 
/// </summary>
/// <param name="Feature"></param>
/// <param name="Action"></param>
/// <param name="Group"></param>
/// <param name="Description"></param>
/// <param name="IsBasic">when i create a new user the only permesion that assign to it is the permession that take isbasic = true</param>
public record AppPermession(string Feature, string Action, string Group, string Description, bool IsBasic = false)
{
    public string Name => NameFor(Feature, Action);

    public static string NameFor(string feature, string action)
     => $"permessions.{feature}.{action}";

}

// here we will build our permessions
public class AppPermessions
{
    private static readonly AppPermession[] _all = new AppPermession[]
    {
        new(AppFeature.Users, AppAction.Create, AppRoleGroup.SystemAccess, "Create Users"),
        new(AppFeature.Users, AppAction.Update, AppRoleGroup.SystemAccess, "Update Users"),
        new(AppFeature.Users, AppAction.Read, AppRoleGroup.SystemAccess, "Read Users"),
        new(AppFeature.Users, AppAction.Delete, AppRoleGroup.SystemAccess, "Delete Users"),

        new(AppFeature.UserRoles, AppAction.Read, AppRoleGroup.SystemAccess, "Read User Roles"),
        new(AppFeature.UserRoles, AppAction.Update, AppRoleGroup.SystemAccess, "Update User Roles"),

        new(AppFeature.Roles, AppAction.Read, AppRoleGroup.SystemAccess, "Read Roles"),
        new(AppFeature.Roles, AppAction.Create, AppRoleGroup.SystemAccess, "Create Roles"),
        new(AppFeature.Roles, AppAction.Update, AppRoleGroup.SystemAccess, "Update Roles"),
        new(AppFeature.Roles, AppAction.Delete, AppRoleGroup.SystemAccess, "Delete Roles"),

        new(AppFeature.RoleClaims, AppAction.Read, AppRoleGroup.SystemAccess, "Read Role Claims/Permissions"),
        new(AppFeature.RoleClaims, AppAction.Update, AppRoleGroup.SystemAccess, "Update Role Claims/Permissions"),

        new(AppFeature.Employees, AppAction.Read, AppRoleGroup.ManagementHierarchy, "Read Employees", IsBasic: true),
        new(AppFeature.Employees, AppAction.Create, AppRoleGroup.ManagementHierarchy, "Create Employees"),
        new(AppFeature.Employees, AppAction.Update, AppRoleGroup.ManagementHierarchy, "Update Employees"),
        new(AppFeature.Employees, AppAction.Delete, AppRoleGroup.ManagementHierarchy, "Delete Employees")

    };

    // admin only permession
    public static IReadOnlyList<AppPermession> AdminPermessions => _all.Where(p=>!p.IsBasic).ToList(); 
    public static IReadOnlyList<AppPermession> BasicPermessions => _all.Where(p=>p.IsBasic).ToList(); 
}


