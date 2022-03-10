using MongoDB.Driver;

namespace ProcHost.Model;

public class MongoDbLogger : ILogger
{
    public MongoDbLogger(string connectionString, string databaseName, string collectionName)
    {
        ConnectionString = connectionString;
        DatabaseName = databaseName;
        CollectionName = collectionName;

        _client = new MongoClient(this.ConnectionString);
        _db = _client.GetDatabase(this.DatabaseName);
        _collection = _db.GetCollection<dynamic>(this.CollectionName);
    }

    public string ConnectionString { get; }
    public string DatabaseName { get; }
    public string CollectionName { get; }

    private MongoClient _client;
    private IMongoDatabase _db;
    private IMongoCollection<dynamic> _collection;

    public void LogOutput(string message, ChildProcess process)
    {
        _collection.InsertOne(new
        {
            Time = DateTime.Now,
            Machine = Environment.MachineName,
            TaskName = process.Name,
            Message = message,
            Process = process.Process
        });
    }
}
