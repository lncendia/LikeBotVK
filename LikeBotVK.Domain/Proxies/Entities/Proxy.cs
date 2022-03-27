using System.Text.RegularExpressions;
using LikeBotVK.Domain.Proxies.Exceptions;

namespace LikeBotVK.Domain.Proxies.Entities;

public class Proxy
{
    public Proxy(string host, int port, string login, string password)
    {
        Host = host;
        Login = login;
        Password = password;
        Port = port;
    }

    public int Id { get; set; }

    private string _host = null!;

    public string Host
    {
        get => _host;
        set
        {
            if (Regex.IsMatch(value, @"/((?:[a-z0-9\-]*\.){1,}[a-z0-9\-]*)/")) _host = value;
            else throw new InvalidHostFormatException(value);
        }
    }

    private int _port;

    public int Port
    {
        get => _port;
        set
        {
            if (value is >= 0 and <= 65536) _port = value;
            else throw new InvalidPortFormatException(value);
        }
    }

    public string Login { get; set; }
    public string Password { get; set; }
}