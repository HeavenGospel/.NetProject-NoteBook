using Arch.EntityFrameworkCore.UnitOfWork.Collections;
using PrismProject.Common.Models;
using System.Threading.Tasks;

namespace PrismProject.Service  // 服务基类(接口)
{
    public interface IBaseService<TEntity> where TEntity : class
    {
        Task<BaseResponse<TEntity>> AddAsync(TEntity entity);
        Task<BaseResponse<TEntity>> UpdateAsync(TEntity entity);
        Task<BaseResponse> DeleteAsync(GetAndDelParameter parameter);
        Task<BaseResponse<TEntity>> GetFirstOrDefaultAsync(GetAndDelParameter parameter);
        Task<BaseResponse<PagedList<TEntity>>> GetAllAsync(QueryParameter parameter);
    }
}
