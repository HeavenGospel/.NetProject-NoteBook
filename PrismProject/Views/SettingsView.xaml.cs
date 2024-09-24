using Prism.Regions;
using PrismProject.Extensions;
using System.Windows.Controls;

namespace PrismProject.Views
{
    /// <summary>
    /// Interaction logic for SettingsView
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
            menuBar.SelectedIndex = 0;
        }
    }
}
