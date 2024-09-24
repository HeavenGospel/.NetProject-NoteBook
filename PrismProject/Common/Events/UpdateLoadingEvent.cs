using Prism.Events;

namespace PrismProject.Common.Events  // 传输加载转圈事件
{
    public class UpdateModel
    { 
        public bool IsOpen { get; set; }
    }

    public class UpdateLoadingEvent : PubSubEvent<UpdateModel>
    {

    }
}
