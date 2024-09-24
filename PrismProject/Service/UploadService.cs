using PrismProject.Common.Models;
using System.Threading.Tasks;

namespace PrismProject.Service
{
    public class UploadService : IUploadService  // 上传文件服务
    {
        private readonly HttpRestClient client;
        private readonly string serviceName = "Upload";
        public UploadService(HttpRestClient client)
        {
            this.client = client;
        }

        public async Task<BaseResponse> UploadAsync(UploadParameter parameter)
        {
            var request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"api/{serviceName}/Upload";
            request.Parameter = parameter;
            request.ContentType = "multipart/form-data";

            // 执行请求
            return await client.UploadAsync(request);
        }
        //public async Task<BaseResponse> GetAsync(int id)
        //{
        //    BaseRequest request = new BaseRequest();
        //    request.Method = RestSharp.Method.Get;
        //    request.Route = $"api/{serviceName}/Upload?id={id}";
        //    return await client.ExecuteAsync(request);
        //}

    }
}
