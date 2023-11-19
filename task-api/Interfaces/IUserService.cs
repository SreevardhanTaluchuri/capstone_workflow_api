using Microsoft.AspNetCore.Mvc;
using task_api.Models;

namespace task_api.Interfaces
{
    public interface IUserService
    {
        public Task<ActionResult<List<User>>> GetAsync();

        public Task<ActionResult<User>> GetAsync(string email);

        public Task<ActionResult<User>> GetUsingIdAsync(string id);

        public Task<ActionResult<string>> CreateAsync(User user);

        public Task UpdateAsync(string id, User user);

        public Task RemoveAsync(string id);
    }
}
