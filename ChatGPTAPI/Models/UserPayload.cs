namespace ChatGPTAPI.Models;

using System.ComponentModel.DataAnnotations;

public class UserPayload
{
    public string Username { get; set; }
    public string Password { get; set; }
}