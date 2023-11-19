using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace task_api.Models
{
    public class Team
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("manager")]
        public string Manager { get; set; }

        [BsonElement("employees")]
        public string[] Employees { get; set; }

        [BsonElement("projects")]
        public string[] Projects { get; set; }
    }
}
