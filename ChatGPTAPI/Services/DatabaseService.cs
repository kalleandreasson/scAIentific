using System.ComponentModel;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

public class MongoDBService
{
    private readonly IMongoCollection<User> _users;
    private readonly string _dbConnectionString;

    public MongoDBService(IOptions<DatabaseServiceOptions> options)
    {
        var settings = options.Value ?? throw new ArgumentNullException(nameof(options));
        var client = new MongoClient(settings.DatabaseConnectString); // Uses the Atlas connection string
        var database = client.GetDatabase("YourDatabaseNameHere");
        _users = database.GetCollection<User>("Users");
    }

    public async Task CreateUserAsync(User user)
    {
        await _users.InsertOneAsync(user);
    }

}