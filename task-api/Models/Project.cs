using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace task_api.Models
{
    public class Project
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("tasks")]
        public string[] Tasks { get; set; }

        [BsonElement("status")]
        public string[] Status { get; set; }

        [BsonElement("employees")]
        public string[] Employees { get; set; }
    }
}
