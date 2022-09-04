using Backend.Entitys;
using Backend.Options;
using Microsoft.Extensions.Options;

namespace Backend.Repositorys; 

public class UserRepository {
    private DatabaseContext _context;
    private TokenRepository _tokens;
    private GroupRepository _groups;
    private UserOptions _options;

    public UserRepository(DatabaseContext context, TokenRepository tokens, GroupRepository groups, IOptions<UserOptions> options) {
        _context = context;
        _tokens = tokens;
        _groups = groups;
        _options = options.Value;
    }

    public IEnumerable<User> GetUsers() => _context.Users.OrderBy(user => user.Created);

    public User GetUser(Guid userId) => _context.Users.SingleOrDefault(user => user.Id == userId);

    public User CreateUser(UserEditor editor) {
        var user = new User {
            Id = Guid.NewGuid(),
            Created = DateTime.Now,

            Email = editor.Email,
            FirstName = editor.FirstName,
            LastName = editor.LastName,
            Password = TokenRepository.Hash128(editor.Password, editor.Email),
            Username = editor.Username
        };

        _context.Users.Add(user);
        _context.SaveChanges();
        _groups.AddPermissions(user.Id, _options.DefaultPermissions);

        return user;
    }

    public void EditUser(Guid userId, UserEditor editor) {
        var user = GetUser(userId);

        string SetValue(string orig, string input, string hashed = null) {
            if (!string.IsNullOrEmpty(input))
                return !string.IsNullOrEmpty(hashed) ? hashed : input;

            return orig;
        }

        user.Email = SetValue(user.Email, editor.Email);
        user.FirstName = SetValue(user.FirstName, editor.FirstName);
        user.LastName = SetValue(user.LastName, editor.LastName);
        user.Username = SetValue(user.Username, editor.Username);
        user.Password = SetValue(user.Password, editor.Password, TokenRepository.Hash128(editor.Password, editor.Email));

        _context.SaveChanges();
    }

    public void DeleteUser(Guid userId) {
        _context.Users.Remove(_context.Users.Single(user => user.Id == userId));
        _context.Permissions.RemoveRange(_context.Permissions.Where(perm => perm.UserId == userId));
        _tokens.DeleteUserTokens(userId);
        _context.SaveChanges();
    }
}