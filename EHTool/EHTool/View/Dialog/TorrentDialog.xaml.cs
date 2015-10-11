using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using EHTool.EHTool.Model;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using EHTool.EHTool.Common;

// “内容对话框”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上进行了说明

namespace EHTool.EHTool.View
{
    public sealed partial class TorrentDialog : ContentDialog,INotifyPropertyChanged
    {
        public ObservableCollection<TorrentModel> TorrentList { get; set; } = new ObservableCollection<TorrentModel>();
        public event PropertyChangedEventHandler PropertyChanged;

        public TorrentDialog()
        {
            this.InitializeComponent();
        }

        public TorrentDialog(Task<IEnumerable<TorrentModel>> task)
        {
            this.InitializeComponent();
            LoadList(task);
        }


        private async void LoadList(Task<IEnumerable<TorrentModel>> task)
        {
            progressBar.IsIndeterminate = true;
            try
            {
                TorrentList = new ObservableCollection<TorrentModel>(await task);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TorrentList)));
            }
            catch (System.Net.WebException)
            {
                MessageDialog dialog = new MessageDialog(StaticResourceLoader.WebErrorDialogContent, StaticResourceLoader.WebErrorDialogTitle);
                await dialog.ShowAsync();
            }
            progressBar.IsIndeterminate = false;
        }

        public async void ListDoubleTapped()
        {
            progressBar.IsIndeterminate = true;
            var item = listView.SelectedItem as TorrentModel;
            var filename = item.Name;
            foreach (var charitem in Path.GetInvalidFileNameChars())
            {
                filename.Replace(charitem,' ');
            }
            var file = await DownloadsFolder.CreateFileAsync($"{filename}.torrent", CreationCollisionOption.GenerateUniqueName);
            using (var client = new HttpClient())
            {
                var bytes = await client.GetByteArrayAsync(item.TorrentLink);
                await FileIO.WriteBytesAsync(file, bytes);
            }
            progressBar.IsIndeterminate = false;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var item = listView.SelectedItem as TorrentModel;
            if (item != null)
            {
                DataPackage data = new DataPackage();
                data.SetText(item.TorrentLink);
                Clipboard.SetContent(data);
            }
        }
    }
}
