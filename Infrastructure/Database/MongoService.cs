using MongoDB.Driver;

namespace portfolio_service.Services
{
    public interface IMongoService
    {
        IMongoDatabase GetDatabase();
    }

    public class MongoService : IMongoService
    {
        private readonly IMongoDatabase _database;

        public MongoService(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MongoDB") 
                ?? "mongodb://localhost:27017";
            var databaseName = configuration["MongoDB:DatabaseName"] ?? "WhalePortfolio";
            
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoDatabase GetDatabase()
        {
            return _database;
        }
    }
}
