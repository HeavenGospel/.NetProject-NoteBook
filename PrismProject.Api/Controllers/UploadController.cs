using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrismProject.Api.Service;
using PrismProject.Shared.Parameters;

namespace PrismProject.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UploadController : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost]
        public async Task<ApiResponse> Upload([FromForm] UploadParameter parameter)
        {
            if (parameter.File == null || parameter.File.Length == 0)
                return new ApiResponse(false, "No file uploaded.");

            int userId = parameter.UserId; // 用户 ID

            // 生成文件路径
            var uploadsFolder = Path.Combine("uploads", "avatars");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder); // 创建目录
            }

            // 使用用户 ID 生成唯一文件名
            var fileName = $"{userId}.jpg"; // 使用用户 ID 和原文件扩展名
            var filePath = Path.Combine(uploadsFolder, fileName);

            // 保存文件到服务器
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await parameter.File.CopyToAsync(stream);
            }
            return new ApiResponse(true, "File uploaded successfully.");
        }

        //[HttpGet]
        //public async Task<ApiResponse> Get(int id)
        //{
        //    var filePath = Path.Combine("uploads", "avatars", $"{id}.jpg");
        //    if (!System.IO.File.Exists(filePath))
        //        return new ApiResponse(false, "No file uploaded.");
        //    var contentType = "image/png";
        //    return new ApiResponse(true, PhysicalFile(filePath, contentType));
        //}
    }
}
