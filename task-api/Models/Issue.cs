using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace task_api.Models
{
    public class Issue
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("created_by")]
        public string Created_by { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("due_date")]
        public string Due_date { get; set; }

        [BsonElement("priority")]
        public string Priority { get; set; }

        [BsonElement("status")]
        public string Status { get; set; }

        [BsonElement("assignee")]
        public string Assignee { get; set; }

        [BsonElement("points")]
        public int Points { get; set; }

        [BsonElement("reporter")]
        public string Reporter { get; set; }

        [BsonElement("project")]
        public string Project { get; set; }

        [BsonElement("type")]
        public string Type { get; set; }
    }
}
