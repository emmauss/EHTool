using EHTool.Common.Helpers;
using EHTool.EHTool.Model;
using EHTool.Shared.Helpers;
using EHTool.Shared.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “内容对话框”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上进行了说明

namespace EHTool.EHTool.View
{
    public sealed partial class LanguageFilterDialog : ContentDialog ,INotifyPropertyChanged
    {
        public List<LanguageModel> LanguageList { get; set; }
        public LanguageFilterDialog()
        {
            this.InitializeComponent();
            OnInit();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void OnInit()
        {
            LanguageList = (await CookieHelper.GetLanguageSetting()).ToList();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LanguageList)));
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            await CookieHelper.UpdateUnconfig(LanguageList);
        }
        
    }
}
