using task_api.Models;

namespace task_api.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
