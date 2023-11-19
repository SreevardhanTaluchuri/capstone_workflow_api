using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using task_api.Interfaces;
using task_api.Models;
using task_api.Services;

namespace task_api.Controllers
{
    public class UserController : BaseApiController
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> getUser(string id)
        {
            var user = (await this.userService.GetUsingIdAsync(id)).Value;
            user = new User
            {
                Email = user.Email,
                Name = user.Name,
                Role = user.Role,
                Team = user.Team,
                Id = user.Id,
            };

            return Ok(user);
        }

        [HttpGet()]
        public async Task<ActionResult<User>> getUsers()
        {
            var users = await this.userService.GetAsync();
            
            var usersList = new List<User>();

            users.Value.ForEach((item) =>
            {
                var user = new User
                {
                    Id = item.Id,
                    Name = item.Name,
                    Email = item.Email,
                    Team = item.Team,
                    Role = item.Role,
                };
                usersList.Add(user);
            });

            return Ok(usersList);
        }
    }
}
