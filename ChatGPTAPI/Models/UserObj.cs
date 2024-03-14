using MongoDB.Bson;

public class User
{
    public ObjectId Id { get; set; } // MongoDB unique ID
    public string AssistantID { get; set; }
    public string ThreadID { get; set; }

}