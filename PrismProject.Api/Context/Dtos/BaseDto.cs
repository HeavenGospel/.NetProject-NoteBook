using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PrismProject.Api.Context.Dtos
{
    public class BaseDto : INotifyPropertyChanged
    {
        public int Id { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;  // 实现通知更新
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
