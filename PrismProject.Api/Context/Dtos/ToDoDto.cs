
namespace PrismProject.Api.Context.Dtos
{
    public class ToDoDto : BaseDto  // 待办事项数据实体
    {
        private string title;
        private string content;
        private int status;
        private int userId;

        public string Title
        {
            get { return title; }
            set { title = value; OnPropertyChanged(); }
        }
        public string Content
        {
            get { return content; }
            set { content = value; OnPropertyChanged(); }
        }
        public int Status
        {
            get { return status; }
            set { status = value; OnPropertyChanged(); }
        }
        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }
    }
}
