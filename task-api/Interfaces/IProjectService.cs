using Microsoft.AspNetCore.Mvc;
using task_api.Models;

namespace task_api.Interfaces
{
    public interface IProjectService
    {
        public Task<ActionResult<List<Project>>> GetAsync();

        public Task<ActionResult<Project>> GetAsync(string name);

        public Task<ActionResult<Project>> GetUsingIdAsync(string id);

        public Task<ActionResult<Project>> GetUsingMemberIdAsync(string id);

        public Task<ActionResult<string>> CreateAsync(Project project);

        public Task UpdateAsync(string id, Project project);

        public Task RemoveAsync(string id);

        public Task RemoveUsingMemberIdAsync(string id);

    }
}
