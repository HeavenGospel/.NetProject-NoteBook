using Prism.Events;

namespace PrismProject.Common.Events  // 传输用户更改和消息提醒等事件
{
    public class MessageModel
    {
        public string Message { get; set; }
        public string FilterName { get; set; }
    }

    public class MessageEvent : PubSubEvent<MessageModel>
    {

    }
}
