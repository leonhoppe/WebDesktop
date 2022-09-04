using System;
using System.Collections.Generic;
using System.Linq;
using Backend.Entitys;
using Backend.LogicResults;
using Backend.Options;
using Backend.Repositorys;
using Backend.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Backend.Logic;

public class UserLogic {
    private UserRepository _users;
    private TokenRepository _tokens;
    private GroupRepository _groups;
    private UserMessageOptions _messages;
    private IHttpContextAccessor _contextAccessor;
    private ITokenContext _context;

    public UserLogic(
        UserRepository users,
        TokenRepository tokens,
        GroupRepository groups,
        IOptions<UserMessageOptions> messages,
        IHttpContextAccessor contextAccessor,
        ITokenContext context) {
        _users = users;
        _tokens = tokens;
        _groups = groups;
        _messages = messages.Value;
        _contextAccessor = contextAccessor;
        _context = context;
    }

    public ILogicResult<IEnumerable<User>> GetUsers() {
        var users = _users.GetUsers()
            .Select(user => new User {
                Id = user.Id,
                Created = user.Created,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username
            });

        return LogicResult<IEnumerable<User>>.Ok(users);
    }

    public ILogicResult<User> GetUser(Guid userId) {
        var user = _users.GetUser(userId);
        if (user is null) return LogicResult<User>.NotFound(_messages.NotFound);

        return LogicResult<User>.Ok(new User {
            Id = user.Id,
            Created = user.Created,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Username = user.Username
        });
    }

    public ILogicResult EditUser(Guid userId, UserEditor editor) {
        if (!ValidateEdit(editor)) return LogicResult.BadRequest(_messages.InvalidEditData);
        if (_users.GetUser(userId) is null) return LogicResult.NotFound(_messages.NotFound);
        
        _users.EditUser(userId, editor);
        return LogicResult.Ok();
    }

    public ILogicResult DeleteUser(Guid userId) {
        if (_users.GetUser(userId) is null) return LogicResult.NotFound(_messages.NotFound);
        
        _tokens.DeleteUserTokens(userId);
        _users.DeleteUser(userId);
        
        return LogicResult.Ok();
    }

    public ILogicResult<IEnumerable<string>> GetPermissions(Guid userId) {
        if (_users.GetUser(userId) is null) return LogicResult<IEnumerable<string>>.NotFound(_messages.NotFound);
        
        return LogicResult<IEnumerable<string>>.Ok(_groups.GetUserPermissions(userId).Select(perm => perm.PermissionKey));
    }
    
    public ILogicResult<IEnumerable<string>> GetPermissionsRaw(Guid userId) {
        if (_users.GetUser(userId) is null) return LogicResult<IEnumerable<string>>.NotFound(_messages.NotFound);
        
        return LogicResult<IEnumerable<string>>.Ok(_groups.GetUserPermissionsRaw(userId).Select(perm => perm.PermissionKey));
    }

    public ILogicResult AddPermissions(Guid userId, params string[] permissions) {
        if (_users.GetUser(userId) is null) return LogicResult.NotFound(_messages.NotFound);
        
        _groups.AddPermissions(userId, permissions);
        return LogicResult.Ok();
    }
    
    public ILogicResult DeletePermissions(Guid userId, params string[] permissions) {
        if (_users.GetUser(userId) is null) return LogicResult.NotFound(_messages.NotFound);
        
        _groups.DeletePermissions(userId, permissions);
        return LogicResult.Ok();
    }

    public ILogicResult<AccessToken> Login(UserLogin login) {
        var user = _users.GetUsers().SingleOrDefault(user =>
            user.Username == login.UsernameOrEmail || user.Email == login.UsernameOrEmail);

        if (user is null) return LogicResult<AccessToken>.NotFound(_messages.NotFound);
        if (user.Password != TokenRepository.Hash128(login.Password, user.Email))
            return LogicResult<AccessToken>.BadRequest(_messages.WrongPassword);

        _tokens.DeleteUserTokens(user.Id);
        var refreshToken = _tokens.CreateRefreshToken(user.Id);
        var accessToken = _tokens.CreateAccessToken(refreshToken.Id);
        SetRefreshToken(refreshToken);

        return LogicResult<AccessToken>.Ok(accessToken);
    }

