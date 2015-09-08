using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Common.Helpers;
using EHTool.Common.Helpers;
using EHTool.EHTool.Common;
using EHTool.EHTool.Model;
using EHTool.EHTool.ViewModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace EHTool.EHTool.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class EHMainPage : Page,INotifyPropertyChanged
    {
        public MainViewModel MainVM { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        #region TitleBarMember
        private void TitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TitleBarHeight"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TitleBarPadding"));
        }
        public void FullScreenClick()
        {
            if (ApplicationView.GetForCurrentView().IsFullScreen)
            {
                ApplicationView.GetForCurrentView().ExitFullScreenMode();
            }
            else
            {
                ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FullScreenButtonContent"));
        }

        #region Properties        
        public string FullScreenButtonContent
        {
            get
            {
                return ApplicationView.GetForCurrentView().IsFullScreenMode ? "\uE73F" : "\uE1D9";
            }
        }
        public Thickness TitleBarPadding
        {
            get
            {
                if (FlowDirection == FlowDirection.LeftToRight)
                {
                    return new Thickness() { Left = CoreApplication.GetCurrentView().TitleBar.SystemOverlayLeftInset, Right = CoreApplication.GetCurrentView().TitleBar.SystemOverlayRightInset };
                }
                else
                {
                    return new Thickness() { Left = CoreApplication.GetCurrentView().TitleBar.SystemOverlayRightInset, Right = CoreApplication.GetCurrentView().TitleBar.SystemOverlayLeftInset };
                }
            }
        }
        public double TitleBarHeight
        {
            get
            {
                return CoreApplication.GetCurrentView().TitleBar.Height;
            }
        }
        #endregion
        #endregion
        public bool IsMainPaneOpen { get; set; }
        public bool IsFavorPaneOpen { get; set; }
        public bool IsSearchOptionShow { get; set; } = true;
        public bool HasLogin => CookieHelper.CheckCookie();
        public bool IsReadingDoublePage
        {
            get
            {
                return SettingHelpers.GetSetting<bool>("IsReadingDoublePage");
            }
            set
            {
                SettingHelpers.SetSetting("IsReadingDoublePage", value);
            }
        }
       

        public EHMainPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            CoreApplication.GetCurrentView().TitleBar.LayoutMetricsChanged += TitleBar_LayoutMetricsChanged;
            MainVM = new MainViewModel();
        }


        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            Window.Current.SetTitleBar(TitleBarRect);
            if (e.NavigationMode == NavigationMode.Back)
            {
                await MainVM.LoadFavorList();
            }
            base.OnNavigatedTo(e);
        }

        private void RefreshClick(object sender, RoutedEventArgs e)
        {
            MainVM.Initialize();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (pivot.SelectedIndex)
            {
                case 0:
                    IsMainPaneOpen = !IsMainPaneOpen;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsMainPaneOpen)));
                    break;
                case 1:
                    IsFavorPaneOpen = !IsFavorPaneOpen;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFavorPaneOpen)));
                    break;
                default:
                    break;
            }
        }

        private async void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            await MainVM.Search(args.QueryText);
        }

        public void SearchOptionTapped()
        {
            IsSearchOptionShow = !IsSearchOptionShow;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSearchOptionShow)));
        }
        public async void LoginClick()
        {
            if (HasLogin)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasLogin)));
                return;
            }
            LoginDialog dialog = new LoginDialog();
            await dialog.ShowAsync();
            if (dialog.IsSuccess)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasLogin)));
            }
        }
        public async void CacheClearClick()
        {
            await CacheHelper.ClearCache();
            MessageDialog dialog = new MessageDialog("Chche cleared");
            await dialog.ShowAsync();
        }
        public async void LoadMoreClick()
        {
            await MainVM.LoadMore();
        }

        public void MainItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(EHDetailPage), new DetailViewModel(e.ClickedItem as GalleryListModel));
        }
    }
}
