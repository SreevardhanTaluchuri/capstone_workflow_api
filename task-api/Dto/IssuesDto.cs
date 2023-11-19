using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace task_api.Dto
{
    public class IssuesDto
    {
        //public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Due_date { get; set; }

        public string Priority { get; set; }

        public string Project { get; set; }

        public string Status { get; set; }

        public string Assignee { get; set; }

        public int Points { get; set; }
        public string Reporter { get; set; }

        public string Type { get; set; }

    }
}
