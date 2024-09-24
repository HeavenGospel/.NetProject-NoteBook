using DryIoc;
using MaterialDesignThemes.Wpf;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.Services.Dialogs;
using PrismProject.Service;
using PrismProject.Views;
using System;
using System.Windows;

namespace PrismProject
{
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Web相关的注册
            containerRegistry.GetContainer()
                .Register<HttpRestClient>(made: Parameters.Of.Type<string>(serviceKey: "webUrl"));
            containerRegistry.GetContainer().RegisterInstance(@"http://localhost:5184/", serviceKey: "webUrl");
            containerRegistry.Register<ITodoService, TodoService>();
            containerRegistry.Register<IMemoService, MemoService>();
            containerRegistry.Register<ILoginService, LoginService>();
            containerRegistry.Register<IUploadService, UploadService>();

            // 导航相关的注册
            containerRegistry.RegisterForNavigation<IndexView>();
            containerRegistry.RegisterForNavigation<TodoView>();
            containerRegistry.RegisterForNavigation<MemoView>();
            containerRegistry.RegisterForNavigation<SettingsView>();
            containerRegistry.RegisterForNavigation<SkinView>();
            containerRegistry.RegisterForNavigation<AboutView>();
            containerRegistry.RegisterForNavigation<MinSetting>();

            // 弹窗相关的注册
            containerRegistry.RegisterDialog<LoginView>();
            //containerRegistry.RegisterDialog<RegisterDialog, RegisterDialogViewModel>();
        }

        protected override void OnInitialized()
        {
            var dialog = Container.Resolve<IDialogService>();
            dialog.ShowDialog("LoginView", callback =>
            {
                if (callback.Result != ButtonResult.OK)
                {
                    //Application.Current.Shutdown();
                    Environment.Exit(0);
                    return;
                }

                base.OnInitialized();

                // 使用 RegionManager 导航到主页面
                var regionManager = Container.Resolve<IRegionManager>();
                regionManager.RequestNavigate("MainViewRegion", "IndexView");
            });
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
        }

        public static void LoginOut(IContainerProvider containerProvider)
        {
            Current.MainWindow.Hide();
            var dialog = containerProvider.Resolve<IDialogService>();
            dialog.ShowDialog("LoginView", callback =>
            {
                if (callback.Result != ButtonResult.OK)
                {
                    //Application.Current.Shutdown();
                    Environment.Exit(0);
                    return;
                }
                Current.MainWindow.Show();
            });
        }
    }
}