    public ILogicResult<AccessToken> Register(UserEditor editor) {
        var users = _users.GetUsers();
        if (users.Any(user => user.Email == editor.Email || user.Username == editor.Username))
            return LogicResult<AccessToken>.Conflict(_messages.UsernameOrEmailExist);
        
        if (!ValidateUserdata(editor))
            return LogicResult<AccessToken>.BadRequest(_messages.InvalidRegisterData);

        var user = _users.CreateUser(editor);
        var refreshToken = _tokens.CreateRefreshToken(user.Id);
        var accessToken = _tokens.CreateAccessToken(refreshToken.Id);
        SetRefreshToken(refreshToken);

        return LogicResult<AccessToken>.Ok(accessToken);
    }

    public ILogicResult Logout(Guid userId) {
        if (_users.GetUser(userId) is null) return LogicResult.NotFound(_messages.NotFound);
        _tokens.DeleteUserTokens(userId);
        
        if (_context.UserId == userId) DeleteRefreshToken();
        return LogicResult.Ok();
    }

    public ILogicResult<AccessToken> GenerateToken(Guid refreshTokenId) {
        if (!_tokens.ValidateRefreshToken(refreshTokenId)) return LogicResult<AccessToken>.Conflict(_messages.InvalidRefreshToken);
        var token = _tokens.GetAccessTokens(refreshTokenId).ToArray().FirstOrDefault(token => _tokens.ValidateAccessToken(token.Id));
        if (token is not null) return LogicResult<AccessToken>.Ok(token);
        return LogicResult<AccessToken>.Ok(_tokens.CreateAccessToken(refreshTokenId));
    }
    
    public Guid GetCurrentUserRefreshToken() {
        var token = _contextAccessor.HttpContext?.Request.Cookies["refresh_token"];
        if (token == null) return Guid.Empty;
        return Guid.Parse(token);
    }

    private bool ValidateUserdata(UserEditor editor) {
        if (string.IsNullOrEmpty(editor.FirstName)) return false;
        if (string.IsNullOrEmpty(editor.LastName)) return false;
        if (string.IsNullOrEmpty(editor.Email)) return false;
        if (string.IsNullOrEmpty(editor.Username)) return false;
        if (string.IsNullOrEmpty(editor.Password)) return false;

        if (editor.FirstName.Length > 255) return false;
        if (editor.LastName.Length > 255) return false;
        if (editor.Email.Length > 255) return false;
        if (editor.Username.Length > 255) return false;
        if (editor.Password.Length > 255) return false;

        if (!editor.Email.Contains('@') || !editor.Email.Contains('.')) return false;
        if (editor.Username.Contains('@')) return false;
        if (editor.Password.Length < 8) return false;

        return true;
    }
    private bool ValidateEdit(UserEditor editor) {
        if (editor.FirstName?.Length > 255) return false;
        if (editor.LastName?.Length > 255) return false;
        if (editor.Email?.Length > 255) return false;
        if (editor.Username?.Length > 255) return false;
        if (editor.Password?.Length > 255) return false;

        if (!string.IsNullOrEmpty(editor.Email)) {
            if (!editor.Email.Contains('@') || !editor.Email.Contains('.')) return false;
        }

        if (!string.IsNullOrEmpty(editor.Username)) {
            if (editor.Username.Contains('@')) return false;
        }

        if (!string.IsNullOrEmpty(editor.Password)) {
            if (editor.Password.Length < 8) return false;
        }

        return true;
    }
    

    private void DeleteRefreshToken() {
        _contextAccessor.HttpContext?.Response.Cookies.Delete("refresh_token");
    }
    private void SetRefreshToken(RefreshToken token) {
        _contextAccessor.HttpContext?.Response.Cookies.Append("refresh_token", token.Id.ToString(), new CookieOptions {
            MaxAge = token.ExpirationDate - DateTime.Now,
            HttpOnly = true,
            Secure = true
        });
    }
}