using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace PrismProject.Extensions
{
    public class PassWordExtension  // 密码框扩展
    {
        public static string GetPassWord(DependencyObject obj)
        {
            return (string)obj.GetValue(PassWordProperty);
        }

        public static void SetPassWord(DependencyObject obj, string value)
        {
            obj.SetValue(PassWordProperty, value);
        }

        public static readonly DependencyProperty PassWordProperty =
            DependencyProperty.RegisterAttached("PassWord", typeof(string), typeof(PassWordExtension), new PropertyMetadata(string.Empty, OnPassWordChanged));

        static void OnPassWordChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var passWord = sender as PasswordBox;
            string password = e.NewValue as string;
            if (passWord!= null && password!= passWord.Password)
                passWord.Password = password;
        }
    }

    public class PasswordBehavior : Behavior<PasswordBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PasswordChanged += PasswordChanged;

        }
        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            string password = PassWordExtension.GetPassWord(passwordBox);
            if(passwordBox!= null && password!= passwordBox.Password)
            {
                PassWordExtension.SetPassWord(passwordBox, passwordBox.Password);
            }
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PasswordChanged -= PasswordChanged;
        }
    }
}
