using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
public class UserObj
{
    [Required(ErrorMessage = "The username is required and cannot be empty.")]
    [MinLength(5, ErrorMessage = "The username must be of minimum length 5 characters.")]
    public string Username { get; set; }

    [Required(ErrorMessage = "The username is required and cannot be empty.")]
    [MinLength(5, ErrorMessage = "The username must be of minimum length 5 characters.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Please provide a valid email address.")]
    public string Email { get; set; }
    public ObjectId Id { get; set; } // MongoDB unique ID

}