using System;

namespace Backend.Entitys;

public class Permission {
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string PermissionKey { get; set; }
}

public class PermissionGroup {
    public string Permission { get; set; }
    public string Name { get; set; }
    public string[] Permissions { get; set; }
    public string[] Inherits { get; set; }
}