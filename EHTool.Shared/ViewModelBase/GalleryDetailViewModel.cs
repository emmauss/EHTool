using EHTool.Shared.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EHTool.Shared.ViewModelBase
{
    public abstract class GalleryDetailViewModel : GalleryDetail, INotifyPropertyChanged
    {
        protected abstract void OnWebErrorOverride();
        protected abstract Task<bool> CheckForFavorOverrideAsync();
        protected abstract Task<bool> CheckForDownloadStateOverrideAsync();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName]string name = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        public bool IsLoading { get; protected set; }
        public bool IsFailed { get; protected set; }
        public DetailModel DetailItem { get; protected set; }
        public int[] Pages { get; set; }
        public bool IsFavor { get; protected set; }
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

        internal GalleryDetailViewModel(GalleryListModel item) : base(item)
        {
            Initialize();
        }
        

        protected async void Initialize()
        {
            IsLoading = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            IsFavor = await CheckForFavorOverrideAsync();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFavor)));
            IsDownloaded = await CheckForDownloadStateOverrideAsync();
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
            catch (System.Net.Http.HttpRequestException)
            {
                IsFailed = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFailed)));
                OnWebErrorOverride();
            }
            catch (System.Net.WebException)
            {
                IsFailed = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFailed)));
                OnWebErrorOverride();
            }
            IsLoading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
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
                OnWebErrorOverride();
            }
            catch (System.Net.WebException)
            {
                IsFailed = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFailed)));
                OnWebErrorOverride();
            }
            IsLoading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
        }



    }
}
