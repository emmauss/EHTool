﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EHTool.EHTool.Common;
using EHTool.EHTool.Entities;
using EHTool.EHTool.Model;
using Windows.UI.Popups;
using static EHTool.EHTool.Common.Helpers.FavorHelper;
using EHTool.EHTool.Common.Helpers;
using Windows.UI.Xaml;

namespace EHTool.EHTool.ViewModel
{
    public class DetailViewModel : GalleryDetail, INotifyPropertyChanged
    {
        public bool IsLoading { get; private set; }
        public bool IsFailed { get; private set; }
        public DetailModel DetailItem { get; private set; }
        public int[] Pages { get; set; }
        public bool IsFavor { get; private set; }
        public string FavorButtonContent
        {
            get
            {
                return IsFavor ? StaticResourceLoader.RemoveFavorString : StaticResourceLoader.AddFavorString;
            }
        }
        private int _selectedPage = -1;

        internal bool IsDownloaded;

        public int SelectedPage
        {
            get { return _selectedPage; }
            set
            {
                if (value != _selectedPage)
                {
                    _selectedPage = value;
                    LoadPageImage(value);
                }
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        internal DetailViewModel(GalleryListModel item) : base(item)
        {
            Initialize();
        }

        internal async Task Download()
        {
            if (IsDownloaded)
            {
                return;
            }
            var downloaditem = new DownloadItemModel(ListItem);
            await DownloadHelper.AddDownload(downloaditem);
            MainViewModel.Current.DownloadList.Insert(0,downloaditem);
            await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                await downloaditem.Start();
            });
        }

        internal async Task Download(string token)
        {
            if (IsDownloaded)
            {
                return;
            }
            var downloaditem = new DownloadItemModel(ListItem, token);
            await DownloadHelper.AddDownload(downloaditem);
            MainViewModel.Current.DownloadList.Insert(0,downloaditem);
            await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                await downloaditem.Start();
            });
        }

        private async void Initialize()
        {
            IsLoading = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            IsFavor = await IsFavor(Id);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFavor)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FavorButtonContent)));
            IsDownloaded = await DownloadHelper.IsDownload(Id);
            try
            {
                DetailItem = await GetDetail();
                Pages = new int[DetailItem.DetailPageCount];
                for (int i = 0; i < DetailItem.DetailPageCount; i++)
                {
                    Pages[i] = i + 1;
                }
                _selectedPage = 0;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DetailItem)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Pages)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedPage)));
            }
            catch (System.Net.Http.HttpRequestException)
            {
                IsFailed = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFailed)));
                MessageDialog dialog = new MessageDialog(StaticResourceLoader.WebErrorDialogContent, StaticResourceLoader.WebErrorDialogTitle);
                await dialog.ShowAsync();
            }
            catch (System.Net.WebException)
            {
                IsFailed = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFailed)));
                MessageDialog dialog = new MessageDialog(StaticResourceLoader.WebErrorDialogContent, StaticResourceLoader.WebErrorDialogTitle);
                await dialog.ShowAsync();
            }
            IsLoading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
        }
        internal async Task FavorHandler()
        {
            if (IsFavor)
            {
                await RemoveFavor(Id);
            }
            else
            {
                await AddFavor(ListItem);
            }
            IsFavor = await IsFavor(Id);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFavor)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FavorButtonContent)));
        }
        internal async void LoadPageImage(int page)
        {
            IsLoading = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            try
            {
                var item = await GetDetail(page);
                DetailItem.ImageList = item.ImageList;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DetailItem)));
            }
            catch (System.Net.Http.HttpRequestException)
            {
                IsFailed = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFailed)));
                MessageDialog dialog = new MessageDialog(StaticResourceLoader.WebErrorDialogContent, StaticResourceLoader.WebErrorDialogTitle);
                await dialog.ShowAsync();
            }
            catch (System.Net.WebException)
            {
                IsFailed = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFailed)));
                MessageDialog dialog = new MessageDialog(StaticResourceLoader.WebErrorDialogContent, StaticResourceLoader.WebErrorDialogTitle);
                await dialog.ShowAsync();
            }
            IsLoading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
        }

        //internal Task<IEnumerable<ImageListModel>> GetImagePageListTask()
        //    => GetImagePageList(DetailItem.ImageList, DetailItem.DetailPageCount);

    }
}
