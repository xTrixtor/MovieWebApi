public class UserDto
{
    public int id { get; set; } = 0; 
    public string username { get; set; } = "";
    public byte[] password_salt { get; set; }
    public byte[] password_hash { get; set; }
    public Role role_id { get; set; }
}

public enum Role
{
    Guest = 1,
    User = 2,
    Admin = 3
}   