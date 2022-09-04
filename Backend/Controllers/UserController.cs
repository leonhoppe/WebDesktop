using Backend.Entitys;
using Backend.Logic;
using Backend.LogicResults;
using Backend.Security;
using Backend.Security.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers; 

[ApiController]
[Route("users")]
public class UserController : ControllerBase {

    private readonly UserLogic _logic;
    private readonly ITokenContext _context;

    public UserController(UserLogic logic, ITokenContext context) {
        _logic = logic;
        _context = context;
    }

    [HttpGet]
    [Authorized(Permissions.ShowUsers)]
    public ActionResult<IEnumerable<User>> GetUsers() {
        return this.FromLogicResult(_logic.GetUsers());
    }

    [HttpPut("login")]
    public ActionResult<AccessToken> Login([FromBody] UserLogin login) {
        return this.FromLogicResult(_logic.Login(login));
    }

    [HttpPost("register")]
    public ActionResult<AccessToken> Register([FromBody] UserEditor editor) {
        return this.FromLogicResult(_logic.Register(editor));
    }

    [HttpGet("token")]
    [Authorized]
    public ActionResult<AccessToken> GetToken() {
        return this.FromLogicResult(_logic.GenerateToken(_logic.GetCurrentUserRefreshToken()));
    }

    [HttpGet("{userId}")]
    [Authorized(Permissions.ShowUsers)]
    public ActionResult<User> GetUser(Guid userId) {
        return this.FromLogicResult(_logic.GetUser(userId));
    }
    
    [HttpGet("self")]
    [Authorized]
    public ActionResult<User> GetOwnUser() {
        return this.FromLogicResult(_logic.GetUser(_context.UserId));
    }

    [HttpPut("{userId}")]
    [Authorized(Permissions.EditUsers)]
    public ActionResult EditUser(Guid userId, [FromBody] UserEditor editor) {
        return this.FromLogicResult(_logic.EditUser(userId, editor));
    }

    [HttpDelete("{userId}")]
    [Authorized(Permissions.DeleteUsers)]
    public ActionResult DeleteUser(Guid userId) {
        return this.FromLogicResult(_logic.DeleteUser(userId));
    }

    [HttpGet("{userId}/permissions")]
    [Authorized(Permissions.ShowUserPermissions)]
    public ActionResult<IEnumerable<string>> GetUserPermissions(Guid userId) {
        return this.FromLogicResult(_logic.GetPermissions(userId));
    }
    
    [HttpGet("{userId}/permissions/raw")]
    [Authorized(Permissions.ShowUserPermissions)]
    public ActionResult<IEnumerable<string>> GetUserPermissionsRaw(Guid userId) {
        return this.FromLogicResult(_logic.GetPermissionsRaw(userId));
    }

    [HttpPost("{userId}/permissions")]
    [Authorized(Permissions.EditUserPermissions, Permissions.EditOwnPermissions)]
    public ActionResult AddUserPermissions(Guid userId, [FromBody] string[] permissions) {
        return this.FromLogicResult(_logic.AddPermissions(userId, permissions));
    }
    
    [HttpPut("{userId}/permissions")]
    [Authorized(Permissions.EditUserPermissions, Permissions.EditOwnPermissions)]
    public ActionResult DeleteUserPermissions(Guid userId, [FromBody] string[] permissions) {
        return this.FromLogicResult(_logic.DeletePermissions(userId, permissions));
    }

    [HttpDelete("{userId}/logout")]
    [Authorized(Permissions.LogoutUsers)]
    public ActionResult Logout(Guid userId) {
        return this.FromLogicResult(_logic.Logout(userId));
    }

}