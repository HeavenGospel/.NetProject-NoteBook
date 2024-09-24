using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrismProject.Api.Context.Dtos;
using PrismProject.Api.Service;

namespace PrismProject.Api.Controllers
{
    [ApiController]  // 添加特性
    [Route("api/[controller]/[action]")]  // 添加路由
    public class LoginController :ControllerBase  // 备忘录控制器
    {
        private readonly ILoginService service;
        public LoginController(ILoginService service)
        {
            this.service = service;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ApiResponse> Login([FromBody] UserDto parameter) => await service.LoginAsync(parameter.Account, parameter.Password);
        
        [AllowAnonymous]
        [HttpPost]
        public async Task<ApiResponse> Register([FromBody] UserDto user) => await service.RegisterAsync(user);

    }
}
