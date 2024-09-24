using Prism.Mvvm;

namespace PrismProject.Common.Models
{
    public class MenuBar : BindableBase  // 侧边栏菜单
    {
        private string icon;
        public string Icon
        {
            get { return icon; }
            set { icon = value; }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        private string nameSpace;
        public string NameSpace
        {
            get { return nameSpace; }
            set { nameSpace = value; }
        }
    }
}
