using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Serilog;

namespace ServerOverflow.Database;

/// <summary>
/// Database controller
/// </summary>
public static class Controller {
    public static readonly IMongoCollection<Account> Accounts;
    public static readonly IMongoCollection<Invitation> Invitations;
    public static readonly IMongoCollection<Exclusion> Exclusions;
    //public static readonly IMongoCollection<Server> Servers;
    
    /// <summary>
    /// Initializes the MongoDB database
    /// </summary>
    static Controller() {
        Log.Information("Initializing MongoDB controller");
        var camelCaseConventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
        ConventionRegistry.Register("CamelCase", camelCaseConventionPack, _ => true);
        var client = new MongoClient(new MongoClientSettings {
            Server = new MongoServerAddress("localhost"),
            MaxConnectionPoolSize = 500
        });
        var database = client.GetDatabase("server-overflow");
        Accounts = database.GetCollection<Account>("accounts");
        Invitations = database.GetCollection<Invitation>("invitations");
        var matscan = client.GetDatabase("matscan");
        Exclusions = matscan.GetCollection<Exclusion>("exclusions");
        //Servers = database.GetCollection<Server>("servers");
    }
}