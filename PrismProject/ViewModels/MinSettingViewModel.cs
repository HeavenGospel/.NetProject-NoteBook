using Microsoft.Win32;
using Prism.Commands;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using PrismProject.Service;
using Microsoft.AspNetCore.Http;
using PrismProject.Common.Models;
using Microsoft.AspNetCore.Http.Internal;
using Prism.Ioc;
using PrismProject.Common.Events;
using PrismProject.Extensions;
using PrismProject.Statics.Informations;

namespace PrismProject.ViewModels
{
    public class MinSettingViewModel : NavigationViewModel
    {
        public MinSettingViewModel(IUploadService service, IContainerProvider containerProvider) : base(containerProvider)
        {
            this.service = service;  // 导入web服务
            SelectImageCommand = new DelegateCommand(SelectImage);
            UploadCommand = new DelegateCommand(async () => await CropAndUploadImage());
        }

        #region 头像
        private BitmapImage _avatarSource;
        public BitmapImage AvatarSource
        {
            get { return _avatarSource; }
            set { SetProperty(ref _avatarSource, value); RaisePropertyChanged(); }
        }

        private BitmapImage _originalImage;
        public BitmapImage OriginalImage
        {
            get { return _originalImage; }
            set { SetProperty(ref _originalImage, value); RaisePropertyChanged(); }
        }
        #endregion

        #region 服务字段
        private readonly IUploadService service;
        #endregion

        #region 选择图片并预览
        public DelegateCommand SelectImageCommand { get; private set; }
        private void SelectImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg)|*.png;*.jpg";
            if (openFileDialog.ShowDialog() == true)
            {
                var image = new BitmapImage(new Uri(openFileDialog.FileName));
                OriginalImage = image;  // 显示原图
                AvatarSource = CropImage(image); // 显示裁剪后的头像
            }
        }
        #endregion

        #region 裁剪图片
        private BitmapImage CropImage(BitmapImage image)
        {
            // 定义裁剪区域（假设为正方形）
            int cropSize = Math.Min(image.PixelWidth, image.PixelHeight);
            var croppedBitmap = new CroppedBitmap(image, new Int32Rect(0, 0, cropSize, cropSize)); // 从左上角裁剪

            // 转换为 BitmapImage
            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(croppedBitmap));
                encoder.Save(memoryStream);

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = new MemoryStream(memoryStream.ToArray());
                bitmapImage.EndInit();
                bitmapImage.Freeze(); // 必须冻结才能跨线程访问
            }
            return bitmapImage;
        }
        #endregion

        #region 上传裁剪后的图片到服务器
        public DelegateCommand UploadCommand { get; private set; }
        public async Task CropAndUploadImage()
        {
            if (AvatarSource != null)
            {
                var filePath = Path.GetTempFileName();  // 临时文件路径
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    // 保存 BitmapImage 到文件
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(AvatarSource));
                    encoder.Save(fileStream);
                }

                // 读取文件并转换为 byte[]，然后构造 UploadParameter
                var fileBytes = File.ReadAllBytes(filePath);
                var formFile = new FormFile(new MemoryStream(fileBytes), 0, fileBytes.Length, "file", "avatar.png")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/png"
                };

                var uploadParameter = new UploadParameter
                {
                    File = formFile,
                    UserId = AppSession.UserId // 设置用户ID，实际情况从登录上下文获取
                };

                // 调用 UploadService.UploadAsync 方法上传
                var response = await service.UploadAsync(uploadParameter);

                if (response.Status)
                {
                    aggregator.ShowMessage(new MessageModel() { Message = "上传头像成功！" });
                    aggregator.ShowMessage(new MessageModel() { Message = AppSession.UserName, FilterName = "TransmitUser" });
                }
                else
                {
                    aggregator.ShowMessage(new MessageModel() { Message = "上传头像失败！" });
                }
            }
            else
            {
                aggregator.ShowMessage(new MessageModel() { Message = "请先选择图片！" });
            }
        }
        #endregion
    }
}
