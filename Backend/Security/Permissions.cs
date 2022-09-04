namespace Backend.Security; 

public static class Permissions {

    public const string ShowUsers = "users.see";
    public const string EditUsers = "users.edit";
    public const string DeleteUsers = "users.delete";
    public const string LogoutUsers = "users.logout";
    public const string EditUserPermissions = "users.permissions.edit";
    public const string ShowUserPermissions = "users.permissions.show";
    
    public const string EditOwnPermissions = "self.permissions.edit";

}