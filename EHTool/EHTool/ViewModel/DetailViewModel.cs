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
using static EHTool.EHTool.Common.Helpers.FavorHelper;

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
                return IsFavor ? "Remove Favor" : "Add Favor";
            }
        }
        private int _selectedPage = -1;

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

        public DetailViewModel(GalleryListModel item) : base(item)
        {
            Initialize();
        }

        private async void Initialize()
        {
            IsLoading = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            IsFavor = await IsFavor(Id);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFavor)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FavorButtonContent)));
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
        public async Task FavorHandler()
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
        public async void LoadPageImage(int page)
        {
            IsLoading = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            try
            {
                var item = await GetDetail(page);
                DetailItem.ImageList = item.ImageList;
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
