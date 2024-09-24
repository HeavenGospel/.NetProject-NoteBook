using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Ioc;
using PrismProject.Common.Events;
using PrismProject.Common.Models;
using PrismProject.Extensions;
using PrismProject.Service;
using PrismProject.Statics.Informations;

namespace PrismProject.ViewModels.Dialogs
{
    public class AddMemoViewModel : NavigationViewModel
    {
        public AddMemoViewModel(IMemoService service, IContainerProvider containerProvider) : base(containerProvider)
        {
            CurrentDto = new MemoDto();  // 实例化临时数据存储对象
            ExecuteCommand = new DelegateCommand<string>(Execute);  // 实例化命令集合(取消, 确认)
            this.service = service;  // 导入web服务
        }

        #region web服务字段
        private readonly IMemoService service;
        #endregion

        # region 新增对象/编辑对象时, 临时数据存储对象
        private MemoDto currentDto;
        public MemoDto CurrentDto
        {
            get { return currentDto; }
            set { currentDto = value; RaisePropertyChanged(); }
        }
        #endregion

        #region 命令集合(取消, 确认)
        public DelegateCommand<string> ExecuteCommand { get; private set; }
        private void Execute(string parameter)
        {
            switch (parameter)
            {
                case "Cancel": Cancel(); break;
                case "Confirm": ConfirmAsync(); break;
            }
        }
        #endregion

        #region 确认
        private async void ConfirmAsync()
        {
            CurrentDto.UserId = AppSession.UserId;  // 自动添加用户Id
            if (string.IsNullOrWhiteSpace(CurrentDto.Title) || string.IsNullOrWhiteSpace(CurrentDto.Content))
            {
                aggregator.ShowMessage(new MessageModel() { Message = $"标题与内容不能为空。" });
                return;
            }

            BaseResponse<MemoDto> addResult;
            if (CurrentDto.Id == 0)  // 新增
                addResult = await service.AddAsync(CurrentDto);
            else  // 修改
                addResult = await service.UpdateAsync(CurrentDto);

            if (addResult.Status)
                DialogHost.Close("IndexDialogHost", addResult.Result);
        }
        #endregion

        #region 取消
        private void Cancel()
        {
            DialogHost.Close("IndexDialogHost");
        }
        #endregion
    }
}
