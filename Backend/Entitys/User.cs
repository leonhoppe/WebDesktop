using System;

namespace Backend.Entitys; 

public class User : UserEditor {
    public Guid Id { get; set; }
    public DateTime Created { get; set; }
}

public class UserLogin {
    public string UsernameOrEmail { get; set; }
    public string Password { get; set; }
}

public class UserEditor {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}