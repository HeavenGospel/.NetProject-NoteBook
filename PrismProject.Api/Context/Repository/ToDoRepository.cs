using Arch.EntityFrameworkCore.UnitOfWork;

namespace PrismProject.Api.Context.Repository
{
    public class ToDoRepository : Repository<ToDo>, IRepository<ToDo>
    {
        public ToDoRepository(PrismProjectContenxt dbContext) : base(dbContext)  // 待办事项仓储
        {
        }
    }
}
