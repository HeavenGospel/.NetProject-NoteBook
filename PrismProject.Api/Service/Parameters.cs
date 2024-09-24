
namespace PrismProject.Shared.Parameters
{
    public class QueryParameter
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? Search { get; set; }
        public int UserId { get; set; }
        public int SortOrder { get; set; }
    }

    public class GetAndDelParameter
    {
        public int Id { get; set; }
        public int UserId { get; set; }
    }

    public class UploadParameter
    {
        public IFormFile File { get; set; }
        public int UserId { get; set; }
    }
}
