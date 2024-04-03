using System.ComponentModel;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

public class MongoDBService
{
    private readonly IMongoCollection<AssistantObj> _assistants;
    private readonly string _dbConnectionString;

    public MongoDBService(IOptions<DatabaseServiceOptions> options)
    {
        var settings = options.Value ?? throw new ArgumentNullException(nameof(options));
        var client = new MongoClient(settings.DatabaseConnectString); // Uses the Atlas connection string
        var database = client.GetDatabase("YourDatabaseNameHere");
        _assistants = database.GetCollection<AssistantObj>("AssistantsObj");
    }

    public async Task SaveAssistantAsync(AssistantObj assistantObject)
    {
        await _assistants.InsertOneAsync(assistantObject);
    }

    public async Task DeleteUserAssistantDetailsAsync(string username)
    {
        // Build the filter to find the specific user by username
        var filter = Builders<AssistantObj>.Filter.Eq(user => user.Username, username);

        // Perform the delete operation on the first matching document
        var result = await _assistants.DeleteOneAsync(filter);

        if (result.DeletedCount == 0)
        {
            throw new KeyNotFoundException($"User '{username}' not found or already deleted.");
        }

        Console.WriteLine($"Deleted assistant details for user: {username}");
    }


    public async Task<AssistantObj> GetAssistantByAssistantIDAsync(string assistantID)
    {
        // Build the filter based on the AssistantID
        var filter = Builders<AssistantObj>.Filter.Eq(assistant => assistant.AssistantID, assistantID);

        // Attempt to find the AssistantObj in the collection
        return await _assistants.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<AssistantObj> GetUserIfExistsAsync(string username)
    {
        // Build the filter based on the username
        var filter = Builders<AssistantObj>.Filter.Eq(user => user.Username, username);

        // Attempt to find the user in the collection
        var user = await _assistants.Find(filter).FirstOrDefaultAsync();

        // This will return the user if found, or null if not found
        return user;
    }

    public async Task<AssistantObj> ListAllAndReturnFirstAsync()
    {
        var allAssistants = await _assistants.Find(_ => true).ToListAsync();

        foreach (var assistant in allAssistants)
        {
            // Print out each AssistantObj (for example, to the console)
            Console.WriteLine($"AssistantID: {assistant.AssistantID}, ThreadID: {assistant.ThreadID}, Username: {assistant.Username}");
        }

        // Return the first AssistantObj in the list, or null if the list is empty
        return allAssistants.FirstOrDefault();
    }

    public async Task DeleteAllAssistantsAsync()
    {
        // Use an empty filter to match all documents
        var filter = Builders<AssistantObj>.Filter.Empty;

        // Delete all documents in the collection
        await _assistants.DeleteManyAsync(filter);
    }

    public async Task<List<string>> GetAllThreadIDsAsync()
    {
        var filter = Builders<AssistantObj>.Filter.Empty;
        var projection = Builders<AssistantObj>.Projection.Include("ThreadID");
        var threadIDsCursor = await _assistants.Find(filter).Project<AssistantObj>(projection).ToListAsync();

        // Assuming ThreadID is a string, extract just the ThreadID from each AssistantObj
        var threadIDs = threadIDsCursor.Select(assistant => assistant.ThreadID).ToList();

        return threadIDs;
    }

    public async Task ReplaceFileIdForUserAsync(string username, string newFileId)
    {
        // Build the filter to find the specific user by username
        var filter = Builders<AssistantObj>.Filter.Eq(user => user.Username, username);

        // Define the update operation to set the new FileId
        var update = Builders<AssistantObj>.Update.Set(user => user.FileID, newFileId);

        // Perform the update operation on the first matching document
        await _assistants.UpdateOneAsync(filter, update);
    }

    public async Task UpdateUserFieldsAsync(string username, string newAssistantID, string newFileID, string newThreadID)
    {
        // Build the filter to find the specific user by username
        var filter = Builders<AssistantObj>.Filter.Eq(user => user.Username, username);

        // Define the update operation to set the new values for AssistantID, ThreadID, and FileID
        var update = Builders<AssistantObj>.Update
            .Set(user => user.AssistantID, newAssistantID)
            .Set(user => user.ThreadID, newThreadID)
            .Set(user => user.FileID, newFileID);

        // Exclude the Username field from the update
        update = update.Unset(user => user.Username);

        // Perform the update operation on the first matching document
        await _assistants.UpdateOneAsync(filter, update);
    }




}