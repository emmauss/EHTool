﻿using System;
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
using System.Diagnostics;
using EHTool.EHTool.Common.Helpers;
using Windows.Storage;
using Windows.Storage.AccessCache;

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
        public bool IsDownloadPaneOpen { get; set; }
        public bool IsLocalFolderPaneOpen { get; set; }
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
                //await MainVM.LoadDownloadList();
            }
            base.OnNavigatedTo(e);
        }

        private void RefreshClick(object sender, RoutedEventArgs e)
        {
            MainVM.Refresh();
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
                case 2:
                    IsDownloadPaneOpen = !IsDownloadPaneOpen;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDownloadPaneOpen)));
                    break;
                case 3:
                    IsLocalFolderPaneOpen = !IsLocalFolderPaneOpen;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLocalFolderPaneOpen)));
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
        public void DownloadItemClick(object sender,ItemClickEventArgs e)
        {
            var item = e.ClickedItem as DownloadItemModel;
            item.Pause();
            Frame.Navigate(typeof(EHReadingPage), new ReadingViewModel(item));
        }

        #region DownloadRightTapped
        private DownloadItemModel _clickedItem;
        public void DownloadRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            _clickedItem = (e.OriginalSource as FrameworkElement).DataContext as DownloadItemModel;
            if (_clickedItem != null)
            {
                Debug.WriteLine($"{_clickedItem?.Title} right clicked");
                var menu = Resources["DownloadMenu"] as MenuFlyout;
                menu.ShowAt(null, e.GetPosition(null));
                e.Handled = true;
            }
        }
        public async void StartClicked()
        {
            await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                await _clickedItem.Start();
            });
        }
        public void PauseClicked()
        {
            _clickedItem.Pause();
        }
        public async void DeleteClicked()
        {
            MessageDialog dialog = new MessageDialog("Delete From Your Computer?", "Delete");
            dialog.Commands.Add(new UICommand("Delete", async (IUICommand command) =>
            {
                _clickedItem.Pause();
                MainVM.DownloadList.Remove(_clickedItem);
                await DownloadHelper.RemoveDownload(_clickedItem);
            }));
            dialog.Commands.Add(new UICommand("No"));
            await dialog.ShowAsync();
        }
        public async void RemoveClicked()
        {
            MessageDialog dialog = new MessageDialog("Remove From The List?", "Remove");
            dialog.Commands.Add(new UICommand("Remove", async (IUICommand command) =>
            {
                _clickedItem.Pause();
                MainVM.DownloadList.Remove(_clickedItem);
                await DownloadHelper.RemoveDownload(_clickedItem, false);
            }));
            dialog.Commands.Add(new UICommand("No"));
            await dialog.ShowAsync();
        }

        #endregion

        private async void PivotItem_DragOver(object sender, DragEventArgs e)
        {
            var def = e.GetDeferral();
            e.Handled = true;
            var folder = (await e.DataView.GetStorageItemsAsync())[0] as StorageFolder;
            e.AcceptedOperation = folder != null ? Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy : Windows.ApplicationModel.DataTransfer.DataPackageOperation.None;
            def.Complete();
        }

        private async void PivotItem_Drop(object sender, DragEventArgs e)
        {
            var def = e.GetDeferral();
            e.Handled = true;
            var items = await e.DataView.GetStorageItemsAsync();
            foreach (var item in items)
            {
                if (item.IsOfType(StorageItemTypes.Folder))
                {
                    var folder = item as StorageFolder;
                    var token = StorageApplicationPermissions.FutureAccessList.Add(folder);
                    await LocalFolderHelper.Add(token, folder.DisplayName);
                    await MainVM.LoadLocalFolderList();
                }
            }
            def.Complete();
        }
        LocalFolderModel _clickedFolder;
        public void LocalFolderClick(object sender,ItemClickEventArgs e)
        {
            var item = e.ClickedItem as LocalFolderModel;
            Frame.Navigate(typeof(EHReadingPage), new ReadingViewModel(item));
        }
        public void LocalFolderRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            _clickedFolder = (e.OriginalSource as FrameworkElement).DataContext as LocalFolderModel;
            if (_clickedFolder != null)
            {
                Debug.WriteLine($"{_clickedFolder?.FolderName} right clicked");
                var menu = Resources["LocalFolderMenu"] as MenuFlyout;
                menu.ShowAt(null, e.GetPosition(null));
                e.Handled = true;
            }
        }
        public async void LocalFolderRemoveClicked()
        {
            await LocalFolderHelper.Remove(_clickedFolder);
            MainVM.LocalFolderList.Remove(_clickedFolder);
        }
    }
}
