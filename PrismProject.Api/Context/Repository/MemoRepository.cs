using Arch.EntityFrameworkCore.UnitOfWork;

namespace PrismProject.Api.Context.Repository
{
    public class MemoRepository : Repository<Memo>, IRepository<Memo>
    {
        public MemoRepository(PrismProjectContenxt dbContext) : base(dbContext)  // 备忘录仓储
        {
        }
    }
}
