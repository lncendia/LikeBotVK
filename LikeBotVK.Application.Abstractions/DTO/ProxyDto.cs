namespace LikeBotVK.Application.Abstractions.DTO;

public class ProxyDto
{
    public ProxyDto(string host, int port, string login, string password, int id)
    {
        Host = host;
        Port = port;
        Login = login;
        Password = password;
        Id = id;
    }

    public int Id { get; }

    public string Host { get; }

    public int Port { get; }

    public string Login { get; }
    public string Password { get; }
}