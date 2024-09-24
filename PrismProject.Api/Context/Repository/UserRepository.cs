using Arch.EntityFrameworkCore.UnitOfWork;

namespace PrismProject.Api.Context.Repository
{
    public class UserRepository : Repository<User>, IRepository<User>
    {
        public UserRepository(PrismProjectContenxt dbContext) : base(dbContext)
        {

        }
    }
}