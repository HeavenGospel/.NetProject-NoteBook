using MaterialDesignThemes.Wpf;
using System.Windows.Controls;

namespace PrismProject.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for MessageView
    /// </summary>
    public partial class MessageView : UserControl
    {
        public MessageView(string message, string dialogHost)
        {
            InitializeComponent();
            this.dialogHost = dialogHost;
            this.Label.Content = message;
        }

        private readonly string dialogHost;

        private void ButtonCancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogHost.Close(dialogHost);
        }
        private void ButtonConfirm_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogHost.Close(dialogHost, "OK");
        }
    }
}
