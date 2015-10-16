using System;
using System.ComponentModel;
using System.Threading.Tasks;
using EHTool.EHTool.Common;
using EHTool.EHTool.Model;
using Windows.UI.Popups;
using EHTool.EHTool.Common.Helpers;
using Windows.UI.Xaml;
using EHTool.Shared;
using EHTool.Shared.Model;
using static EHTool.EHTool.Common.Helpers.FavorHelper;
using EHTool.Shared.ViewModelBase;

namespace EHTool.EHTool.ViewModel
{
    public class DetailViewModel : GalleryDetailViewModel
    {
        internal DetailViewModel(GalleryListModel item) : base(item)
        {

        }

        internal async Task Download()
        {
            if (!IsDownloaded)
            {
                var downloaditem = new DownloadItemModel(ListItem);
                await DownloadHelper.AddDownload(downloaditem);
                MainViewModel.Current.DownloadList.Insert(0, downloaditem);
                await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () => await downloaditem.Start());
            }
        }

        internal async Task Download(string token)
        {
            if (!IsDownloaded)
            {
                var downloaditem = new DownloadItemModel(ListItem, token);
                await DownloadHelper.AddDownload(downloaditem);
                MainViewModel.Current.DownloadList.Insert(0, downloaditem);
                await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () => await downloaditem.Start());
            }
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
            OnPropertyChanged(nameof(IsFavor));
        }

        protected override async void OnWebErrorOverride()
            => await new MessageDialog(StaticResourceLoader.WebErrorDialogContent, StaticResourceLoader.WebErrorDialogTitle).ShowAsync();

        protected override Task<bool> CheckForFavorOverrideAsync() => IsFavor(Id);
        protected override Task<bool> CheckForDownloadStateOverrideAsync() => DownloadHelper.IsDownload(Id);
    }
}
