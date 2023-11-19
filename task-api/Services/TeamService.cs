using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using task_api.Interfaces;
using task_api.Models;

namespace task_api.Services
{
    public class TeamService : ITeamService
    {
        private readonly IMongoCollection<Team> teamsCollection;
        public TeamService(
        IOptions<DatabaseSettings> bookStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                bookStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                bookStoreDatabaseSettings.Value.DatabaseName);

            this.teamsCollection = mongoDatabase.GetCollection<Team>(
                bookStoreDatabaseSettings.Value.TeamsCollectionName);

        }
        public async Task<ActionResult<List<Team>>> GetAsync() =>
        await this.teamsCollection.Find(_ => true).ToListAsync();

        public async Task<ActionResult<Team>> GetAsync(string name) =>
            await this.teamsCollection.Find(x => x.Name.ToLower() == name.ToLower()).FirstOrDefaultAsync();

        public async Task<ActionResult<Team>> GetUsingIdAsync(string id) =>
            await this.teamsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();


        public async Task<ActionResult<Team>> GetUsingManagerIdAsync(string id) =>
            await this.teamsCollection.Find(x => x.Manager == id).FirstOrDefaultAsync();


        public async Task<ActionResult<Team>> GetUsingMemberIdAsync(string id) =>
            await this.teamsCollection.Find(x => x.Employees.Any(member => member == id)).FirstOrDefaultAsync();

        public async Task<ActionResult<Team>> GetUsingProjectIdAsync(string id) =>
    await this.teamsCollection.Find(x => x.Projects.Any(member => member == id)).FirstOrDefaultAsync();

        public async Task<ActionResult<string>> CreateAsync(Team team)
        {
            await this.teamsCollection.InsertOneAsync(team);
            return team.Id;
        }

        public async Task UpdateAsync(string id, Team team) =>
            await this.teamsCollection.ReplaceOneAsync(x => x.Id == id, team);

        public async Task RemoveAsync(string id) =>
            await this.teamsCollection.DeleteOneAsync(x => x.Id == id);
    }
}
