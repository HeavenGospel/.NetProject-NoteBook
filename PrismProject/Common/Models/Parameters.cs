using Microsoft.AspNetCore.Http;

namespace PrismProject.Common.Models
{
    public class QueryParameter  // 查询参数
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Search { get; set; }
        public int UserId { get; set; }
        public int SortOrder { get; set; }
    }

    public class GetAndDelParameter  // 获取和删除参数
    {
        public int Id { get; set; }
        public int UserId { get; set; }
    }

    public class UploadParameter  // 上传参数
    {
        public IFormFile File { get; set; }
        public int UserId { get; set; }
    }
}
