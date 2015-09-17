using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Common.Helpers;
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
using EHTool.EHTool.Common.Helpers;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace EHTool.EHTool.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class EHDetailPage : Page,INotifyPropertyChanged
    {
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
        
        public DetailViewModel DetailVM { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public double ImageWidth { get; private set; }
        public double ImageHeight { get; private set; }
        public bool IsPaneOpen { get; set; }
        
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

        public EHDetailPage()
        {
            this.InitializeComponent();
            ImageWidth = Window.Current.Bounds.Width / 4d - 10d;
            ImageHeight = Width * 4d / 3d;
            SizeChanged += EHDetailPage_SizeChanged;
        }

        public void SelectedPageChanged(object sender,object e)
        {
            var combobox = sender as ComboBox;
            DetailVM.SelectedPage = combobox.SelectedIndex;
        }

        private void EHDetailPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ImageWidth = Window.Current.Bounds.Width / 4d - 10d;
            ImageHeight = Width * 4d / 3d;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageWidth)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageHeight)));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Window.Current.SetTitleBar(TitleBarRect);
            DetailVM = e.Parameter as DetailViewModel;
            base.OnNavigatedTo(e);
        }
        public void SettingButtonClick()
        {
            IsPaneOpen = !IsPaneOpen;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPaneOpen)));
        }
        public void BackClick()
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
        public async void DownloadTorrentClick()
        {
            var torrentDialog = new TorrentDialog(DetailVM.GetTorrentList());
            await torrentDialog.ShowAsync();
        }
        public async void ReadButtonClick()
        {
            if (DetailVM.IsDownloaded)
            {
                var item = await DownloadHelper.GetItem(DetailVM.Id);
                Frame.Navigate(typeof(EHReadingPage), new ReadingViewModel(item));
            }
            else
            {
                Frame.Navigate(typeof(EHReadingPage), new ReadingViewModel(DetailVM.ListItem));
            }
        }
        public async void FavorClick()
        {
            await DetailVM.FavorHandler();
        }

        public async void DownloadClick()
        {
            var dialog = new AddDownloadDialog(DetailVM.ListItem);
            await dialog.ShowAsync();
            if (dialog.IsDownloadInApp)
            {
                await DetailVM.Download();
            }
            else
            {
                if (dialog.Token != null)
                {
                    await DetailVM.Download(dialog.Token);
                }
            }
        }

        public async void ImageListItemClick(object sender, ItemClickEventArgs e)
        {
            var clickitem = e.ClickedItem as ImageListModel;
            if (DetailVM.IsDownloaded)
            {
                var item = await DownloadHelper.GetItem(DetailVM.Id);
                Frame.Navigate(typeof(EHReadingPage), new ReadingViewModel(item, clickitem));
            }
            else
            {
                Frame.Navigate(typeof(EHReadingPage), new ReadingViewModel(DetailVM.ListItem, clickitem));
            }
        }
    }
}
