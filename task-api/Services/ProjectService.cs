using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using task_api.Interfaces;
using task_api.Models;

namespace task_api.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IMongoCollection<Project> projectService;

        public ProjectService(
        IOptions<DatabaseSettings> bookStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                bookStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                bookStoreDatabaseSettings.Value.DatabaseName);

            this.projectService = mongoDatabase.GetCollection<Project>(
                bookStoreDatabaseSettings.Value.ProjectsCollectionName);

        }
        public async Task<ActionResult<List<Project>>> GetAsync() =>
        await this.projectService.Find(_ => true).ToListAsync();

        public async Task<ActionResult<Project>> GetAsync(string name) =>
            await this.projectService.Find(x => x.Name.ToLower() == name.ToLower()).FirstOrDefaultAsync();


        public async Task<ActionResult<Project>> GetUsingIdAsync(string id) =>
            await this.projectService.Find(x => x.Id == id).FirstOrDefaultAsync();


        public async Task<ActionResult<string>> CreateAsync(Project project)
        {
            await this.projectService.InsertOneAsync(project);
            return project.Id;
        }

        public async Task<ActionResult<Project>> GetUsingMemberIdAsync(string id) =>
    await this.projectService.Find(x => x.Employees.Any(x => x.Equals(id))).FirstOrDefaultAsync();

        public async Task UpdateAsync(string id, Project project) =>
            await this.projectService.ReplaceOneAsync(x => x.Id == id, project);

        public async Task RemoveAsync(string id) =>
            await this.projectService.DeleteOneAsync(x => x.Id == id);

        public async Task RemoveUsingMemberIdAsync(string id) =>
            await this.projectService.DeleteManyAsync(x => x.Employees.Any(emp => emp == id));
    }
}
