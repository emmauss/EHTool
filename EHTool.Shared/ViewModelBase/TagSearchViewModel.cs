using EHTool.Shared.Entities;
using EHTool.Shared.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
#if WINDOWS_UWP
using Windows.UI.Popups;
using EHTool.EHTool.Common;
#endif

namespace EHTool.Shared.ViewModelBase
{
    public class TagSearchViewModel : TagGallery, INotifyPropertyChanged
    {
        public bool IsLoading { get; private set; }
        public bool IsFailed { get; private set; }
        public string TagValue => _tagValue;
        public ObservableCollection<GalleryListModel> MainList { get; private set; } = new ObservableCollection<GalleryListModel>();

        private int _currentPage = 0;
        public event PropertyChangedEventHandler PropertyChanged;

        internal TagSearchViewModel(string tagValue, ServerTypes type) : base(tagValue, type)
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
            catch (System.Net.Http.HttpRequestException)
            {
                OnWebError();
            }
            catch (System.Net.WebException)
            {
                OnWebError();
            }
            IsLoading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
        }

        private async void OnWebError()
        {
            IsFailed = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFailed)));
#if WINDOWS_UWP
                await new MessageDialog(StaticResourceLoader.WebErrorDialogContent, StaticResourceLoader.WebErrorDialogTitle).ShowAsync();
#else

#endif
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
            catch (System.Net.Http.HttpRequestException)
            {
                OnWebError();
            }
            catch (System.Net.WebException)
            {
                OnWebError();
            }
            IsLoading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));

        }

    }
}
