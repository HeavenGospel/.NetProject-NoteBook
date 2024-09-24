using PrismProject.Api.Context.Dtos;

namespace PrismProject.Api.Service
{
    public interface ILoginService
    {
        Task<ApiResponse> LoginAsync(string Account, string Password);
        Task<ApiResponse> RegisterAsync(UserDto user);
    }
}
