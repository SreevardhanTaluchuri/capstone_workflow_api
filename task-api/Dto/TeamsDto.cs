namespace task_api.Dto
{
    public class TeamsDto
    {
        public string Name { get; set; }

        public string Manager { get; set; }

        public string[] Employees { get; set; }

        public AddTeamProjectDto[] Projects { get; set; }
    }

    public class AddTeamProjectDto
    {
        public string Name { get; set; }
        public string[] Employees { get; set; }
    }
}
