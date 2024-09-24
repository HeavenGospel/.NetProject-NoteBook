using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using PrismProject.Extensions;

namespace PrismProject.ViewModels
{
    public class NavigationViewModel : BindableBase, INavigationAware  // 页面导航基类
    {
        private readonly IContainerProvider containerProvider;
        public readonly IEventAggregator aggregator;
        public NavigationViewModel(IContainerProvider containerProvider)
        {
            this.containerProvider = containerProvider;
            aggregator = containerProvider.Resolve<IEventAggregator>();
        }
        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public void ShowLoading(bool IsOpen)
        {
            aggregator.ShowLoading(new Common.Events.UpdateModel()
            {
                IsOpen = IsOpen
            });
        }
    }
}
