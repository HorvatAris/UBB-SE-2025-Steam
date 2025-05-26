namespace BusinessLayer.Models.Login;

public class LoginResponse
{
    public User User { get; set; }
    public string Token { get; set; }
    public UserWithSessionDetails UserWithSessionDetails { get; set; }
}