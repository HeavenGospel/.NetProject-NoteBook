
namespace PrismProject.Common.Models
{
    public class MemoDto : BaseDto  // 备忘录数据实体
    {
        private string title;
        private string content;
        private int userId;

        public string Title
        {
            get { return title; }
            set { title = value; OnPropertyChanged();}
        }
        public string Content
        {
            get { return content; }
            set { content = value; OnPropertyChanged(); }
        }
        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }
    }
}
