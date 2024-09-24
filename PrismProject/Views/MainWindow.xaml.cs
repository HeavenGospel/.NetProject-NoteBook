using MaterialDesignThemes.Wpf;
using Prism.Events;
using PrismProject.Common.Events;
using PrismProject.Extensions;
using PrismProject.Statics.Informations;
using PrismProject.ViewModels;
using PrismProject.Views.Dialogs;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PrismProject.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(IEventAggregator aggregator)
        {
            InitializeComponent();

            new SkinViewModel();  // 加载主题

            menuBar.SelectedIndex = 0;

            // 注册等待消息窗口
            aggregator.RegisterLoading(arg =>
            {
                DialogHost.IsOpen = arg.IsOpen;
                if (DialogHost.IsOpen)
                    DialogHost.DialogContent = new ProgressView();
            });

            // 导航栏按钮点击事件
            this.btnMin.Click += (sender, e) => this.WindowState = WindowState.Minimized;
            this.btnMax.Click += (sender, e) => this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            this.btnClose.Click += async (sender, e) =>
            {
                var res = await DialogHost.Show(new MessageView("确认退出应用？", "MainWindowDialogHost"), "MainWindowDialogHost");
                if (res != null) this.Close();
            };
            this.ColorZone.MouseLeftButtonDown += (sender, e) => this.DragMove();
            this.ColorZone.MouseDoubleClick += (sender, e) => this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

            // 侧边栏点击后自动关闭
            menuBar.SelectionChanged += (sender, e) =>
            {
                drawerHost.IsLeftDrawerOpen = false;
            };

            // 注册提示消息
            aggregator.RegisterMessage(arg =>
            {
                snackbar.MessageQueue.Enqueue(arg.Message);
            });

            aggregator.GetEvent<NavigationEvent>().Subscribe(Msg => { menuBar.SelectedIndex = Msg.Index; });  // 订阅首页菜单栏跳转事件

            aggregator.RegisterMessage(async args =>  // 更换用户或头像便加载头像
            {
                userName.Text = args.Message;
                string avatarUrl = $"http://localhost:5184/uploads/avatars/{AppSession.UserId}.jpg";
                await LoadAvatarAsync(avatarUrl);
            }, "TransmitUser");

        }

        #region 加载头像
        private async Task LoadAvatarAsync(string imageUrl)
        {
            if (await ImageExists(imageUrl))
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri($"{imageUrl}?v={Guid.NewGuid()}", UriKind.Absolute); // 通过添加随机参数避免缓存
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    AvatarImage1.Source = bitmap;  // 设置远程头像
                    AvatarImage2.Source = bitmap;  // 设置远程头像
                }
                catch (Exception ex)
                {
                    // 如果加载远程图片失败，使用本地默认头像
                    AvatarImage1.Source = new BitmapImage(new Uri("pack://application:,,,/Statics/Images/user.jpg"));
                    AvatarImage2.Source = new BitmapImage(new Uri("pack://application:,,,/Statics/Images/user.jpg"));

                    // 打印错误信息
                    // MessageBox.Show($"Failed to load image: {ex.Message}");
                }
            }
            else
            {
                // 如果图片不存在，使用本地默认头像
                AvatarImage1.Source = new BitmapImage(new Uri("pack://application:,,,/Statics/Images/user.jpg"));
                AvatarImage2.Source = new BitmapImage(new Uri("pack://application:,,,/Statics/Images/user.jpg"));
            }
        }
        // 发送 HEAD 请求检查图片是否存在
        private async Task<bool> ImageExists(string imageUrl)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // 发送 HEAD 请求，只检查响应状态，不下载内容
                    HttpResponseMessage response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, imageUrl));
                    return response.IsSuccessStatusCode;
                }
            }
            catch
            {
                // 发生异常时，图片不存在
                return false;
            }
        }
        #endregion
    }
}
