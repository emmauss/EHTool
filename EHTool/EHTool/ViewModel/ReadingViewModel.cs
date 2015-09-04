using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EHTool.EHTool.Model;

namespace EHTool.EHTool.ViewModel
{
    public class ReadingViewModel : INotifyPropertyChanged
    {

        public ObservableCollection<ImageModel> ImageList { get; private set; } = new ObservableCollection<ImageModel>();
        public int SelectedIndex { get; set; } = 0;
        public int MaxPageCount => ImageList.Count;


        public event PropertyChangedEventHandler PropertyChanged;
        private string _id;

        public ReadingViewModel(Task<IEnumerable<ImageListModel>> task, string id)
        {
            _id = id;
            LoadList(task);
        }

        private async void LoadList(Task<IEnumerable<ImageListModel>> task)
        {
            var list = (await task).ToList();
            for (int i = 0; i < list.Count; i++)
            {
                ImageList.Add(new ImageModel(list[i], i, _id));
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MaxPageCount)));
        }


    }
}
