using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using PrismProject.Common.Events;
using PrismProject.Common.Models;
using PrismProject.Extensions;
using PrismProject.Service;
using PrismProject.Statics.Informations;
using PrismProject.ViewModels.Dialogs;
using PrismProject.Views.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Media;
using System.Windows;

namespace PrismProject.ViewModels
{
	public class IndexViewModel : NavigationViewModel
    {
        public IndexViewModel(ITodoService todoService, IMemoService memoService, IContainerProvider containerProvider) : base(containerProvider)
        {
            this.todoService = todoService;  // 导入web服务
            this.memoService = memoService;  // 导入web服务
            TaskBars = new ObservableCollection<TaskBar>();
            TodoDtos = new ObservableCollection<TodoDto>();
            MemoDtos = new ObservableCollection<MemoDto>();
            AddTodoCommand = new DelegateCommand(AddTodoAsync);
            AddMemoCommand = new DelegateCommand(AddMemoAsync);
            UpdateTodoCommand = new DelegateCommand<TodoDto>(UpdateTodoAsync);
            UpdateMemoCommand = new DelegateCommand<MemoDto>(UpdateMemoAsync);
            FinishTodoCommand = new DelegateCommand<TodoDto>(FinishTodoAsync);
            NavigateCommand = new DelegateCommand<TaskBar>(NavigateToPage);
            regionManager = containerProvider.Resolve<IRegionManager>();
            aggregator.RegisterMessage(args =>
            {
                Greet = $"你好，{args.Message}！今天是{DateTime.Now.GetDateTimeFormats('D')[1].ToString()}";
                GetDataAsync();
                UpdateTaskBarsAsync();
            }, "TransmitUser");
        }

        #region web服务字段
        private readonly ITodoService todoService;
        private readonly IMemoService memoService;
        #endregion

        #region 显示弹窗
        public async Task<object> ShowCustomDialogAsync(string dialogType, object dto = null)
        {
            UserControl dialogContent = null;
            // 根据 dialogType 条件，选择要显示的弹窗控件
            switch (dialogType)
            {
                case "AddTodo":
                    dialogContent = new AddTodoView();
                    break;
                case "AddMemo":
                    dialogContent = new AddMemoView(); 
                    break;
                case "UpdateTodo":
                    dialogContent = new AddTodoView();
                    if(dto == null) return null;  // 防止用户双击出现bug
                    (dialogContent.DataContext as AddTodoViewModel).CurrentDto = dto as TodoDto;
                    break;
                case "UpdateMemo":
                    if (dto == null) return null;  // 防止用户双击出现bug
                    dialogContent = new AddMemoView();
                    (dialogContent.DataContext as AddMemoViewModel).CurrentDto = dto as MemoDto;
                    break;
            }
            // 显示弹窗
            var res = await DialogHost.Show(dialogContent, "IndexDialogHost");
            UpdateTaskBarsAsync();  // 每次操作完更新菜单栏
            return res;
        }
        #endregion

        #region 添加
        public DelegateCommand AddTodoCommand { get; private set; }
        public DelegateCommand AddMemoCommand { get; private set; }
        private  async void AddTodoAsync()
        {
            TodoDto res = await ShowCustomDialogAsync("AddTodo") as TodoDto;
            if (res == null || res.Status == 1) return;
            GetDataAsync();  // 更新列表
            //TodoDtos.Add(res);
        }
        private async void AddMemoAsync()
        {
            MemoDto res = await ShowCustomDialogAsync("AddMemo") as MemoDto;
            if (res == null) return;
            GetDataAsync();  // 更新列表
            //MemoDtos.Add(res);
        }
        #endregion

