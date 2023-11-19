namespace task_api.Models
{
    public class DatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string UsersCollectionName { get; set; } = null!;

        public string IssuesCollectionName { get; set; } = null!;

        public string ProjectsCollectionName { get; set; } = null!;

        public string TeamsCollectionName { get; set; } = null!;
    }


}
