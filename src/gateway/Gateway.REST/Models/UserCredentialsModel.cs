namespace QuickChat.Gateway.REST.Models;

public record UserCredentialsModel(UserModel User, string AccessToken);
