using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using task_api.Interfaces;
using task_api.Models;

namespace task_api.Services
{
    public class IssueService : IIssueService
    {
        private readonly IMongoCollection<Issue> issueCollection;
        private readonly IMongoDatabase issueDatabase;
        public IssueService(
        IOptions<DatabaseSettings> bookStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                bookStoreDatabaseSettings.Value.ConnectionString);

            this.issueDatabase = mongoClient.GetDatabase(
                bookStoreDatabaseSettings.Value.DatabaseName);

            this.issueCollection = this.issueDatabase.GetCollection<Issue>(
                bookStoreDatabaseSettings.Value.IssuesCollectionName);

        }

        public IMongoDatabase getDatabaseRef() => this.issueDatabase;

        public async Task<ActionResult<List<Issue>>> GetAsyncByUserId(string userId)
        {
            return await this.issueCollection.Find(issue => issue.Assignee.ToLower() == userId.ToLower()).ToListAsync();
        }


        public async Task<ActionResult<List<Issue>>> GetAsync() =>
        await this.issueCollection.Find(_ => true).ToListAsync();

        //public async Task<ActionResult<Issue>> GetAsync(string email) =>
            //await this.issueCollection.Find(x => x.Email.ToLower() == email.ToLower()).FirstOrDefaultAsync();


        public async Task<ActionResult<Issue>> GetUsingIdAsync(string id) =>
            await this.issueCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<string> CreateAsync(Issue issue)
        {
            await this.issueCollection.InsertOneAsync(issue);
            return issue.Id;

        }
            

        public async Task UpdateAsync(string id, Issue issue) =>
            await this.issueCollection.ReplaceOneAsync(x => x.Id == id, issue);

        public async Task RemoveAsync(string id) =>
            await this.issueCollection.DeleteOneAsync(x => x.Id == id);

        public async Task RemoveAllIssuesAssignedTo(string userId) =>
            await this.issueCollection.DeleteManyAsync(x => x.Assignee == userId);
    }
}
