using Prism.Events;
using PrismProject.Extensions;
using System.Windows.Controls;

namespace PrismProject.Views
{
    /// <summary>
    /// Interaction logic for LoginView
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginView(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            // 注册消息提示
            eventAggregator.RegisterMessage(args => 
            {
                Snackbar.MessageQueue.Enqueue(args.Message);
            },"Login");
        }
    }
}
