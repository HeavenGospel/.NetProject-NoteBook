using PrismProject.Common.Models;
using System.Threading.Tasks;

namespace PrismProject.Service
{
    public interface ILoginService  // 登陆注册服务(接口)
    {
        Task<BaseResponse<UserDto>> LoginAsync(UserDto dto);
        Task<BaseResponse> RegisterAsync(UserDto dto);
    }
}
