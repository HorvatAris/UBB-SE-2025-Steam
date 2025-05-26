namespace BusinessLayer.Models.Login;

public class LoginRequest
{
    public string EmailOrUsername { get; set; }
    public string Password { get; set; }
}