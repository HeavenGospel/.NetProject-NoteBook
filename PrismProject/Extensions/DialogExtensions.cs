using Prism.Events;
using PrismProject.Common.Events;
using System;

namespace PrismProject.Extensions
{
    public static class DialogExtensions  // 弹窗的扩展方法
    {
        public static void ShowLoading(this IEventAggregator aggregator, UpdateModel model)  // 推送等待消息
        {
            aggregator.GetEvent<UpdateLoadingEvent>().Publish(model);
        }
        public static void RegisterLoading(this IEventAggregator aggregator, Action<UpdateModel> model)  // 注册等待消息
        {
            aggregator.GetEvent<UpdateLoadingEvent>().Subscribe(model);
        }


        public static void ShowMessage(this IEventAggregator aggregator, MessageModel model)  // 推送提示消息
        {
            if(model.FilterName == null) model.FilterName = "Main";
            aggregator.GetEvent<MessageEvent>().Publish(model);
        }
        public static void RegisterMessage(this IEventAggregator aggregator, Action<MessageModel> model, string FilterName = "Main")  // 注册提示消息
        {
            aggregator.GetEvent<MessageEvent>().Subscribe(model, ThreadOption.PublisherThread, true, (m) => 
            { 
                return m.FilterName.Equals(FilterName);
            });
        }
    }
}
