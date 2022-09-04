namespace Backend.Options; 

public class UserMessageOptions : OptionsFromConfiguration {
    public override string Position => "Messages:Users";
    
    public string NotFound { get; set; }
    public string InvalidEditData { get; set; }
    public string InvalidRegisterData { get; set; }
    public string WrongPassword { get; set; }
    public string UsernameOrEmailExist { get; set; }
    public string InvalidRefreshToken { get; set; }
}