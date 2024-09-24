using Arch.EntityFrameworkCore.UnitOfWork.Collections;
using PrismProject.Common.Models;
using System.Threading.Tasks;

namespace PrismProject.Service  // 服务基类
{
    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : class
    {
        private readonly HttpRestClient client;
        private readonly string serviceName;

        public BaseService(HttpRestClient client, string serviceName)
        {
            this.client = client;
            this.serviceName = serviceName;
        }
        public async Task<BaseResponse<TEntity>> AddAsync(TEntity entity)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"api/{serviceName}/Add";
            request.Parameter = entity;
            return await client.ExecuteAsync<TEntity>(request);
        }

        public async Task<BaseResponse> DeleteAsync(GetAndDelParameter parameter)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Delete;
            request.Route = $"api/{serviceName}/Delete";
            request.Parameter = parameter;
            return await client.ExecuteAsync(request);
        }

        public async Task<BaseResponse<PagedList<TEntity>>> GetAllAsync(QueryParameter parameter)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Get;
            request.Route = $"api/{serviceName}/GetAll" +
                $"?pageIndex={parameter.PageIndex}" +
                $"&pageSize={parameter.PageSize}" +
                $"&search={parameter.Search}"+
                $"&userId={parameter.UserId}"+
                $"&sortOrder={parameter.SortOrder}";
            return await client.ExecuteAsync<PagedList<TEntity>>(request);
        }

        public async Task<BaseResponse<TEntity>> GetFirstOrDefaultAsync(GetAndDelParameter parameter)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Get;
            request.Route = $"api/{serviceName}/Get?id={parameter.Id}&UserId={parameter.UserId}";
            return await client.ExecuteAsync<TEntity>(request);
        }

        public async Task<BaseResponse<TEntity>> UpdateAsync(TEntity entity)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"api/{serviceName}/Update";
            request.Parameter = entity;
            return await client.ExecuteAsync<TEntity>(request);
        }
    }
}
