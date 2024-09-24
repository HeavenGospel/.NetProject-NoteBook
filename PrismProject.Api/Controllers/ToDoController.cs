using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrismProject.Api.Context.Dtos;
using PrismProject.Api.Service;
using PrismProject.Shared.Parameters;

namespace PrismProject.Api.Controllers
{
    [ApiController]  // 添加特性
    [Route("api/[controller]/[action]")]  // 添加路由
    public class ToDoController:ControllerBase  // 待办事项控制器
    {
        private readonly IToDoService service;
        public ToDoController(IToDoService service)
        {
            this.service = service;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ApiResponse> Get(GetAndDelParameter parameter) => await service.GetSingleAsync(parameter);

        [AllowAnonymous]
        [HttpGet]
        public async Task<ApiResponse> GetAll([FromQuery] QueryParameter q) => await service.GetAllAsync(q);

        [AllowAnonymous]
        [HttpPost]
        public async Task<ApiResponse> Add([FromBody] ToDoDto model) => await service.AddAsync(model);

        [AllowAnonymous]
        [HttpPost]
        public async Task<ApiResponse> Update([FromBody] ToDoDto model) => await service.UpdateAsync(model);

        [AllowAnonymous]
        [HttpDelete]
        public async Task<ApiResponse> Delete(GetAndDelParameter parameter) => await service.DeleteAsync(parameter);

    }
}
