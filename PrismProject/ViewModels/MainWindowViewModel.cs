using Prism.Mvvm;
using System.Collections.ObjectModel;
using PrismProject.Common.Models;
using Prism.Commands;
using Prism.Regions;
using PrismProject.Extensions;
using PrismProject.Views;
using System.Linq;
using Prism.Ioc;

namespace PrismProject.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel(IRegionManager regionManager, IContainerProvider containerProvider)
        {
            this.containerProvider = containerProvider;  // 导入容器
            this.regionManager = regionManager;  // 导入区域
            MenuBars = new ObservableCollection<MenuBar>();  // 实例化侧边菜单栏
            CreateMenuBar();  // 创建菜单栏菜单
            NavigateCommand = new DelegateCommand<MenuBar>(Navigate);  // 实例化导航命令
            MenuToggleButtonCommand = new DelegateCommand(MenuToggleButton_Click);  // 实例化侧边菜单栏按钮命令
            LoginOutCommand = new DelegateCommand(() =>
            {
                App.LoginOut(containerProvider);  // 执行登出操作
            });  // 实例化登出命令
        }

        #region 容器字段
        private readonly IContainerProvider containerProvider;
        #endregion

        #region 创建区域字段
        private readonly IRegionManager regionManager;
        #endregion

        #region 侧边栏导航
        public DelegateCommand<MenuBar> NavigateCommand { get; private set; }  // 导航命令
        private IRegionNavigationJournal journal;  // 导航历史记录
        private void Navigate(MenuBar bar)
        {
            if (bar == null || string.IsNullOrWhiteSpace(bar.NameSpace)) return;
            regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(bar.NameSpace, back =>
            {
                //if (back.Result == true) // 检查导航是否成功
                //{
                //    journal = back.Context.NavigationService.Journal;  // 记录导航历史
                //}
            });  // 导航到指定视图, 使用依赖注入
        }
        #endregion

        #region 侧边菜单栏
        private ObservableCollection<MenuBar> menuBars;  // 动态属性集合, 存放侧边菜单栏菜单
        public ObservableCollection<MenuBar> MenuBars
        {
            get { return menuBars; }
            set { menuBars = value; RaisePropertyChanged(); }  // 通知绑定属性变化
        }
        void CreateMenuBar()  // 创建菜单
        {
            MenuBars.Add(new MenuBar() { Icon = "Home", Title = "主页", NameSpace = "IndexView" });
            MenuBars.Add(new MenuBar() { Icon = "NotebookOutline", Title = "待办", NameSpace = "TodoView" });
            MenuBars.Add(new MenuBar() { Icon = "NotebookPlus", Title = "备忘录", NameSpace = "MemoView" });
            MenuBars.Add(new MenuBar() { Icon = "Cog", Title = "设置", NameSpace = "SettingsView" });
        }
        #endregion

        #region 点击侧边菜单栏按钮时, 关闭控件侧边栏
        public DelegateCommand MenuToggleButtonCommand { get; private set; }  // 侧边菜单栏按钮命令
        private void MenuToggleButton_Click()
        {
            var activeView = regionManager.Regions[PrismManager.MainViewRegionName].ActiveViews.FirstOrDefault();
            if (activeView == null) return;
            if (activeView.GetType().Equals(typeof(TodoView)))
            {
                var temp = (TodoView)activeView;
                temp.drawerHost.IsRightDrawerOpen = false;
            }
            else if (activeView.GetType().Equals(typeof(MemoView)))
            {
                var temp = (MemoView)activeView;
                temp.drawerHost.IsRightDrawerOpen = false;
            }
        }
        #endregion

        #region 登出命令
        public DelegateCommand LoginOutCommand { get; private set; }
        #endregion
    }
}
