namespace BusinessLayer.Models.Register;

public class RegisterResponse
{
    public User User { get; set; }
    public string Token { get; set; }
    public UserWithSessionDetails UserWithSessionDetails { get; set; }
}