        #region 更新
        public DelegateCommand<TodoDto> UpdateTodoCommand { get; private set; }
        private async void UpdateTodoAsync(TodoDto dto)
        {
            TodoDto res = await ShowCustomDialogAsync("UpdateTodo", dto) as TodoDto;
            if (res == null) return;
            GetDataAsync();  // 更新列表
            //var todo = TodoDtos.FirstOrDefault(t => t.Id == res.Id);
            //if (todo != null)
            //{
            //    if (res.Status == 0)
            //    {
            //        todo.Title = res.Title;
            //        todo.Content = res.Content;
            //    }
            //    else TodoDtos.Remove(todo);
            //}
        }
        public DelegateCommand<MemoDto> UpdateMemoCommand { get; private set; }
        private async void UpdateMemoAsync(MemoDto dto)
        {
            MemoDto res = await ShowCustomDialogAsync("UpdateMemo", dto) as MemoDto;
            if (res == null) return;
            GetDataAsync();  // 更新列表
            //var memo = MemoDtos.FirstOrDefault(t => t.Id == res.Id);
            //if (memo != null)
            //{
            //    memo.Title = res.Title;
            //    memo.Content = res.Content;
            //}
        }
        #endregion

        #region 完成
        public DelegateCommand<TodoDto> FinishTodoCommand { get; private set; }
        private async void FinishTodoAsync(TodoDto dto)
        {
            if (dto == null || dto.Status == 1) return;

            dto.Status = 1;
            var res = await todoService.UpdateAsync(dto);
            if (res.Status)
            {
                await Task.Delay(200);
                TodoDtos.Remove(dto);
                UpdateTaskBarsAsync();  // 因为ToggleButton没有弹窗, 所以特别更新
        
                // 播放完成音效
                PlaySoundEffect();
            }
    
            aggregator.ShowMessage(new MessageModel() { Message = "已完成" });  // 发送消息提示
        }

        private void PlaySoundEffect()
        {
            try
            {
                var uri = new Uri("pack://application:,,,/Statics/Audios/finish.mp3");
                var player = new MediaPlayer();
                player.Volume = 1.0; // 设置音量为最大
                player.MediaOpened += (sender, e) => 
                {
                    player.Play();
                };
        
                player.Open(uri);
            }
            catch (Exception ex)
            {
                // 处理异常
                Console.WriteLine("播放音效时出现问题: " + ex.Message);
            }
        }
        #endregion

        #region 菜单栏
        private ObservableCollection<TaskBar> taskBars;

        public ObservableCollection<TaskBar> TaskBars
        {
            get { return taskBars; }
            set { taskBars = value; RaisePropertyChanged(); }  // 添加通知更改
        }
        private void CreateTaskBars()
        {
            TaskBars.Clear();
            TaskBars.Add(new TaskBar() {Icon = "ClockFast", Title = "汇总", Content = "0", Color = "#FF0CA0FF", Target = "TodoView"});
            TaskBars.Add(new TaskBar() {Icon = "ClockCheckOutline", Title = "已完成", Content = "0", Color = "#FF1ECA3A", Target = "TodoView"});
            TaskBars.Add(new TaskBar() {Icon = "ChartLineVariant", Title = "完成率", Content = "100%", Color = "#FF02c6DC", Target = "PopUp"});
            TaskBars.Add(new TaskBar() {Icon = "PlaylistStar", Title = "备忘录", Content = "0", Color = "#FFFFA000", Target="MemoView"});
        }
        private async void UpdateTaskBarsAsync()
        {
            var todoResult = (await todoService.GetAllAsync(new QueryParameter()
            {
                PageIndex = 0,
                PageSize = 100,
                UserId = AppSession.UserId,
            })).Result;
            var memoResult = (await memoService.GetAllAsync(new QueryParameter()
            {
                PageIndex = 0,
                PageSize = 100,
                UserId = AppSession.UserId,
            })).Result;
            if(todoResult != null)
            {
                TaskBars[0].Content = todoResult.TotalCount.ToString();
                TaskBars[1].Content = todoResult.Items.Count(t => t.Status == 1).ToString();
                TaskBars[2].Content = todoResult.Items.Count(t => t.Status == 1) == 0 ? "0" :
                    Math.Round((todoResult.Items.Count(t => t.Status == 1) / (double)todoResult.TotalCount * 100)).ToString("0") + "%";

            }
            if (memoResult != null)
                TaskBars[3].Content = memoResult.TotalCount.ToString();
        }
        #endregion

