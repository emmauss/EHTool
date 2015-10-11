using EHTool.EHTool.Common;
using EHTool.EHTool.Common.Helpers;
using EHTool.EHTool.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace EHTool.EHTool.ViewModel
{
    public class CommonReadingViewModel : ReadingViewModel
    {
        private GalleryListModel _item;
        private ImageListModel _indexItem;
        public CommonReadingViewModel(GalleryListModel item):base()
        {
            _item = item;
            LoadList();
        }
        public CommonReadingViewModel(GalleryListModel item, ImageListModel indexitem):base()
        {
            _item = item;
            _indexItem = indexitem;
            LoadList();
        }
        protected override async Task LoadListOverride()
        {
            try
            {
                GalleryDetail detail = new GalleryDetail(_item.ID, _item.Token, _item.ServerType);
                var list = (await detail.GetImagePageList()).ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    ImageList.Add(new CommonImageModel(list[i], _item.ID, i));
                }
                if (_indexItem != null)
                {
                    _selectedIndex = ImageList.ToList().FindIndex((a) => a.ImagePage == _indexItem.ImagePage);
                }
            }
            catch (System.Net.WebException)
            {
                MessageDialog dialog = new MessageDialog(StaticResourceLoader.WebErrorDialogContent, StaticResourceLoader.WebErrorDialogTitle);
                await dialog.ShowAsync();
            }
        }
    }
}
