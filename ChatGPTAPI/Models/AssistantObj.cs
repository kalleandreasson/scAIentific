using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
public class AssistantObj
{
    public string Username { get; set; }

    public ObjectId Id { get; set; } // MongoDB unique ID
    public string AssistantID { get; set; }
    public string ThreadID { get; set; }
    public string FileID { get; set; }

}