using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Common.Converters;
using Common.Helpers;
using EHTool.Common.Helpers;
using EHTool.DHPlayer;
using EHTool.DHPlayer.Model;
using EHTool.DHPlayer.View;
using EHTool.EHTool.View;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI;
using EHTool.EHTool;
using EHTool.EHTool.Common;
using EHTool.Shared.Helpers;
using EHTool.Shared.Entities;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace EHTool
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page ,INotifyPropertyChanged
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
        
        public bool IsPaneOpen { get; set; }
        
        public ObservableCollection<VideoListModel> VideoList { get; set; }
        public bool IsMiniAutoSuggestBoxShowed { get; private set; }
        public bool IsForceDecodeAudio
        {
            get
            {
                return SettingHelper.GetSetting(SettingNames.IsForceDecodeAudio,true);
            }
            set
            {
                SettingHelper.SetSetting(SettingNames.IsForceDecodeAudio, value);
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
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            CoreApplication.GetCurrentView().TitleBar.LayoutMetricsChanged += TitleBar_LayoutMetricsChanged;
            VideoList = new ObservableCollection<VideoListModel>();
            GetItems(KnownFolders.VideosLibrary);
            var a = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Window.Current.SetTitleBar(TitleBarRect);
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                StatusBar.GetForCurrentView().BackgroundColor = ColorHelper.FromArgb(255,90,77,174);
                StatusBar.GetForCurrentView().BackgroundOpacity = 1d;
                StatusBar.GetForCurrentView().ForegroundColor = Colors.White;
            }
            base.OnNavigatedTo(e);
        }

        public void SettingButtonClick()
        {
            IsPaneOpen = !IsPaneOpen;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPaneOpen)));
        }


        private async void GetItems(StorageFolder folder)
        {
            var childs = await folder.GetItemsAsync();
            for (int i = 0; i < childs.Count; i++)
            {
                if (childs[i] is StorageFolder)
                {
                    GetItems(childs[i] as StorageFolder);
                }
                else
                {
                    var file = childs[i] as StorageFile;
                    if (file.ContentType.ToLower().Contains("video"))
                    {
                        using (var thumb = await file.GetScaledImageAsThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.SingleItem))
                        {
                            using (var stream = thumb.AsStream())
                            {
                                byte[] bytes = new byte[Convert.ToUInt32(thumb.Size)];
                                await stream.ReadAsync(bytes, 0, bytes.Length);
                                VideoList.Add(new VideoListModel()
                                {
                                    Name = file.Name,
                                    ThumbImage = await ConvertHelper.ByteArrayToBitmapImage(bytes),
                                    Detail = await file.Properties.GetVideoPropertiesAsync(),
                                    FilePath = file.Path,
                                });
                            }
                        }
                    }
                    else if(FileTypeHelper.CheckFileType(file.FileType))
                    {
                        VideoList.Add(new VideoListModel()
                        {
                            Name = file.Name,
                            ThumbImage = null,
                            Detail = await file.Properties.GetVideoPropertiesAsync(),
                            FilePath = file.Path,
                        });
                    }

                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;


        public void AboutClick()
        {

        }

        public void VideoItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(VideoPlayPage),e.ClickedItem as VideoListModel);
        }

        private async void Grid_Drop(object sender, DragEventArgs e)
        {
            var def = e.GetDeferral();
            e.Handled = true;
            var file = (await e.DataView.GetStorageItemsAsync())[0] as StorageFile;
            Frame.Navigate(typeof(VideoPlayPage), file);
            def.Complete();
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

        public async void OpenFileClick()
        {
            FileOpenPicker filePicker = new FileOpenPicker();
            filePicker.ViewMode = PickerViewMode.Thumbnail;
            filePicker.SuggestedStartLocation = PickerLocationId.VideosLibrary;
            foreach (var item in FileTypeHelper.SupportTypeList)
            {
                filePicker.FileTypeFilter.Add(item);
            }
            
            StorageFile file = await filePicker.PickSingleFileAsync();
            if (file != null)
            {
                Frame.Navigate(typeof(VideoPlayPage), file);
            }

        }

        private async void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.QueryText.ToUpper() == "I AM HENTAI")
            {
                MessageDialog dialog = new MessageDialog(StaticResourceLoader.SureString);
                dialog.Commands.Add(new UICommand(StaticResourceLoader.YesString, (IUICommand command) =>
                {
                    Frame.Navigate(typeof(EHMainPage));
                    Frame.BackStack.Clear();
                }));
                dialog.Commands.Add(new UICommand(StaticResourceLoader.NoString, (IUICommand command) =>
                 {
                     Frame.Navigate(typeof(SearchResultPage), args.QueryText);
                 }));
                await dialog.ShowAsync();
            }
            else
            {
                Frame.Navigate(typeof(SearchResultPage), args.QueryText);
            }
        }

        private void MiniAutoSuggestBox_LostFocus(object sender, RoutedEventArgs e)
        {
            IsMiniAutoSuggestBoxShowed = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsMiniAutoSuggestBoxShowed)));
        }

        public void MiniSearchClick()
        {
            IsMiniAutoSuggestBoxShowed = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsMiniAutoSuggestBoxShowed)));
            MiniAutoSuggestBox.Focus(FocusState.Programmatic);
        }
    }
}
