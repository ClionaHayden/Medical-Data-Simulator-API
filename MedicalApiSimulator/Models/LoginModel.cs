/// <summary>
/// Model representing user login credentials.
/// </summary>
public class LoginModel
{
    /// <summary>
    /// The username of the user trying to authenticate.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The password of the user.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}