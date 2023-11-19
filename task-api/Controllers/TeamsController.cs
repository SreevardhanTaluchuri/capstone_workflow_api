using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.IdentityModel.Tokens.Jwt;
using task_api.Dto;
using task_api.Interfaces;
using task_api.Models;

namespace task_api.Controllers
{
    public class TeamsController : BaseApiController
    {
        private readonly ITeamService teamService;
        private readonly IUserService userService;
        private readonly IProjectService projectService;
        private readonly IIssueService issueService;

        public TeamsController(ITeamService teamService , IUserService userService , IProjectService projectService , IIssueService issueService)

        {
            this.teamService = teamService;
            this.userService = userService;
            this.projectService = projectService;
            this.issueService = issueService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Team>> getTeams(string id)
        {
            System.Diagnostics.Debug.WriteLine(id);
            var teams = (await this.teamService.GetUsingIdAsync(id)).Value;
            System.Diagnostics.Debug.WriteLine(teams);  
            return Ok(teams);
        }

        [HttpGet()]
        public async Task<ActionResult<Team>> getTeams()
        {
            var teams = await this.teamService.GetAsync();
            return Ok(teams.Value);
        }

        

        [HttpGet("member")]
        public async Task<ActionResult<Team>> getTeamOfMember()
        {
            var re = Request;
            var headers = re.Headers;
            var jwt = headers.Authorization.ToString().Split(" ")[1];
            var jwtClaims = (new JwtSecurityTokenHandler().ReadToken(jwt) as JwtSecurityToken).Claims;
            var userId = jwtClaims.ElementAtOrDefault(1).Value;

            var teamFound = await this.teamService.GetUsingMemberIdAsync(userId);

            return Ok(teamFound);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Team>> deleteTeam(string id)
        {
            var team = (await this.teamService.GetUsingIdAsync(id)).Value;

            var manager = (await this.userService.GetUsingIdAsync(team.Manager)).Value;
            manager.Team = "";
            manager.Role = 1;

            await this.userService.UpdateAsync(manager.Id, manager);

            for(int i=0;i<team.Employees.Length; i++)
            {
                var employee = (await this.userService.GetUsingIdAsync(team.Employees[i])).Value;
                employee.Team = "";

                await this.userService.UpdateAsync(employee.Id, employee);
            }
            for(int i=0;i<team.Projects.Length; i++)
            {
                await this.projectService.RemoveAsync(team.Projects[i]);
            }
            await this.teamService.RemoveAsync(id);
            return Ok();
        }
 
        [HttpPost()]
        public async Task<ActionResult<Team>> addTeam(TeamsDto teamDto)
        {

            var team = new Team
            {
                Name = teamDto.Name,
                Projects = new string[0],
                Manager = teamDto.Manager,
                Employees = teamDto.Employees,
            };
            
            var teamCreated = (await this.teamService.CreateAsync(team)).Value;
            
            var manager = await this.userService.GetUsingIdAsync(team.Manager);
            var managerValue = manager.Value;
            if (managerValue.Team != null)
            {
                await this.projectService.RemoveUsingMemberIdAsync(managerValue.Id);
                await this.issueService.RemoveAllIssuesAssignedTo(managerValue.Id);
                var teamOfManager = (await this.teamService.GetUsingMemberIdAsync(managerValue.Id)).Value;
                if(teamOfManager != null)
                {
                    if(teamOfManager.Employees != null)
                    {
                    var employeesList = new List<string>(teamOfManager.Employees);
                    employeesList.Remove(managerValue.Id);
                    teamOfManager.Employees = employeesList.ToArray();
                    }
                else
                {
                    teamOfManager.Employees = new string[0];
                }
                await this.teamService.UpdateAsync(teamOfManager.Id, teamOfManager);
                }
            }
            managerValue.Team = teamCreated;
            managerValue.Role = 2;
            await this.userService.UpdateAsync(managerValue.Id, managerValue);

            for (int i=0;i< team.Employees.Length; i++)
            {
                var user = await  this.userService.GetUsingIdAsync(team.Employees[i]);
                var userValue = user.Value;
                userValue.Team = team.Id;
                System.Diagnostics.Debug.WriteLine(team.Id);
                var project = (await this.projectService.GetUsingMemberIdAsync(team.Employees[i])).Value;
                if(project != null)
                {
                    await this.issueService.RemoveAllIssuesAssignedTo(userValue.Id);
                    await this.projectService.RemoveAsync(project.Id);
                    await this.userService.UpdateAsync(userValue.Id, userValue);
                }
            }
            var projectsId = new List<string>();
            for(int i=0;i<teamDto.Projects.Length; i++)
            {
                var newProject = teamDto.Projects[i];
                var project = new Project
                {
                    Name = newProject.Name,
                    Status = new string[] { "TO DO", "IN PROGRESS", "DONE" },
                    Tasks = new string[0],
                    Employees = newProject.Employees,
                };
                var projectId = (await this.projectService.CreateAsync(project)).Value;
                projectsId.Add(projectId);
            }

            team.Projects = projectsId.ToArray();
            await this.teamService.UpdateAsync(teamCreated, team);

            return Ok();
        }
    }
}
