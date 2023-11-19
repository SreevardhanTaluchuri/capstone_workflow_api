using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using task_api.Models;

namespace task_api.Interfaces
{
    public interface IIssueService
    {
        public Task<ActionResult<List<Issue>>> GetAsync();

        public Task<ActionResult<List<Issue>>> GetAsyncByUserId(string userId);

        //public Task<ActionResult<Issue>> GetAsync(string email);
        public IMongoDatabase getDatabaseRef();
        public Task<ActionResult<Issue>> GetUsingIdAsync(string id);

        public Task<string> CreateAsync(Issue issue);

        public Task UpdateAsync(string id, Issue issue);

        public Task RemoveAsync(string id);

        public Task RemoveAllIssuesAssignedTo(string userId);
    }
}
