using System.Threading.Tasks;
using PrismProject.Common.Models;

namespace PrismProject.Service
{
    public interface IUploadService  // 上传文件(接口)
    {
        Task<BaseResponse> UploadAsync(UploadParameter parameter);

        //Task<BaseResponse> GetAsync(int id);
    }
}
