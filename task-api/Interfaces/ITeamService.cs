using Microsoft.AspNetCore.Mvc;
using task_api.Models;

namespace task_api.Interfaces
{
    public interface ITeamService
    {
        public Task<ActionResult<List<Team>>> GetAsync();

        public Task<ActionResult<Team>> GetAsync(string name);

        public Task<ActionResult<Team>> GetUsingManagerIdAsync(string id);

        public Task<ActionResult<Team>> GetUsingProjectIdAsync(string id);

        public Task<ActionResult<Team>> GetUsingMemberIdAsync(string id);

        public Task<ActionResult<Team>> GetUsingIdAsync(string id);

        public Task<ActionResult<string>> CreateAsync(Team team);

        public Task UpdateAsync(string id, Team team);

        public Task RemoveAsync(string id);
    }
}
