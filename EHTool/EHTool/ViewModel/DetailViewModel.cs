using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EHTool.EHTool.Common;
using EHTool.EHTool.Entities;
using EHTool.EHTool.Model;
using Windows.UI.Popups;

namespace EHTool.EHTool.ViewModel
{
    public class DetailViewModel : GalleryDetail, INotifyPropertyChanged
    {
        public bool IsLoading { get; private set; }
        public bool IsFailed { get; private set; }
        public DetailModel DetailItem { get; private set; }
        public int[] Pages { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public DetailViewModel(GalleryListModel item) : base(item)
        {
            Initialize();
        }

        private async void Initialize()
        {
            IsLoading = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            try
            {
                DetailItem = await GetDetail();
                Pages = new int[DetailItem.DetailPageCount];
                for (int i = 0; i < DetailItem.DetailPageCount; i++)
                {
                    Pages[i] = i + 1;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DetailItem)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Pages)));
            }
            catch (System.Net.WebException)
            {
                IsFailed = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFailed)));
                MessageDialog dialog = new MessageDialog($"Can not connect to {ServerType}", "Web Error");
                await dialog.ShowAsync();
            }
            IsLoading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
        }

        public async Task LoadPageImage(int page)
        {
            IsLoading = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            try
            {
                DetailItem = await GetDetail(page);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DetailItem)));
            }
            catch (System.Net.WebException)
            {
                IsFailed = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFailed)));
                MessageDialog dialog = new MessageDialog($"Can not connect to {ServerType}", "Web Error");
                await dialog.ShowAsync();
            }
            IsLoading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
        }

        public Task<IEnumerable<ImageListModel>> GetImagePageListTask()
            => GetImagePageList(DetailItem.ImageList, DetailItem.DetailPageCount);
    }
}
