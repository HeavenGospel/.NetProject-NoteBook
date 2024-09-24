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
	public class TodoViewModel : NavigationViewModel
    {
        public TodoViewModel(ITodoService service, IContainerProvider containerProvider) : base(containerProvider)
        {
            TodoDtos = new ObservableCollection<TodoDto>();  // 实例化待办事项列表
            ExecuteCommand = new DelegateCommand<string>(Execute);  // 实例化命令集合(添加, 查询, 保存)
            SelectCommand = new DelegateCommand<TodoDto>(Select);  // 实例化选择命令
            DeleteCommand = new DelegateCommand<TodoDto>(DeleteAsync);  // 实例化删除命令
            this.service = service;  // 导入web服务
            aggregator.RegisterMessage(args =>
            {
                GetDataAsync();
            }, "TransmitUser");
        }

        #region web服务字段
        private readonly ITodoService service;
        #endregion

        #region 判断是否打开右侧抽屉
        private bool isRightDrawerOpen; 
        public bool IsRightDrawerOpen
        {
            get { return isRightDrawerOpen; }
            set { isRightDrawerOpen = value; RaisePropertyChanged(); }  // 添加通知更改
        }
        #endregion

        # region 新增对象/编辑对象时, 临时数据存储对象
        private TodoDto currentDto;
        public TodoDto CurrentDto
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

        #region 待办事项查询状态筛选
        private int selectIndex;

        public int SelectIndex
        {
            get { return selectIndex; }
            set { selectIndex = value; RaisePropertyChanged(); }
        }
        #endregion

        #region 待办事项列表
        private ObservableCollection<TodoDto> todoDtos;
        public ObservableCollection<TodoDto> TodoDtos
        {
            get { return todoDtos; }
            set { todoDtos = value; RaisePropertyChanged(); }  // 通知更改
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

        #region 添加待办事项
        private void Add()
        {
            CurrentDto = new TodoDto();
            CurrentDto.UserId = AppSession.UserId;
            IsRightDrawerOpen = true;
        }
        #endregion

        #region 查询待办事项
        private async void GetDataAsync()
        {
            try
            {
                //UpdateLoading(true);  // 打开等待窗口
                var todoResult = await service.GetAllAsync(new QueryParameter()
                {
                    PageIndex = 0,
                    PageSize = Int32.MaxValue,
                    Search = Search,
                    UserId = AppSession.UserId,
                    SortOrder = 0,  // 按创建时间排序
                });
                if (todoResult.Status)
                {
                    TodoDtos.Clear();
                    foreach (var item in todoResult.Result.Items)
                    {
                        if (SelectIndex == 0 || (SelectIndex >> 1) == item.Status)
                            TodoDtos.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                //UpdateLoading(false);  // 关闭等待窗口
            }
        }
        #endregion

        #region 保存待办事项
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
                    TodoDtos.Add(addResult.Result);
                }
            }
            else
            {
                var updateResult = await service.UpdateAsync(CurrentDto);
                if (updateResult.Status)
                {
                    //GetDataAsync();
                    var todo = TodoDtos.FirstOrDefault(t => t.Id == CurrentDto.Id);
                    if (todo != null)
                    {
                        todo.Title = CurrentDto.Title;  // 这里之所以不能写todo = CurrrentDto,  
                        todo.Content = CurrentDto.Content;  // 是因为todo本身是数组元素, 它并不像CurrentDto, 没有通知更新,
                        todo.Status = CurrentDto.Status;  // 而数组有通知更新并不能使更新单个元素时更新, 只能在添加和删除的时候更新
                    }
                }
            }
            IsRightDrawerOpen = false;
        }
        #endregion

        #region 选择(修改)待办事项
        public DelegateCommand<TodoDto> SelectCommand { get; private set; }
        private void Select(TodoDto dto)
        {
            CurrentDto = new TodoDto();
            CurrentDto.Id = dto.Id;
            CurrentDto.Title = dto.Title;
            CurrentDto.Content = dto.Content;
            CurrentDto.Status = dto.Status;
            CurrentDto.UserId = AppSession.UserId;
            IsRightDrawerOpen = true;
        }
        #endregion

        #region 删除待办事项
        public DelegateCommand<TodoDto> DeleteCommand { get; private set; }
        private async void DeleteAsync(TodoDto dto)
        {
            var deleteResult = await service.DeleteAsync(new GetAndDelParameter() { Id = dto.Id , UserId = AppSession.UserId });
            if (deleteResult.Status)
            {
                TodoDtos.Remove(dto);
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

            if (navigationContext.Parameters.ContainsKey("SelectIndex"))
                SelectIndex = navigationContext.Parameters.GetValue<int>("SelectIndex");  // 接收参数, 筛选待办事项状态
            else
                SelectIndex = 0;
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
