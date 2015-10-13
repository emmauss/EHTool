using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Common.Converters;
using EHTool.Common.Helpers;
using EHTool.DHPlayer.Model;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using EHTool.Shared.Helpers;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace EHTool.DHPlayer.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SearchResultPage : Page ,INotifyPropertyChanged
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

        public string SearchKey { get; set; }
        public ObservableCollection<VideoListModel> VideoList { get; set; } = new ObservableCollection<VideoListModel>();
        public SearchResultPage()
        {
            this.InitializeComponent();
            Window.Current.SetTitleBar(TitleBarRect);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            SearchKey = e.Parameter as string;
            base.OnNavigatedTo(e);
            await GetItems(KnownFolders.VideosLibrary);
        }

        private async Task GetItems(StorageFolder folder)
        {
            var childs = await folder.GetItemsAsync();
            for (int i = 0; i < childs.Count; i++)
            {
                if (childs[i] is StorageFolder)
                {
                    await GetItems(childs[i] as StorageFolder);
                }
                else
                {
                    var file = childs[i] as StorageFile;
                    if (file.DisplayName.ToLower().Contains(SearchKey.ToLower()))
                    {
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
                        else if (FileTypeHelper.CheckFileType(file.FileType))
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
        }


        public void VideoItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(VideoPlayPage), e.ClickedItem as VideoListModel);
        }

        public void BackClick()
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
    }
}
