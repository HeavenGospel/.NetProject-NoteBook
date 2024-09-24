using PrismProject.Common.Models;
using System.Threading.Tasks;

namespace PrismProject.Service
{
    public class LoginService : ILoginService  // 登陆注册服务
    {
        private readonly HttpRestClient client;
        private readonly string serviceName = "Login";
        public LoginService(HttpRestClient client)
        {
            this.client = client;
        }
        public async Task<BaseResponse<UserDto>> LoginAsync(UserDto dto)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"api/{serviceName}/Login";
            request.Parameter= dto;
            return await client.ExecuteAsync<UserDto>(request);
        }
        public async Task<BaseResponse> RegisterAsync(UserDto dto)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"api/{serviceName}/Register";
            request.Parameter = dto;
            return await client.ExecuteAsync(request);
        }
    }
}
