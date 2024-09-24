namespace PrismProject.Api.Context
{
    public class Memo : BaseEntity
    {
        public string Title { get; set; }
        public string? Content { get; set; }
        public int UserId { get; set; }
    }
}
