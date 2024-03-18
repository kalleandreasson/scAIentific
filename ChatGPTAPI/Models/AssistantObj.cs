using MongoDB.Bson;

public class AssistantObj
{
    public ObjectId Id { get; set; } // MongoDB unique ID
    public string AssistantID { get; set; }
    public string ThreadID { get; set; }
    public string Username { get; set; }

}