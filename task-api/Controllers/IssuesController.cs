using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using task_api.Dto;
using task_api.Interfaces;
using task_api.Models;

namespace task_api.Controllers
{
    public class IssuesController : BaseApiController
    {
        private readonly IIssueService issueService;
        private readonly IUserService userService;
        private readonly IProjectService projectService;
        private readonly ITeamService teamService;

        public IssuesController(IIssueService issueService , IUserService userService , IProjectService projectService , ITeamService teamService)
        {
            this.issueService = issueService;
            this.userService = userService;
            this.projectService = projectService;
            this.teamService = teamService;
        }

        [HttpPost()]
        public async Task<ActionResult<Issue>> addIssue(IssuesDto issueDto)
        {
            var re = Request;
            var headers = re.Headers;
            var jwt = headers.Authorization.ToString().Split(" ")[1];
            var jwtClaims = (new JwtSecurityTokenHandler().ReadToken(jwt) as JwtSecurityToken).Claims;
            var userId = jwtClaims.ElementAtOrDefault(1).Value;
            var issue = new Issue
            {
                Title = issueDto.Title,
                Created_by = userId,
                Description = issueDto.Description,
                Due_date = issueDto.Due_date,
                Status = issueDto.Status,
                Priority = issueDto.Priority,
                Assignee = issueDto.Assignee,
                Reporter = issueDto.Reporter,
                Points = issueDto.Points,
                Project = issueDto.Project,
                Type = issueDto.Type,
            };
            var issueCreatedId = await this.issueService.CreateAsync(issue);
            var projectFound = await this.projectService.GetUsingIdAsync(issue.Project);
            var project = projectFound.Value;
            if(project.Tasks == null || project.Tasks.Length == 0)
            {
                project.Tasks = new string[] { issueCreatedId };
            }
            else
            {
                var tasksList = new List<string>(project.Tasks);
                tasksList.Add(issueCreatedId);
                project.Tasks = tasksList.ToArray();
            }
            await this.projectService.UpdateAsync(project.Id, project);
            return Ok();

        }

        [HttpGet()]
        public async Task<ActionResult<List<Issue>>> getIssues()
        {
            var re = Request;
            var headers = re.Headers;
            var jwt = headers.Authorization.ToString().Split(" ")[1];
            var jwtClaims = (new JwtSecurityTokenHandler().ReadToken(jwt) as JwtSecurityToken).Claims;
            var userId = jwtClaims.ElementAtOrDefault(1).Value;
            var role =  jwtClaims.ElementAtOrDefault(2).Value;
            var teamId = jwtClaims.ElementAtOrDefault(3).Value;


            if (int.Parse(role) > 2)
            {
                var issues = (await this.issueService.GetAsync()).Value;
                return Ok(issues);
            }


            if (teamId != "")
            {
                var teamValue = await this.teamService.GetUsingIdAsync(teamId);
                var teamFound = teamValue.Value;
                if (int.Parse(role) == 1)
                {
                    var projects = new List<Project>();
                    for (int i = 0; i < teamFound.Projects.Length; i++)
                    {
                        var projectId = teamFound.Projects[i];
                        var projectFound = (await this.projectService.GetUsingIdAsync(projectId)).Value;
                        for (int j = 0; j < projectFound.Employees.Length; j++)
                        {
                            if (projectFound.Employees[j] == userId)
                            {
                                projects.Add(projectFound);
                            }
                        }
                    }
                    var issues = new List<Issue>();
                    for(int i=0;i<projects.Count; i++)
                    {
                        var project = projects[i];
                        if (project.Id != null)
                        {
                            System.Diagnostics.Debug.WriteLine("Issue name", project.Id);
                            for (int j = 0; j < project.Tasks.Length; j++)
                            {
                                var issue = (await this.issueService.GetUsingIdAsync(project.Tasks[j])).Value;
                                issues.Add(issue);
                            }
                        }
                    }
                            return Ok(issues);
                }
                else if (int.Parse(role) > 1)
                {
                    var issues = new List<Issue>();
                    System.Diagnostics.Debug.WriteLine("Role-2");
                    if (teamFound != null)
                    {
                        for (int i = 0; i < teamFound.Projects.Length; i++)
                        {
                            var projectId = teamFound.Projects[i];
                            var projectFound = (await this.projectService.GetUsingIdAsync(projectId)).Value;

                            for (int j = 0; j < projectFound.Tasks.Length; j++)
                            {
                                var issue = (await this.issueService.GetUsingIdAsync(projectFound.Tasks[j])).Value;
                                issues.Add(issue);


                            }
                        }
                    }
                    return Ok(issues);
                }
            }
            else
            {
                var issues = (await this.issueService.GetAsyncByUserId(userId)).Value;
                return Ok(issues);
            }
            
            return Ok(new List<Issue>());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<Issue>>> updateIssue(string id)
        {
            var issue = (await this.issueService.GetUsingIdAsync(id)).Value;
            return Ok(issue);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<List<Issue>>> updateIssue(string id,Issue issue)
        {
            await this.issueService.UpdateAsync(id, issue);
            return Ok();
        }
    }
}