        #region 待办事项列表 和 备忘录列表
        private ObservableCollection<TodoDto> todoDtos;
        public ObservableCollection<TodoDto> TodoDtos
        {
            get { return todoDtos; }
            set { todoDtos = value; RaisePropertyChanged(); }  // 添加通知更改
        }

        private ObservableCollection<MemoDto> memoDtos;
        public ObservableCollection<MemoDto> MemoDtos
        {
            get { return memoDtos; }
            set { memoDtos = value; RaisePropertyChanged(); }  // 添加通知更改
        }
        private async void GetDataAsync()
        {
            try
            {
                ShowLoading(true);  // 打开等待窗口
                var todoResult = await todoService.GetAllAsync(new QueryParameter()
                {
                    PageIndex = 0,
                    PageSize = Int32.MaxValue,
                    UserId = AppSession.UserId,
                    SortOrder = 1,  // 按修改时间排序
                });
                if (todoResult.Status)
                {
                    TodoDtos.Clear();
                    foreach (var item in todoResult.Result.Items)
                        if(item.Status == 0)
                            TodoDtos.Add(item);
                }

                var memoResult = await memoService.GetAllAsync(new QueryParameter()
                {
                    PageIndex = 0,
                    PageSize = Int32.MaxValue,
                    UserId = AppSession.UserId,
                    SortOrder = 1,  // 按修改时间排序
                });
                if (memoResult.Status)
                {
                    MemoDtos.Clear();
                    foreach (var item in memoResult.Result.Items)
                        MemoDtos.Add(item);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                ShowLoading(false);  // 关闭等待窗口
            }
        }
        #endregion

        #region 菜单栏点击
        private readonly IRegionManager regionManager;
        public DelegateCommand<TaskBar> NavigateCommand { get; private set; }
        private void NavigateToPage(TaskBar bar)
        {
            if(string.IsNullOrWhiteSpace(bar.Target)) return;
            if(bar.Target == "PopUp")
            {
                int.TryParse(bar.Content.TrimEnd('%'), out int percentage);
                switch(percentage)
                {
                    case > 0 and < 20:
                        aggregator.ShowMessage(new MessageModel() { Message = $"完成率为{percentage}%，赶快完成任务吧！" });
                        break;
                    case >= 20 and < 40:
                        aggregator.ShowMessage(new MessageModel() { Message = $"完成率为{percentage}%，同志仍需努力！" });
                        break;
                    case >= 40 and < 60:
                        aggregator.ShowMessage(new MessageModel() { Message = $"完成率为{percentage}%，再接再厉哦！" });
                        break;
                    case >= 60 and < 80:
                        aggregator.ShowMessage(new MessageModel() { Message = $"完成率为{percentage}%，加油！加油！" });
                        break;
                    case >= 80 and < 100:
                        aggregator.ShowMessage(new MessageModel() { Message = $"完成率为{percentage}%，成功只差临门一脚！" });
                        break;
                    case 100:
                        aggregator.ShowMessage(new MessageModel() { Message = "完成率为100%，哇，你真棒！" });
                        break;
                }
                return;
            }
            NavigationParameters parameters = new NavigationParameters();
            if(bar.Title == "已完成")
            {
                parameters.Add("SelectIndex", 2);
            }
            regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(bar.Target, back =>
            {
                aggregator.GetEvent<NavigationEvent>().Publish(new NavigationModel() { Index = bar.Title == "备忘录" ? 2 : 1});
            }, parameters);  // 导航并发送消息
        }
        #endregion

        #region 导航进页面时
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            Greet = $"你好，{AppSession.UserName}！今天是{DateTime.Now.GetDateTimeFormats('D')[1].ToString()}";
            GetDataAsync();
            CreateTaskBars();
            UpdateTaskBarsAsync();
        }
        #endregion

        #region 问候语
        private string greet;
        public string Greet
        {
            get { return greet; }
            set { greet = value; RaisePropertyChanged(); }
        }
        #endregion
    }
}
