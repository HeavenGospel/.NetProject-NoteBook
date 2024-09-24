using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using PrismProject.Common.Models;
using PrismProject.Extensions;
using System.Collections.ObjectModel;

namespace PrismProject.ViewModels
{
	public class SettingsViewModel : NavigationViewModel
	{
        public SettingsViewModel(IRegionManager regionManager, IContainerProvider containerProvider) : base(containerProvider)
        {
            this.regionManager = regionManager;  // 导入区域
            MenuBars = new ObservableCollection<MenuBar>();  // 实例化侧边菜单栏
            CreateMenuBar();  // 创建菜单栏菜单
            NavigateCommand = new DelegateCommand<MenuBar>(Navigate);  // 实例化导航命令
            
        }

        #region 创建区域字段
        private readonly IRegionManager regionManager;
        #endregion

        #region 侧边栏导航
        private bool firstLoad = true;  // 首次加载标志
        public DelegateCommand<MenuBar> NavigateCommand { get; private set; }  // 导航命令
        private void Navigate(MenuBar bar)
        {
            if (bar == null || string.IsNullOrWhiteSpace(bar.NameSpace)) return;
            regionManager.Regions[PrismManager.SettingsViewRegionName].RequestNavigate(bar.NameSpace);  // 导航到指定视图
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
            MenuBars.Add(new MenuBar() { Icon = "Palette", Title = "个性化", NameSpace = "SkinView" });
            MenuBars.Add(new MenuBar() { Icon = "Cog", Title = "其他设置", NameSpace = "MinSetting" });
            MenuBars.Add(new MenuBar() { Icon = "Information", Title = "关于更多", NameSpace = "AboutView" });
        }
        #endregion
        
        #region 首次加载
        bool isFirstLoad = true;  // 首次加载标志
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (isFirstLoad)
            {
                this.regionManager.Regions[PrismManager.SettingsViewRegionName].RequestNavigate("SkinView");  // 导航到指定视图
                isFirstLoad = false;
            }
            base.OnNavigatedFrom(navigationContext);
        }
        #endregion
    }
}
