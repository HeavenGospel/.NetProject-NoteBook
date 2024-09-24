using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PrismProject.Common.Models
{
    public class BaseDto : INotifyPropertyChanged  // 数据实体基类
    {
        private int id;
        private DateTime createdDate;
        private DateTime modifiedDate;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public DateTime CreatedDate
        {
            get { return createdDate; }
            set { createdDate = value; }
        }

        public DateTime ModifiedDate
        {
            get { return modifiedDate; }
            set { modifiedDate = value; }
        }

        public event PropertyChangedEventHandler? PropertyChanged;  // 实现通知更新
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
