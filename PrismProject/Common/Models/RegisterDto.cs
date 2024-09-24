
namespace PrismProject.Common.Models
{
    public class RegisterDto : BaseDto  // 注册用户数据实体
    {
        private string userName;

        public string UserName
        {
            get { return userName; }
            set { userName = value; OnPropertyChanged(); }
        }

        private string account;

        public string Account
        {
            get { return account; }
            set { account = value; OnPropertyChanged(); }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set { password = value; OnPropertyChanged(); }
        }

        private string newPassword;

        public string PasswordAgain
        {
            get { return newPassword; }
            set { newPassword = value; OnPropertyChanged(); }
        }
    }
}
