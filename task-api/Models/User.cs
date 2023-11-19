using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace task_api.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("passwordHash")]
        public byte[] PasswordHash { get; set; }

        [BsonElement("passwordSalt")]
        public byte[] PasswordSalt { get; set; }

        [BsonElement("role")]
        public int Role { get; set; }

        [BsonElement("team")]
        public string Team { get; set; }
    }
}
