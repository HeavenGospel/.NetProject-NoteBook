using PrismProject.Common.Models;

namespace PrismProject.Service
{
    public class TodoService : BaseService<TodoDto>, ITodoService  // 待办事项服务
    {
        public TodoService(HttpRestClient client, string serviceName = "ToDo") : base(client, serviceName)
        {
            
        }
    }
}
