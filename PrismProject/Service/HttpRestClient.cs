using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PrismProject.Common.Models;
using RestSharp;

namespace PrismProject.Service
{
    public class HttpRestClient  // 服务通信类
    {
        private readonly RestClient client;
        private readonly object apiUrl;

        public HttpRestClient(string apiUrl)
        {
            client = new RestClient();
            this.apiUrl = apiUrl;
        }

        public async Task<BaseResponse> ExecuteAsync(BaseRequest baseRequest)  // 普通请求
        {
            var request = new RestRequest(apiUrl + baseRequest.Route, baseRequest.Method);
            request.AddHeader("Content-Type", baseRequest.ContentType);
            if (baseRequest.Parameter != null)
                request.AddJsonBody(baseRequest.Parameter);  // 以 JSON 格式发送数据
            var response = await client.ExecuteAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<BaseResponse>(response.Content);
            else
                return new BaseResponse { Status = false, Message = response.ErrorMessage };
        }

        public async Task<BaseResponse<T>> ExecuteAsync<T>(BaseRequest baseRequest)  // 普通请求(泛型)
        {
            var request = new RestRequest(apiUrl + baseRequest.Route, baseRequest.Method);
            request.AddHeader("Content-Type", baseRequest.ContentType);
            if (baseRequest.Parameter != null)
                request.AddJsonBody(baseRequest.Parameter);  // 以 JSON 格式发送数据
            var response = await client.ExecuteAsync(request);
            if(response.StatusCode == System.Net.HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<BaseResponse<T>>(response.Content);
            else
                return new BaseResponse<T> { Status = false, Message = response.ErrorMessage };
        }

        public async Task<BaseResponse> UploadAsync(BaseRequest baseRequest)  // 文件上传请求
        {
            var request = new RestRequest(apiUrl + baseRequest.Route, baseRequest.Method);
            request.AddHeader("Content-Type", baseRequest.ContentType);
            if (baseRequest.Parameter != null)
            {
                var parameter = baseRequest.Parameter as UploadParameter;
                using (var ms = new MemoryStream())
                {
                    parameter.File.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    // 添加文件到请求中
                    request.AddFile("file", fileBytes, parameter.File.FileName, "multipart/form-data");
                }
                request.AddParameter("UserId", parameter.UserId);
            }
            var response = await client.ExecuteAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<BaseResponse>(response.Content);
            else
                return new BaseResponse { Status = false, Message = response.ErrorMessage };
        }
    }
}
