using MongoDB.Bson.Serialization.Attributes;

namespace BankApi;

public class CustomerDocument
{
    [BsonId]
    public string Username { get; set; }

    public string Password { get; set; }

    public List<string> AccountNumbers { get; set; } = new List<string>();
}