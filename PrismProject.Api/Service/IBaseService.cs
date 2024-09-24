using PrismProject.Shared.Parameters;

namespace PrismProject.Api.Service
{
    public interface IBaseService<T>
    {
        Task<ApiResponse> GetAllAsync(QueryParameter parameter);
        Task<ApiResponse> GetSingleAsync(GetAndDelParameter parameter);
        Task<ApiResponse> AddAsync(T model);
        Task<ApiResponse> UpdateAsync(T model);
        Task<ApiResponse> DeleteAsync(GetAndDelParameter parameter);
    }
}
