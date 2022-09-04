using Backend.Entitys;

namespace Backend.Repositorys;

public class GroupRepository {
    public static PermissionGroup[] Groups;

    public static void CompileGroups(IConfiguration configuration) {
        var groupsSections = configuration.GetSection("Groups").GetChildren();
        List<PermissionGroup> groups = new List<PermissionGroup>();
        foreach (var section in groupsSections) {
            PermissionGroup group = new PermissionGroup();
            group.Name = section.GetValue<string>("Name");
            group.Permission = section.GetValue<string>("Permission");
            group.Permissions = section.GetSection("Permissions").Get<string[]>();
            group.Inherits = section.GetSection("Inherits").Get<string[]>();
            groups.Add(group);
        }
        Groups = groups.ToArray();
    }
    
    private readonly DatabaseContext _context;
    private readonly PermissionGroup[] _groups;

    public GroupRepository(DatabaseContext context) {
        _context = context;
        _groups = Groups;
    }

    public PermissionGroup GetPermissionGroup(string name) {
        return _groups.SingleOrDefault(group => group.Permission.Equals(name));
    }

    public PermissionGroup[] GetGroupsFromUser(Guid userId) {
        Permission[] permissions = GetUserPermissionsRaw(userId).ToArray();
        return ExtractGroups(permissions);
    }

    public PermissionGroup[] ExtractGroups(Permission[] permissions) {
        List<PermissionGroup> permissionGroups = new List<PermissionGroup>();
        foreach (var permission in permissions) {
            if (permission.PermissionKey.StartsWith("group.")) {
                foreach (var permissionGroup in _groups) {
                    if (permission.PermissionKey.Equals(permissionGroup.Permission)) {
                        permissionGroups.Add(permissionGroup);

                        if (permissionGroup.Inherits is not null) {
                            foreach (var inherit in permissionGroup.Inherits) {
                                permissionGroups.Add(GetPermissionGroup(inherit));
                            }
                        }
                    }
                }
            }
        }

        return permissionGroups.ToArray();
    }

    public IEnumerable<Permission> GetUserPermissions(Guid userId) {
        List<Permission> permissions = GetUserPermissionsRaw(userId).ToList();

        PermissionGroup[] groups = ExtractGroups(permissions.ToArray());
        foreach (var group in groups) {
            if (group.Permissions is null) continue;
            permissions.AddRange(group.Permissions
                .Select(perm => new Permission { Id = -1, UserId = userId, PermissionKey = perm }));
        }

        return permissions;
    }

    public IEnumerable<Permission> GetUserPermissionsRaw(Guid userId) {
        return _context.Permissions.Where(permission => permission.UserId == userId);
    }

    public void AddPermissions(Guid userId, params string[] permissions) {
        foreach (var permission in permissions) {
            _context.Permissions.Add(new Permission
                { PermissionKey = permission, UserId = userId });
        }

        _context.SaveChanges();
    }

    public void DeletePermissions(Guid userId, params string[] permissions) {
        foreach (var permission in permissions) {
            _context.Permissions.RemoveRange(_context.Permissions.Where(perm =>
                perm.UserId == userId && perm.PermissionKey == permission));
        }

        _context.SaveChanges();
    }
}