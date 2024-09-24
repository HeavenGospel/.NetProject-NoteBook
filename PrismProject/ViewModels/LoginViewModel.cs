using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using PrismProject.Common.Events;
using PrismProject.Common.Models;
using PrismProject.Extensions;
using PrismProject.Service;
using PrismProject.Statics.Informations;
using System;

namespace PrismProject.ViewModels
{
    public class LoginViewModel : BindableBase, IDialogAware
    {
        public LoginViewModel(ILoginService loginService, IEventAggregator eventAggregator)
        {
            RegisterDto = new RegisterDto();
            ExecuteCommand = new DelegateCommand<string>(Execute);
            this.loginService = loginService;
            this.eventAggregator = eventAggregator;
        }

        #region 服务字段
        private readonly ILoginService loginService;
        #endregion

        #region 事件字段
        private readonly IEventAggregator eventAggregator;
        #endregion

        #region 窗口标题
        public string Title { get; set; } = "笔记本";
        #endregion

        #region 用户账号和密码(登陆)
        private string account;
        public string Account
        {
            get { return account; }
            set { account = value; RaisePropertyChanged(); }
        }
        private string password;
        public string Password
        {
            get { return password; }
            set { password = value; RaisePropertyChanged(); }
        }
        #endregion

        #region 用户注册信息
        private RegisterDto registerDto;
        public RegisterDto RegisterDto
        {
            get { return registerDto; }
            set { registerDto = value; RaisePropertyChanged(); }
        }
        #endregion

        #region 命令集合(登陆, 登出, 注册, 切换页面)
        public DelegateCommand<string> ExecuteCommand { get; private set; }
        private void Execute(string obj)
        {
            switch (obj)
            {
                case "Login": Login(); break;
                case "LoginOut": LoginOut(); break;
                case "Register": Register(); break;
                case "GotoRegister": SelectedIndex = 1; break;
                case "ReturnLogin": SelectedIndex = 0; break;
            }
        }
        #endregion

        #region 登陆
        private async void Login()
        {
            if (string.IsNullOrWhiteSpace(Account) || string.IsNullOrWhiteSpace(Password))
            {
                eventAggregator.ShowMessage(new MessageModel() { Message = "请填写完整且合法的信息。", FilterName = "Login" });
                return;
            }
            var loginResult = await loginService.LoginAsync(new UserDto() { UserName = "", Account = Account, Password = Password });
            if (loginResult != null && loginResult.Status)
            {
                AppSession.UserName = loginResult.Result.UserName;
                AppSession.UserId = loginResult.Result.Id;
                eventAggregator.ShowMessage(new MessageModel() { Message = "登陆成功！" });
                eventAggregator.ShowMessage(new MessageModel() { Message = AppSession.UserName, FilterName = "TransmitUser" });
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
            }
            else
                eventAggregator.ShowMessage(new MessageModel() { Message = loginResult.Message, FilterName = "Login" });
        }
        #endregion

        #region 登出
        private void LoginOut()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.No));
        }
        #endregion

        #region 注册
        private async void Register()
        {
            if (string.IsNullOrWhiteSpace(RegisterDto.Account) ||
               string.IsNullOrWhiteSpace(RegisterDto.UserName) ||
               string.IsNullOrWhiteSpace(RegisterDto.Password))
            {
                eventAggregator.ShowMessage(new MessageModel() { Message = "请填写完整且合法的信息。", FilterName = "Login" });
                return;
            }
            if (RegisterDto.Password != RegisterDto.PasswordAgain)
            {
                eventAggregator.ShowMessage(new MessageModel() { Message = "两次输入的密码不一致，请检查。", FilterName = "Login" });
                return;
            }
            var regiterResult = await loginService.RegisterAsync(new UserDto()
            {
                Account = RegisterDto.Account,
                Password = RegisterDto.Password,
                UserName = RegisterDto.UserName
            });
            if (regiterResult != null && regiterResult.Status)
            {
                eventAggregator.ShowMessage(new MessageModel() { Message = "注册成功！", FilterName = "Login" });
                Account = RegisterDto.Account;
                Password = RegisterDto.Password;
                SelectedIndex = 0;
            }
            else
                eventAggregator.ShowMessage(new MessageModel() { Message = regiterResult.Message, FilterName = "Login" });
        }
        #endregion

        #region 切换页面(登陆/注册)
        private int selectedIndex;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value; RaisePropertyChanged(); }
        }
        #endregion

        #region 弹窗相关
        public event Action<IDialogResult> RequestClose;
        public bool CanCloseDialog()
        {
            return true;
        }
        public void OnDialogClosed()
        {
            LoginOut();
        }
        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
        #endregion
    }
}
