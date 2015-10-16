﻿using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Common.Helpers;
using EHTool.Common.Helpers;
using EHTool.DHPlayer.Model;
using FFmpegInterop;
using Windows.ApplicationModel.Core;
using Windows.Media.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using EHTool.EHTool;
using EHTool.Shared.Entities;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace EHTool.DHPlayer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class VideoPlayPage : Page, INotifyPropertyChanged
    {

        #region TitleBarMember
        private void TitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TitleBarHeight"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TitleBarPadding"));
        }
        public void FullScreenClick()
        {
            if (ApplicationView.GetForCurrentView().IsFullScreenMode)
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
        public bool CanGoBack
        {
            get
            {
                return Frame.CanGoBack;
            }
        }
        public string ItemTitle { get; set; }
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


        Windows.System.Display.DisplayRequest req = new Windows.System.Display.DisplayRequest();

        public bool IsForceDecodeAudio
        {
            get
            {
                return SettingHelper.GetSetting(SettingNames.IsForceDecodeAudio,true);
            }
            set
            {
                SettingHelper.SetSetting(SettingNames.IsForceDecodeAudio, value);
                ReloadVideo();
            }
        }
        public bool IsForceDecodeVideo
        {
            get
            {
                return SettingHelper.GetSetting(SettingNames.IsForceDecodeVideo,true);
            }
            set
            {
                SettingHelper.SetSetting(SettingNames.IsForceDecodeVideo, value);
                ReloadVideo();
            }
        }
        public bool IsPaneOpen { get; set; }


        private FFmpegInteropMSS _ffmpegMSS;
        private IRandomAccessStream _fileStream;
        private string _fileType;

        private bool _isSystemPlay;
        public bool IsSystemPlay
        {
            get
            {
                return _isSystemPlay;
            }
            set
            {
                _isSystemPlay = value;
                ReloadVideo();
            }
        }

        public void VolButtonClick()
        {

            var menu = Resources["VolFlyout"] as Flyout;
            menu.ShowAt(button);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public VideoPlayPage()
        {
            this.InitializeComponent(); 
            Window.Current.SetTitleBar(TitleBarRect);
            CoreApplication.GetCurrentView().TitleBar.LayoutMetricsChanged += TitleBar_LayoutMetricsChanged;
        }
        

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is VideoListModel)
            {
                var item = e.Parameter as VideoListModel;
                LoadVideo(item);
            }
            else if(e.Parameter is StorageFile)
            {
                await LoadVideo(e.Parameter as StorageFile);
            }
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                await StatusBar.GetForCurrentView().HideAsync();
            }
            req.RequestActive();
            base.OnNavigatedTo(e);
        }
        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _fileStream?.Dispose();
            _fileStream = null;
            _ffmpegMSS?.Dispose();
            _ffmpegMSS = null;
            //notice that windows will not release the memory right now
            //but it will lower after few seconds
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                await StatusBar.GetForCurrentView().ShowAsync();
            }
            req.RequestRelease();
            base.OnNavigatedFrom(e);
        }
        private async void LoadVideo(VideoListModel item)
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(item.FilePath);
            await LoadVideo(file);
        }
        private void ReloadVideo()
        {
            if (IsSystemPlay)
            {
                mediaElement.SetSource(_fileStream, _fileType);
                _isSystemPlay = true;
            }
            else
            {
                _ffmpegMSS = FFmpegInteropMSS.CreateFFmpegInteropMSSFromStream(_fileStream, IsForceDecodeAudio, IsForceDecodeVideo);
                MediaStreamSource mss = _ffmpegMSS.GetMediaStreamSource();
                if (mss != null)
                {
                    mediaElement.SetMediaStreamSource(mss);
                }
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSystemPlay)));
        }
        public async Task LoadVideo(StorageFile file)
        {
            mediaElement.Stop();
            _isSystemPlay = false;
            ItemTitle = Path.GetFileNameWithoutExtension(file.Path);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ItemTitle)));
            _fileStream = await file.OpenAsync(FileAccessMode.Read);
            _fileType = file.ContentType;
            if (file.ContentType.ToLower().Contains("video"))
            {
                mediaElement.SetSource(_fileStream, _fileType);
                IsSystemPlay = true;
            }
            else
            {
                IsSystemPlay = false;
                _ffmpegMSS = FFmpegInteropMSS.CreateFFmpegInteropMSSFromStream(_fileStream, IsForceDecodeAudio, IsForceDecodeVideo);
                MediaStreamSource mss = _ffmpegMSS.GetMediaStreamSource();
                if (mss != null)
                {
                    mediaElement.SetMediaStreamSource(mss);
                }
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSystemPlay)));
        }

        
        public void BackClick()
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        public void PlayClick(object sender,object e)
        {
            var button = sender as Button;
            switch (mediaElement.CurrentState)
            {
                case MediaElementState.Playing:
                    if (mediaElement.CanPause)
                    {
                        mediaElement.Pause();
                    }
                    break;
                case MediaElementState.Paused:
                case MediaElementState.Stopped:
                    mediaElement.Play();
                    break;
                default:
                    break;
            }
            
        }

        private void mediaElement_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            switch (mediaElement.CurrentState)
            {
                case MediaElementState.Playing:
                    PlayButton.Content = "\uE103";
                    break;
                case MediaElementState.Paused:
                case MediaElementState.Stopped:
                    PlayButton.Content = "\uE102";
                    break;
                default:
                    break;
            }
        }

        private async void Grid_DragOver(object sender, DragEventArgs e)
        {
            var def = e.GetDeferral();
            e.Handled = true;
            var file = (await e.DataView.GetStorageItemsAsync())[0] as StorageFile;
            if (FileTypeHelper.CheckFileType(file.FileType))
            {
                e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
            }
            else
            {
                e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.None;
            }
            def.Complete();
        }

        private async void Grid_Drop(object sender, DragEventArgs e)
        {
            var def = e.GetDeferral();
            e.Handled = true;
            var file = (await e.DataView.GetStorageItemsAsync())[0] as StorageFile;
            mediaElement.Stop();
            await LoadVideo(file);
            def.Complete();
        }

        private void Grid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (ControlPanel.Opacity == 0d)
            {
                ShowControlPanel.Begin();
            }
            else
            {
                HideControlPanel.Begin();
            }
        }

        private void Grid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ShowControlPanel.Begin();
        }

        private void Grid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            HideControlPanel.Begin();
        }

        private async void mediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            if (_isSystemPlay)
            {
                _ffmpegMSS = FFmpegInteropMSS.CreateFFmpegInteropMSSFromStream(_fileStream, IsForceDecodeAudio, IsForceDecodeVideo);
                MediaStreamSource mss = _ffmpegMSS.GetMediaStreamSource();
                if (mss != null)
                {
                    mediaElement.SetMediaStreamSource(mss);
                }
                _isSystemPlay = false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSystemPlay)));
            }
            else
            {
                MessageDialog dialog = new MessageDialog("can not play this video");
                await dialog.ShowAsync();
            }
        }
        public void SettingButtonClick()
        {
            IsPaneOpen = !IsPaneOpen;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPaneOpen)));
        }

        private void Grid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            FullScreenClick();
        }
        
    }
}
