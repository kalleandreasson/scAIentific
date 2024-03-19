using System.ComponentModel;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

public class MongoDBService
{
    private readonly IMongoCollection<AssistantObj> _users;
    private readonly string _dbConnectionString;

    public MongoDBService(IOptions<DatabaseServiceOptions> options)
    {
        var settings = options.Value ?? throw new ArgumentNullException(nameof(options));
        var client = new MongoClient(settings.DatabaseConnectString); // Uses the Atlas connection string
        var database = client.GetDatabase("YourDatabaseNameHere");
        _users = database.GetCollection<AssistantObj>("AssistantsObj");
    }

    public async Task SaveAssistantAsync(AssistantObj assistantObject)
    {
        await _users.InsertOneAsync(assistantObject);
    }

    public async Task<AssistantObj> GetSingletonUser()
{
        var filter = Builders<AssistantObj>.Filter.Eq(user => user.Username, "singletonUser");
        return await _users.Find(filter).FirstOrDefaultAsync();
}

public async Task<AssistantObj> GetAssistantByAssistantIDAsync(string assistantID)
{
    // Build the filter based on the AssistantID
    var filter = Builders<AssistantObj>.Filter.Eq(assistant => assistant.AssistantID, assistantID);

    // Attempt to find the AssistantObj in the collection
    return await _users.Find(filter).FirstOrDefaultAsync();
}

public async Task<AssistantObj> GetUserIfExistsAsync(string username)
{
    // Build the filter based on the username
    var filter = Builders<AssistantObj>.Filter.Eq(user => user.Username, username);

    // Attempt to find the user in the collection
    var user = await _users.Find(filter).FirstOrDefaultAsync();

    // This will return the user if found, or null if not found
    return user;
}

}