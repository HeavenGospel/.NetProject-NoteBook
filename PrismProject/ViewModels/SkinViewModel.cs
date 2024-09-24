using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using System;
using System.Collections.Generic;
using MaterialDesignColors.ColorManipulation;
using System.Windows.Media;
using System.IO;
using System.Text.Json;
using PrismProject.Extensions;
using Prism.Mvvm;

namespace PrismProject.ViewModels
{
    public class SkinViewModel : BindableBase
    {
        public SkinViewModel()
        {
            // 加载并应用保存的主题
            var savedTheme = LoadTheme();

            IsDarkTheme = savedTheme.IsDarkTheme;

            if (ColorConverter.ConvertFromString(savedTheme.PrimaryColor) is Color savedColor)
            {
                ChangeHue(savedColor);
            }

            ChangeHueCommand = new DelegateCommand<object>(ChangeHue);
        }

        public IEnumerable<ISwatch> Swatches { get; } = SwatchHelper.Swatches;
        public DelegateCommand<object> ChangeHueCommand { get; private set; }

        private readonly PaletteHelper paletteHelper = new();

        private void ChangeHue(object? obj)
        {
            var hue = (Color)obj!;

            Theme theme = paletteHelper.GetTheme();

            theme.PrimaryLight = new ColorPair(hue.Lighten());
            theme.PrimaryMid = new ColorPair(hue);
            theme.PrimaryDark = new ColorPair(hue.Darken());

            paletteHelper.SetTheme(theme);

            // 保存新颜色和主题设置
            SaveTheme(hue, IsDarkTheme);
        }

        private bool _isDarkTheme;
        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set
            {
                if (SetProperty(ref _isDarkTheme, value))
                {
                    ModifyTheme(theme => theme.SetBaseTheme(value ? BaseTheme.Dark : BaseTheme.Light));

                    // 保存亮暗主题设置
                    SaveTheme((Color)ColorConverter.ConvertFromString(LoadTheme().PrimaryColor), value);
                }
                RaisePropertyChanged();
            }
        }

        private static void ModifyTheme(Action<Theme> modificationAction)
        {
            var paletteHelper = new PaletteHelper();
            Theme theme = paletteHelper.GetTheme();

            modificationAction?.Invoke(theme);

            paletteHelper.SetTheme(theme);
        }

        private void SaveTheme(Color primaryColor, bool isDarkTheme)
        {
            var themeSettings = new ThemeSettings
            {
                PrimaryColor = primaryColor.ToString(),
                IsDarkTheme = isDarkTheme
            };

            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PrismProject");
            Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, "ThemeSettings.json");

            string jsonString = JsonSerializer.Serialize(themeSettings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonString);
        }

        private ThemeSettings LoadTheme()
        {
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PrismProject");
            string filePath = Path.Combine(folderPath, "ThemeSettings.json");

            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<ThemeSettings>(jsonString);
            }

            return new ThemeSettings { PrimaryColor = "#FF0D47A1", IsDarkTheme = true };
        }
    }
}
