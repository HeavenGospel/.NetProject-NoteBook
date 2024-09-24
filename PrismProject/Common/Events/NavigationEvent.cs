using Prism.Events;

namespace PrismProject.Common.Events  // 传输导航事件
{
    public class NavigationModel
    {
        public int Index { get; set; }
    }

    public class NavigationEvent : PubSubEvent<NavigationModel>
    {

    }
}
