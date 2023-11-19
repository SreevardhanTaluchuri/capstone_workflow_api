using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using System.IdentityModel.Tokens.Jwt;
using task_api.Dto;
using task_api.Interfaces;
using task_api.Models;

namespace task_api.Controllers
{
    public class ProjectsController : BaseApiController
    {
        private readonly IProjectService projectService;
        private readonly ITeamService teamService;

        public ProjectsController(IProjectService projectService , ITeamService teamService)
        {
            this.projectService = projectService;
            this.teamService = teamService;
        }

        [HttpGet("team/{id}")]
        public async Task<ActionResult<Project>> getProjectsinTeam(string id)
        {
            var teamFoundValue = await this.teamService.GetUsingIdAsync(id);
            var teamFound = teamFoundValue.Value;
            var projects = new List<Project> { };
            for(int i=0;i<teamFound.Projects.Length; i++)
            {
                var project = await this.projectService.GetUsingIdAsync(teamFound.Projects[i]);
                projects.Add(project.Value);
                
            }
            return Ok(projects);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Project>> updateProject(string id , Project project)
        {
            await this.projectService.UpdateAsync(id, project);
            return Ok(project);
        }

        [HttpGet()]
        public async Task<ActionResult<Project>> getProjects()
        {
            var re = Request;
            var headers = re.Headers;
            var jwt = headers.Authorization.ToString().Split(" ")[1];
            var jwtClaims = (new JwtSecurityTokenHandler().ReadToken(jwt) as JwtSecurityToken).Claims;
            var userId = jwtClaims.ElementAtOrDefault(1).Value;
            var role = jwtClaims.ElementAtOrDefault(2).Value;
            var teamId = jwtClaims.ElementAtOrDefault(3).Value;

            if(int.Parse(role) == 1 && teamId != null)
            {
            var projects = await this.projectService.GetUsingMemberIdAsync(userId);
                Project[] project = new Project[]{ projects.Value};
                
                return Ok(project);
            }


            if(int.Parse(role) == 2)
            {
                var teamFound = (await this.teamService.GetUsingManagerIdAsync(userId)).Value;
                var projects = new List<Project>();
                if(teamFound != null)
                for(int i=0;i<teamFound.Projects.Length; i++)
                {
                    var project = (await this.projectService.GetUsingIdAsync(teamFound.Projects[i])).Value;
                    projects.Add(project);
                }

                return Ok(projects);
            }

            if (int.Parse(role) == 3)
            {
                System.Diagnostics.Debug.WriteLine("ROLE");
                var projects = (await this.projectService.GetAsync()).Value;
                return Ok(projects);
            }

            if (teamId == null)
            {
                System.Diagnostics.Debug.WriteLine(userId);
                var project = (await this.projectService.GetUsingMemberIdAsync(userId)).Value;
                return Ok(project);
            }
            
            return Ok(new List<Project>());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Project>> deleteProject(string id)
        {
            var teamFound = (await this.teamService.GetUsingProjectIdAsync(id)).Value;
            var projects = new List<string>(teamFound.Projects);
            projects.Remove(id);

            teamFound.Projects = projects.ToArray();

            await this.teamService.UpdateAsync(teamFound.Id, teamFound);

            await this.projectService.RemoveAsync(id);

            return Ok();
        }


        [HttpPost()]
        public async Task<ActionResult<Project>> addProject(ProjectDto projectDto)
        {


                var project = new Project
                {
                    Name = projectDto.Name,
                    Status = new string[] { "TO DO", "IN PROGRESS", "DONE" },
                    Tasks = new string[0],
                    Employees = projectDto.Employees,
                };
                var projectId = await this.projectService.CreateAsync(project);
                
                var teamValue = await this.teamService.GetUsingIdAsync(projectDto.Team);
                var teamFound = teamValue.Value;
                if (teamFound.Projects == null || teamFound.Projects.Length == 0)
                {
                    teamFound.Projects = new string[] { projectId.Value };
                }
                else
                {
                    var projectsList = new List<string>(teamFound.Projects);
                    projectsList.Add(projectId.Value);
                    teamFound.Projects = projectsList.ToArray();
                }

                await this.teamService.UpdateAsync(projectDto.Team, teamFound);

                return Ok(projectId.ToJson());
        }
    }
}
