using EHTool.EHTool.Common;
using EHTool.EHTool.Common.Helpers;
using EHTool.EHTool.Entities;
using EHTool.EHTool.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace EHTool.EHTool.ViewModel
{
    public class TagSearchViewModel : TagGallery, INotifyPropertyChanged
    {
        public bool IsLoading { get; private set; }
        public bool IsFailed { get; private set; }
        public string TagValue => _tagValue;
        public ObservableCollection<GalleryListModel> MainList { get; private set; } = new ObservableCollection<GalleryListModel>();

        private int _currentPage = 0;
        public event PropertyChangedEventHandler PropertyChanged;

        internal TagSearchViewModel(string tagValue,ServerTypes type):base(tagValue,type)
        {
            Initialize();
        }

        private async void Initialize()
        {
            _currentPage = 0;
            IsLoading = true;
            IsFailed = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            try
            {
                MainList = new ObservableCollection<GalleryListModel>(await GetGalleryList());
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MainList)));
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
        internal async Task LoadMore()
        {
            if (IsFailed)
            {
                return;
            }
            IsLoading = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            IEnumerable<GalleryListModel> list = null;
            try
            {
                if (MaxPageCount > _currentPage + 1)
                {
                    list = await GetGalleryList(++_currentPage);
                    foreach (var item in list)
                    {
                        MainList.Add(item);
                    }
                }
            }
            catch (System.Net.WebException)
            {
                _currentPage--;
                MessageDialog dialog = new MessageDialog($"Can not connect to {ServerType}", "Web Error");
                await dialog.ShowAsync();
            }
            IsLoading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));

        }

    }
}
