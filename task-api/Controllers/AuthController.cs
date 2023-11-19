using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using task_api.Dto;
using task_api.Interfaces;
using task_api.Models;

namespace task_api.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly IUserService userService;
        private readonly ITokenService tokenService;
        private readonly IProjectService projectService;

        public AuthController(IUserService userService, ITokenService tokenService , IProjectService projectService)
        {
            this.userService = userService;
            this.tokenService = tokenService;
            this.projectService = projectService;
        }
        [HttpPost("register")]
        public async Task<ActionResult<MessageDto>> Register(UserSignUpDto signUpDto)
        {
            if (await this.UserExists(signUpDto.Email)) return BadRequest("Email is already taken");
            using var hma = new HMACSHA512();

            var user = new User
            {
                Name = signUpDto.Name,
                Email = signUpDto.Email,
                PasswordHash = hma.ComputeHash(Encoding.UTF8.GetBytes(signUpDto.Password)),
                PasswordSalt = hma.Key,
                Role = 1,
                Team = string.Empty
            };
            var userId = (await this.userService.CreateAsync(user)).Value;

            var project = new Project
            {
                Name = user.Name,
                Tasks = new string[0],
                Employees = new string[1] { userId },
                Status = new string[3] {"TO DO" , "IN PROGRESS" , "DONE"}
            };

            await this.projectService.CreateAsync(project);

            var message = new MessageDto
            {
                message = "User created succesfully"
            };

            return Ok(message);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserLoginDto loginDto)
        {
            var userFound = await this.userService.GetAsync(loginDto.Email);
            
            if (userFound.Value == null) return Unauthorized("inavlid email!");
            else
            {
                using var hma = new HMACSHA512(userFound.Value.PasswordSalt);
                var computedHash = hma.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != userFound.Value.PasswordHash[i]) return Unauthorized("Invalid password");
                }

                var message = new MessageDto
                {
                    message = this.tokenService.CreateToken(userFound.Value)
                };

                return Ok(message);
            }
        }

        private async Task<bool> UserExists(string email)
        {
            
            var userFound = await this.userService.GetAsync(email);
            if (userFound.Value == null) return false;
            return true;
        }
    }
}
