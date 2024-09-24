using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace PrismProject.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for AddTodoView
    /// </summary>
    public partial class AddTodoView : UserControl
    {
        public AddTodoView()
        {
            InitializeComponent();
        }

        #region 重新设置文本框焦点
        [DllImport("User32.dll")]
        private static extern IntPtr SetFocus(IntPtr hWnd);
        private void GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            var source = (HwndSource)PresentationSource.FromVisual(textBox);
            if (source != null)
            {
                SetFocus(source.Handle);
                textBox.Focus();
            }
        }
        #endregion
    }
}
