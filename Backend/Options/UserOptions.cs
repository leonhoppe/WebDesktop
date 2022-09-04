namespace Backend.Options; 

public class UserOptions : OptionsFromConfiguration {
    public override string Position => "Users";
    
    public string[] DefaultPermissions { get; set; }
}