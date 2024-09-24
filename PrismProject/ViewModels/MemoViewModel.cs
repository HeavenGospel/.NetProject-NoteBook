using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using PrismProject.Common.Events;
using PrismProject.Common.Models;
using PrismProject.Extensions;
using PrismProject.Service;
using PrismProject.Statics.Informations;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace PrismProject.ViewModels
{
	public class MemoViewModel : NavigationViewModel
    {
        public MemoViewModel(IMemoService service, IContainerProvider containerProvider) : base(containerProvider)
        {
            MemoDtos = new ObservableCollection<MemoDto>();  // 实例化备忘录列表
            ExecuteCommand = new DelegateCommand<string>(Execute);  // 实例化命令集合(添加, 查询, 保存)
            SelectCommand = new DelegateCommand<MemoDto>(Select);  // 实例化选择命令
            DeleteCommand = new DelegateCommand<MemoDto>(Delete);  // 实例化删除命令
            this.service = service;  // 导入web服务
            aggregator.RegisterMessage(args =>
            {
                GetDataAsync();
            }, "TransmitUser");
        }

        #region web服务字段
        private readonly IMemoService service;
        #endregion

        #region 判断是否打开右侧抽屉
        private bool isRightDrawerOpen;
        public bool IsRightDrawerOpen
        {
            get { return isRightDrawerOpen; }
            set { isRightDrawerOpen = value; RaisePropertyChanged(); }  // 添加通知更改
        }
        #endregion

        #region 新增对象/编辑对象时, 临时数据存储对象
        private MemoDto currentDto;
        public MemoDto CurrentDto
        {
            get { return currentDto; }
            set { currentDto = value; RaisePropertyChanged(); }
        }
        #endregion

        #region 查询条件
        private string search;
        public string Search
        {
            get { return search; }
            set { search = value; RaisePropertyChanged(); }
        }
        #endregion

        #region 备忘录列表
        private ObservableCollection<MemoDto> memoDtos;
        public ObservableCollection<MemoDto> MemoDtos
        {
            get { return memoDtos; }
            set { memoDtos = value; RaisePropertyChanged(); }  // 通知更改
        }
        #endregion

        #region 命令集合(添加, 查询, 保存)
        public DelegateCommand<string> ExecuteCommand { get; private set; }
        private void Execute(string parameter)
        {
            switch (parameter)
            {
                case "Add": Add(); break;
                case "Query": GetDataAsync(); break;
                case "Save": SaveAsync(); break;
            }
        }
        #endregion

        #region 添加备忘录
        public DelegateCommand AddCommand { get; private set; }
        private void Add()
        {
            CurrentDto = new MemoDto();
            CurrentDto.UserId = AppSession.UserId;
            IsRightDrawerOpen = true;
        }
        #endregion

        #region 查询备忘录
        private async void GetDataAsync()
        {
            try
            {
                //UpdateLoading(true);  // 打开等待窗口
                var memoResult = await service.GetAllAsync(new QueryParameter()
                {
                    PageIndex = 0,
                    PageSize = Int32.MaxValue,
                    Search = Search,
                    UserId = AppSession.UserId,
                    SortOrder = 0,  // 按创建时间排序
                });
                if (memoResult.Status)
                {
                    MemoDtos.Clear();
                    foreach (var item in memoResult.Result.Items)
                    {
                        MemoDtos.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                //UpdateLoading(false);  // 关闭等待窗口
            }
        }
        #endregion

        #region 保存备忘录
        private async void SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(CurrentDto.Title) || string.IsNullOrWhiteSpace(CurrentDto.Content))
            {
                aggregator.ShowMessage(new MessageModel() { Message = $"标题与内容不能为空。" });
                return;
            }
            if (CurrentDto.Id == 0)
            {
                var addResult = await service.AddAsync(CurrentDto);
                if (addResult.Status)
                {
                    MemoDtos.Add(addResult.Result);
                }
            }
            else
            {
                var updateResult = await service.UpdateAsync(CurrentDto);
                if (updateResult.Status)
                {
                    //GetDataAsync();
                    var memo = MemoDtos.FirstOrDefault(t => t.Id == CurrentDto.Id);
                    if (memo != null)
                    {
                        memo.Title = CurrentDto.Title;
                        memo.Content = CurrentDto.Content;
                    }
                }
            }
            IsRightDrawerOpen = false;
        }
        #endregion

        #region 选择(修改)备忘录
        public DelegateCommand<MemoDto> SelectCommand { get; private set; }
        private void Select(MemoDto dto)
        {
            CurrentDto = dto;
            CurrentDto.UserId = AppSession.UserId;
            IsRightDrawerOpen = true;
        }
        #endregion

        #region 删除备忘录
        public DelegateCommand<MemoDto> DeleteCommand { get; private set; }
        private async void Delete(MemoDto dto)
        {
            var deleteResult = await service.DeleteAsync(new GetAndDelParameter() { Id = dto.Id , UserId = AppSession.UserId });
            if (deleteResult.Status)
            {
                MemoDtos.Remove(dto);
            }
        }
        #endregion

        #region 导航进页面时
        public bool IsDialogOpen
        {
            get { return isDialogOpen; }
            set { isDialogOpen = value; RaisePropertyChanged(); }
        }
        private bool isDialogOpen;
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            IsDialogOpen = false;
            base.OnNavigatedTo(navigationContext);

            GetDataAsync();

            Sleep();
        }
        private async void Sleep()
        {
            await Task.Delay(350);
            IsDialogOpen = true;
        }
        #endregion        
    }
}
