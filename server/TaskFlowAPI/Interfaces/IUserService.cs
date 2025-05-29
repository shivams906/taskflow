using TaskFlowAPI.DTOs;

namespace TaskFlowAPI.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDto>> GetUsersAsync(string? search);
    }
}